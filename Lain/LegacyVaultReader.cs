using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lain
{
    /*
     * Reads a v1 Lain.data file - a List<LainAccount> written by BinaryFormatter - without using BinaryFormatter.
     *
     * This is a passive parser for the MS-NRBF record stream: it never loads types, calls constructors or runs
     * any code from the file. It only collects strings, class member values and arrays, then checks that the
     * result has exactly the expected shape (a list of accounts with string fields) before building accounts.
     * Every length and count is bounds-checked; anything unexpected throws InvalidDataException.
     */
    internal static class LegacyVaultReader
    {
        const int MaxRecords = 4000000;
        const int MaxArrayLength = 1 << 22;
        const int MaxMembers = 256;
        const int MaxDepth = 32;

        const string ListTypePrefix = "System.Collections.Generic.List`1[[Lain.LainAccount,";
        const string AccountType = "Lain.LainAccount";

        internal static List<LainAccount> Read(byte[] data)
        {
            if (data == null) throw new InvalidDataException("The legacy vault data is empty.");

            try
            {
                return new Parser(data).Run();
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is OverflowException || ex is DecoderFallbackException
                || ex is IndexOutOfRangeException || ex is KeyNotFoundException || ex is InvalidCastException)
            {
                throw new InvalidDataException("The legacy vault data is malformed.");
            }
        }

        // Binary type of a class member (MS-NRBF BinaryTypeEnum).
        enum BinaryType : byte { Primitive = 0, String = 1, Object = 2, SystemClass = 3, Class = 4, ObjectArray = 5, StringArray = 6, PrimitiveArray = 7 }

        sealed class ClassInfo
        {
            internal string Name;
            internal string[] Members;
            internal BinaryType[] Types;     // null for records without member type information
            internal byte[] PrimitiveTypes;
        }

        sealed class ClassObject
        {
            internal ClassInfo Info;
            internal object[] Values;
        }

        sealed class ArrayObject
        {
            internal object[] Items;
        }

        sealed class Reference
        {
            internal int Id;
        }

        sealed class NullRun
        {
            internal int Count;
        }

        // Value of a primitive member we don't need (anything but Int32).
        static readonly object OtherPrimitive = new object();
        static readonly object EndMarker = new object();

        sealed class Parser
        {
            readonly byte[] _b;
            int _pos;
            int _records;
            int _depth;
            long _elements;
            readonly Dictionary<int, object> _objects = new Dictionary<int, object>();
            readonly Dictionary<int, ClassInfo> _classes = new Dictionary<int, ClassInfo>();
            readonly HashSet<int> _libraries = new HashSet<int>();
            static readonly Encoding Utf8 = new UTF8Encoding(false, false);

            internal Parser(byte[] data)
            {
                _b = data;
            }

            internal List<LainAccount> Run()
            {
                // SerializationHeaderRecord
                if (ReadByte() != 0) throw Bad("missing header");
                int rootId = ReadInt32();
                ReadInt32();
                if (ReadInt32() != 1 || ReadInt32() != 0) throw Bad("unsupported version");

                while (true)
                {
                    object record = ReadRecord();
                    if (record == EndMarker) break;
                    if (record is NullRun) throw Bad("unexpected null run");
                }

                return Build(rootId);
            }

            #region Shape validation

            List<LainAccount> Build(int rootId)
            {
                ClassObject list = Resolve(new Reference { Id = rootId }) as ClassObject;
                if (list == null || !list.Info.Name.StartsWith(ListTypePrefix, StringComparison.Ordinal)) throw Bad("the root is not an account list");

                object sizeValue = Member(list, "_size");
                if (!(sizeValue is int)) throw Bad("the account list has no size");
                int size = (int)sizeValue;

                object itemsValue = Resolve(Member(list, "_items"));
                if (itemsValue == null)
                {
                    if (size != 0) throw Bad("the account list is inconsistent");
                    return new List<LainAccount>();
                }

                ArrayObject items = itemsValue as ArrayObject;
                if (items == null || size < 0 || size > items.Items.Length) throw Bad("the account list is inconsistent");

                List<LainAccount> accounts = new List<LainAccount>(size);
                for (int i = 0; i < size; i++)
                {
                    object item = Resolve(items.Items[i]);
                    if (item == null) continue;

                    ClassObject account = item as ClassObject;
                    if (account == null || account.Info.Name != AccountType) throw Bad("the list contains something other than an account");

                    accounts.Add(new LainAccount(Field(account, "_Name"), Field(account, "_Email"), Field(account, "_Password"),
                        Field(account, "_Link"), Field(account, "_Note")));
                }

                return accounts;
            }

            static object Member(ClassObject obj, string name)
            {
                int index = Array.IndexOf(obj.Info.Members, name);
                return index < 0 ? null : obj.Values[index];
            }

            // Account fields are strings or null; a field missing from older files is treated as empty (null).
            string Field(ClassObject account, string name)
            {
                object value = Resolve(Member(account, name));
                if (value != null && !(value is string)) throw Bad("an account field is not text");
                return (string)value;
            }

            object Resolve(object value)
            {
                Reference reference = value as Reference;
                if (reference == null) return value;

                object target;
                if (!_objects.TryGetValue(reference.Id, out target)) throw Bad("a reference points to a missing object");
                return target;
            }

            #endregion

            #region Records

            object ReadRecord()
            {
                if (++_records > MaxRecords) throw Bad("too many records");
                if (++_depth > MaxDepth) throw Bad("nesting is too deep");

                try
                {
                    byte type = ReadByte();

                    // BinaryLibrary records only name an assembly and precede the record that uses it
                    while (type == 12)
                    {
                        if (++_records > MaxRecords) throw Bad("too many records");
                        if (!_libraries.Add(ReadInt32())) throw Bad("duplicate library");
                        ReadString();
                        type = ReadByte();
                    }

                    switch (type)
                    {
                        case 1: // ClassWithId
                            {
                                int id = ReadInt32();
                                ClassInfo info;
                                if (!_classes.TryGetValue(ReadInt32(), out info)) throw Bad("unknown class metadata");
                                return ReadClassValues(id, info);
                            }
                        case 2: // SystemClassWithMembers
                            return ReadClass(false, false);
                        case 3: // ClassWithMembers
                            return ReadClass(false, true);
                        case 4: // SystemClassWithMembersAndTypes
                            return ReadClass(true, false);
                        case 5: // ClassWithMembersAndTypes
                            return ReadClass(true, true);
                        case 6: // BinaryObjectString
                            {
                                int id = ReadInt32();
                                string s = ReadString();
                                Register(id, s);
                                return s;
                            }
                        case 7: // BinaryArray
                            return ReadBinaryArray();
                        case 8: // MemberPrimitiveTyped
                            return ReadPrimitive(ReadByte());
                        case 9: // MemberReference
                            return new Reference { Id = ReadInt32() };
                        case 10: // ObjectNull
                            return null;
                        case 11: // MessageEnd
                            if (_depth != 1) throw Bad("unexpected end");
                            return EndMarker;
                        case 13: // ObjectNullMultiple256
                            return new NullRun { Count = ReadByte() };
                        case 14: // ObjectNullMultiple
                            return new NullRun { Count = ReadInt32() };
                        case 15: // ArraySinglePrimitive
                            {
                                int id = ReadInt32();
                                int length = ReadLength();
                                byte primitive = ReadByte();
                                object[] items = new object[length];
                                for (int i = 0; i < length; i++) items[i] = ReadPrimitive(primitive);
                                return RegisterArray(id, items);
                            }
                        case 16: // ArraySingleObject
                        case 17: // ArraySingleString
                            {
                                int id = ReadInt32();
                                return RegisterArray(id, ReadElements(ReadLength()));
                            }
                        default:
                            throw Bad("unsupported record type " + type);
                    }
                }
                finally
                {
                    _depth--;
                }
            }

            object ReadClass(bool hasTypes, bool hasLibrary)
            {
                int id = ReadInt32();
                ClassInfo info = new ClassInfo { Name = ReadString() };

                int count = ReadInt32();
                if (count < 0 || count > MaxMembers) throw Bad("invalid member count");
                info.Members = new string[count];
                for (int i = 0; i < count; i++) info.Members[i] = ReadString();

                if (hasTypes)
                {
                    info.Types = new BinaryType[count];
                    info.PrimitiveTypes = new byte[count];
                    for (int i = 0; i < count; i++)
                    {
                        byte t = ReadByte();
                        if (t > (byte)BinaryType.PrimitiveArray) throw Bad("invalid member type");
                        info.Types[i] = (BinaryType)t;
                    }
                    for (int i = 0; i < count; i++) ReadTypeInfo(info.Types[i], out info.PrimitiveTypes[i]);
                }

                if (hasLibrary) ReadInt32();

                if (_classes.ContainsKey(id)) throw Bad("duplicate class metadata");
                _classes[id] = info;
                return ReadClassValues(id, info);
            }

            void ReadTypeInfo(BinaryType type, out byte primitive)
            {
                primitive = 0;
                switch (type)
                {
                    case BinaryType.Primitive:
                    case BinaryType.PrimitiveArray:
                        primitive = ReadByte();
                        break;
                    case BinaryType.SystemClass:
                        ReadString();
                        break;
                    case BinaryType.Class:
                        ReadString();
                        ReadInt32();
                        break;
                }
            }

            object ReadClassValues(int id, ClassInfo info)
            {
                ClassObject obj = new ClassObject { Info = info, Values = new object[info.Members.Length] };
                Register(id, obj);

                for (int i = 0; i < obj.Values.Length; i++)
                {
                    if (info.Types != null && info.Types[i] == BinaryType.Primitive)
                    {
                        obj.Values[i] = ReadPrimitive(info.PrimitiveTypes[i]);
                    }
                    else
                    {
                        object value = ReadRecord();
                        if (value is NullRun || value == EndMarker) throw Bad("invalid member value");
                        obj.Values[i] = value;
                    }
                }
                return obj;
            }

            object ReadBinaryArray()
            {
                int id = ReadInt32();
                byte arrayType = ReadByte();
                int rank = ReadInt32();
                // only single-dimension, zero-based arrays (Single / Jagged) are expected
                if ((arrayType != 0 && arrayType != 1) || rank != 1) throw Bad("unsupported array");
                int length = ReadLength();

                byte t = ReadByte();
                if (t > (byte)BinaryType.PrimitiveArray) throw Bad("invalid array type");
                byte primitive;
                ReadTypeInfo((BinaryType)t, out primitive);

                object[] items;
                if ((BinaryType)t == BinaryType.Primitive)
                {
                    items = new object[length];
                    for (int i = 0; i < length; i++) items[i] = ReadPrimitive(primitive);
                }
                else
                {
                    items = ReadElements(length);
                }
                return RegisterArray(id, items);
            }

            object[] ReadElements(int length)
            {
                object[] items = new object[length];
                int i = 0;
                while (i < length)
                {
                    object value = ReadRecord();
                    if (value == EndMarker) throw Bad("unexpected end");

                    NullRun nulls = value as NullRun;
                    if (nulls != null)
                    {
                        if (nulls.Count <= 0 || nulls.Count > length - i) throw Bad("invalid null run");
                        i += nulls.Count;
                    }
                    else
                    {
                        items[i++] = value;
                    }
                }
                return items;
            }

            object RegisterArray(int id, object[] items)
            {
                ArrayObject array = new ArrayObject { Items = items };
                Register(id, array);
                return array;
            }

            void Register(int id, object value)
            {
                if (_objects.ContainsKey(id)) throw Bad("duplicate object id");
                _objects[id] = value;
            }

            object ReadPrimitive(byte type)
            {
                switch (type)
                {
                    case 1: Skip(1); return OtherPrimitive;   // Boolean
                    case 2: Skip(1); return OtherPrimitive;   // Byte
                    case 5: ReadString(); return OtherPrimitive; // Decimal (as text)
                    case 6: Skip(8); return OtherPrimitive;   // Double
                    case 7: Skip(2); return OtherPrimitive;   // Int16
                    case 8: return ReadInt32();               // Int32
                    case 9: Skip(8); return OtherPrimitive;   // Int64
                    case 10: Skip(1); return OtherPrimitive;  // SByte
                    case 11: Skip(4); return OtherPrimitive;  // Single
                    case 12: Skip(8); return OtherPrimitive;  // TimeSpan
                    case 13: Skip(8); return OtherPrimitive;  // DateTime
                    case 14: Skip(2); return OtherPrimitive;  // UInt16
                    case 15: Skip(4); return OtherPrimitive;  // UInt32
                    case 16: Skip(8); return OtherPrimitive;  // UInt64
                    default: throw Bad("unsupported primitive type " + type);
                }
            }

            #endregion

            #region Bytes

            void Need(int count)
            {
                if (count < 0 || count > _b.Length - _pos) throw Bad("the file is truncated");
            }

            void Skip(int count)
            {
                Need(count);
                _pos += count;
            }

            byte ReadByte()
            {
                Need(1);
                return _b[_pos++];
            }

            int ReadInt32()
            {
                Need(4);
                int v = BitConverter.ToInt32(_b, _pos);
                _pos += 4;
                return v;
            }

            // Array lengths are capped, per array and in total, so a tiny file can't demand huge allocations.
            int ReadLength()
            {
                int length = ReadInt32();
                if (length < 0 || length > MaxArrayLength) throw Bad("invalid array length");
                _elements += length;
                if (_elements > MaxArrayLength) throw Bad("too many array elements");
                return length;
            }

            // LengthPrefixedString: 7-bit encoded byte count followed by UTF-8 bytes.
            string ReadString()
            {
                int length = 0;
                for (int shift = 0; ; shift += 7)
                {
                    if (shift > 28) throw Bad("invalid string length");
                    byte b = ReadByte();
                    length |= (b & 0x7F) << shift;
                    if ((b & 0x80) == 0) break;
                }
                Need(length);
                string s = Utf8.GetString(_b, _pos, length);
                _pos += length;
                return s;
            }

            #endregion

            static InvalidDataException Bad(string reason)
            {
                return new InvalidDataException("The legacy vault data is malformed (" + reason + ").");
            }
        }
    }
}
