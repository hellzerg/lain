# BEFORE CONTINUING, MAKE SURE YOU HAVE A WORKING BACKUP OF YOUR PROFILE! #
## How to use a custom salt with Lain ##

#### Custom salt SHOULD be used ONLY when setting up a fresh Lain profile! ####
#### Accounts encrypted with a different salt, can't be decrypted! ####
#### Salt should be at least 32 characters long. ####

#### Using command-line: ####
- Lain.exe /salt=CUSTOMSALT

#### Using a file: ####
- Place a Lain.salt file in Data folder with your desired salt in it and run the app normally.

## How to change your current salt (dangerous) ##

#### If you aren't using a custom salt: ####
- Lain.exe /changesalt=YOUR_PASSWORD,NEW_SALT

#### CHanging from a custom salt to a new one: ####
- Lain.exe /changesalt=YOUR_PASSWORD,OLD_SALT,NEW_SALT
