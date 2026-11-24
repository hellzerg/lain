namespace Lain
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new Lain.FluentLabel();
            this.lblAppearance = new Lain.FluentLabel();
            this.rSystem = new Lain.MoonRadio();
            this.rLight = new Lain.MoonRadio();
            this.rDark = new Lain.MoonRadio();
            this.label27 = new Lain.FluentLabel();
            this.themeCard = new Lain.Card();
            this.rAmethyst = new Lain.MoonRadio();
            this.rAzurite = new Lain.MoonRadio();
            this.rRuby = new Lain.MoonRadio();
            this.rJade = new Lain.MoonRadio();
            this.rAmber = new Lain.MoonRadio();
            this.rSilver = new Lain.MoonRadio();
            this.label2 = new Lain.FluentLabel();
            this.sizePanel = new Lain.Card();
            this.rSmall = new Lain.MoonRadio();
            this.rNormal = new Lain.MoonRadio();
            this.rLarge = new Lain.MoonRadio();
            this.lblBehavior = new Lain.FluentLabel();
            this.lblSecurity = new Lain.FluentLabel();
            this.securityCard = new Lain.Card();
            this.behaviorCard = new Lain.Card();
            this.chkAutoSizeColumns = new Lain.MoonCheck();
            this.chkShowCreated = new Lain.MoonCheck();
            this.chkShowModified = new Lain.MoonCheck();
            this.lblSpacing = new Lain.FluentLabel();
            this.spacingPanel = new Lain.Card();
            this.rComfortable = new Lain.MoonRadio();
            this.rCompact = new Lain.MoonRadio();
            this.chkHidePass = new Lain.MoonCheck();
            this.chkAlwaysPass = new Lain.MoonCheck();
            this.chkAutoStart = new Lain.MoonCheck();
            this.chkAutoLock = new Lain.MoonCheck();
            this.timerBox = new Lain.InputBox();
            this.txtTimer = new Lain.FluentTextBox();
            this.lblMins = new Lain.FluentLabel();
            this.chkClearClip = new Lain.MoonCheck();
            this.clipBox = new Lain.InputBox();
            this.txtClipSeconds = new Lain.FluentTextBox();
            this.lblSecs = new Lain.FluentLabel();
            this.chkMinLength = new Lain.MoonCheck();
            this.minLengthBox = new Lain.InputBox();
            this.txtMinLength = new Lain.FluentTextBox();
            this.lblMinLength = new Lain.FluentLabel();
            this.chkMinPerType = new Lain.MoonCheck();
            this.minPerTypeBox = new Lain.InputBox();
            this.txtMinPerType = new Lain.FluentTextBox();
            this.lblMinPerType = new Lain.FluentLabel();
            this.lblStartup = new Lain.FluentLabel();
            this.startupCard = new Lain.Card();
            this.lblGenerator = new Lain.FluentLabel();
            this.generatorCard = new Lain.Card();
            this.lblSymbols = new Lain.FluentLabel();
            this.symbolsBox = new Lain.InputBox();
            this.txtSymbols = new Lain.FluentTextBox();
            this.btnSymbolPresets = new Lain.ModernButton();
            this.footer = new Lain.FooterPanel();
            this.btnOk = new Lain.ModernButton();
            this.themeCard.SuspendLayout();
            this.sizePanel.SuspendLayout();
            this.spacingPanel.SuspendLayout();
            this.securityCard.SuspendLayout();
            this.behaviorCard.SuspendLayout();
            this.startupCard.SuspendLayout();
            this.generatorCard.SuspendLayout();
            this.symbolsBox.SuspendLayout();
            this.timerBox.SuspendLayout();
            this.clipBox.SuspendLayout();
            this.minLengthBox.SuspendLayout();
            this.minPerTypeBox.SuspendLayout();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.btnChangePassword = new Lain.ModernButton();
            this.btnSecurityKeys = new Lain.ModernButton();
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = Fluent.Display(16F);
            this.lblTitle.Location = new System.Drawing.Point(26, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Settings";
            //
            // lblAppearance
            //
            this.lblAppearance.Font = Fluent.UiSemibold(9F);
            this.lblAppearance.Location = new System.Drawing.Point(468, 18);
            this.lblAppearance.Name = "lblAppearance";
            this.lblAppearance.Size = new System.Drawing.Size(94, 30);
            this.lblAppearance.Text = "Appearance";
            this.lblAppearance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // rSystem
            //
            this.rSystem.Location = new System.Drawing.Point(562, 16);
            this.rSystem.Name = "rSystem";
            this.rSystem.Size = new System.Drawing.Size(90, 34);
            this.rSystem.Style = Lain.RadioStyle.Segment;
            this.rSystem.TabIndex = 8;
            this.rSystem.Text = "System";
            this.rSystem.CheckedChanged += new System.EventHandler(this.appearance_CheckedChanged);
            //
            // rLight
            //
            this.rLight.Location = new System.Drawing.Point(654, 16);
            this.rLight.Name = "rLight";
            this.rLight.Size = new System.Drawing.Size(82, 34);
            this.rLight.Style = Lain.RadioStyle.Segment;
            this.rLight.TabIndex = 9;
            this.rLight.Text = "Light";
            this.rLight.CheckedChanged += new System.EventHandler(this.appearance_CheckedChanged);
            //
            // rDark
            //
            this.rDark.Location = new System.Drawing.Point(738, 16);
            this.rDark.Name = "rDark";
            this.rDark.Size = new System.Drawing.Size(82, 34);
            this.rDark.Style = Lain.RadioStyle.Segment;
            this.rDark.TabIndex = 10;
            this.rDark.Text = "Dark";
            this.rDark.CheckedChanged += new System.EventHandler(this.appearance_CheckedChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = Fluent.UiSemibold(10F);
            this.label27.Location = new System.Drawing.Point(29, 66);
            this.label27.Name = "label27";
            this.label27.Text = "Accent color";
            // 
            // themeCard
            // 
            this.themeCard.Controls.Add(this.rAmethyst);
            this.themeCard.Controls.Add(this.rAzurite);
            this.themeCard.Controls.Add(this.rRuby);
            this.themeCard.Controls.Add(this.rJade);
            this.themeCard.Controls.Add(this.rAmber);
            this.themeCard.Controls.Add(this.rSilver);
            this.themeCard.Location = new System.Drawing.Point(30, 88);
            this.themeCard.Name = "themeCard";
            this.themeCard.Size = new System.Drawing.Size(400, 96);
            this.themeCard.TabIndex = 0;
            // 
            // rAmethyst
            // 
            this.rAmethyst.Location = new System.Drawing.Point(14, 12);
            this.rAmethyst.Name = "rAmethyst";
            this.rAmethyst.Size = new System.Drawing.Size(122, 34);
            this.rAmethyst.Style = Lain.RadioStyle.Swatch;
            this.rAmethyst.SwatchColor = System.Drawing.Color.FromArgb(153, 102, 204);
            this.rAmethyst.TabIndex = 0;
            this.rAmethyst.Text = "Amethyst";
            this.rAmethyst.CheckedChanged += new System.EventHandler(this.zergtheme_CheckedChanged);
            // 
            // rAzurite
            // 
            this.rAzurite.Location = new System.Drawing.Point(140, 12);
            this.rAzurite.Name = "rAzurite";
            this.rAzurite.Size = new System.Drawing.Size(122, 34);
            this.rAzurite.Style = Lain.RadioStyle.Swatch;
            this.rAzurite.SwatchColor = System.Drawing.Color.FromArgb(0, 127, 255);
            this.rAzurite.TabIndex = 1;
            this.rAzurite.Text = "Azurite";
            this.rAzurite.CheckedChanged += new System.EventHandler(this.oceantheme_CheckedChanged);
            // 
            // rRuby
            // 
            this.rRuby.Location = new System.Drawing.Point(266, 12);
            this.rRuby.Name = "rRuby";
            this.rRuby.Size = new System.Drawing.Size(122, 34);
            this.rRuby.Style = Lain.RadioStyle.Swatch;
            this.rRuby.SwatchColor = System.Drawing.Color.FromArgb(205, 22, 39);
            this.rRuby.TabIndex = 2;
            this.rRuby.Text = "Ruby";
            this.rRuby.CheckedChanged += new System.EventHandler(this.magmatheme_CheckedChanged);
            // 
            // rJade
            // 
            this.rJade.Location = new System.Drawing.Point(14, 50);
            this.rJade.Name = "rJade";
            this.rJade.Size = new System.Drawing.Size(122, 34);
            this.rJade.Style = Lain.RadioStyle.Swatch;
            this.rJade.SwatchColor = System.Drawing.Color.FromArgb(70, 175, 105);
            this.rJade.TabIndex = 3;
            this.rJade.Text = "Jade";
            this.rJade.CheckedChanged += new System.EventHandler(this.limetheme_CheckedChanged);
            // 
            // rAmber
            // 
            this.rAmber.Location = new System.Drawing.Point(140, 50);
            this.rAmber.Name = "rAmber";
            this.rAmber.Size = new System.Drawing.Size(122, 34);
            this.rAmber.Style = Lain.RadioStyle.Swatch;
            this.rAmber.SwatchColor = System.Drawing.Color.FromArgb(195, 146, 0);
            this.rAmber.TabIndex = 4;
            this.rAmber.Text = "Amber";
            this.rAmber.CheckedChanged += new System.EventHandler(this.carameltheme_CheckedChanged);
            // 
            // rSilver
            // 
            this.rSilver.Location = new System.Drawing.Point(266, 50);
            this.rSilver.Name = "rSilver";
            this.rSilver.Size = new System.Drawing.Size(122, 34);
            this.rSilver.Style = Lain.RadioStyle.Swatch;
            this.rSilver.SwatchColor = System.Drawing.Color.Gray;
            this.rSilver.TabIndex = 5;
            this.rSilver.Text = "Silver";
            this.rSilver.CheckedChanged += new System.EventHandler(this.minimaltheme_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = Fluent.UiSemibold(10F);
            this.label2.Location = new System.Drawing.Point(29, 200);
            this.label2.Name = "label2";
            this.label2.Text = "Text size";
            // 
            // sizePanel
            // 
            this.sizePanel.Controls.Add(this.rSmall);
            this.sizePanel.Controls.Add(this.rNormal);
            this.sizePanel.Controls.Add(this.rLarge);
            this.sizePanel.Location = new System.Drawing.Point(30, 222);
            this.sizePanel.Name = "sizePanel";
            this.sizePanel.Size = new System.Drawing.Size(400, 42);
            this.sizePanel.TabIndex = 1;
            // 
            // rSmall
            // 
            this.rSmall.Location = new System.Drawing.Point(4, 4);
            this.rSmall.Name = "rSmall";
            this.rSmall.Size = new System.Drawing.Size(130, 34);
            this.rSmall.Style = Lain.RadioStyle.Segment;
            this.rSmall.TabIndex = 0;
            this.rSmall.Text = "Small";
            this.rSmall.CheckedChanged += new System.EventHandler(this.rSmall_CheckedChanged);
            // 
            // rNormal
            // 
            this.rNormal.Location = new System.Drawing.Point(135, 4);
            this.rNormal.Name = "rNormal";
            this.rNormal.Size = new System.Drawing.Size(130, 34);
            this.rNormal.Style = Lain.RadioStyle.Segment;
            this.rNormal.TabIndex = 1;
            this.rNormal.Text = "Normal";
            this.rNormal.CheckedChanged += new System.EventHandler(this.rNormal_CheckedChanged);
            // 
            // rLarge
            // 
            this.rLarge.Location = new System.Drawing.Point(266, 4);
            this.rLarge.Name = "rLarge";
            this.rLarge.Size = new System.Drawing.Size(130, 34);
            this.rLarge.Style = Lain.RadioStyle.Segment;
            this.rLarge.TabIndex = 2;
            this.rLarge.Text = "Large";
            this.rLarge.CheckedChanged += new System.EventHandler(this.rLarge_CheckedChanged);
            // 
            // lblSpacing
            // 
            this.lblSpacing.AutoSize = true;
            this.lblSpacing.Font = Fluent.UiSemibold(10F);
            this.lblSpacing.Location = new System.Drawing.Point(29, 280);
            this.lblSpacing.Name = "lblSpacing";
            this.lblSpacing.Text = "Row spacing";
            // 
            // spacingPanel
            // 
            this.spacingPanel.Controls.Add(this.rComfortable);
            this.spacingPanel.Controls.Add(this.rCompact);
            this.spacingPanel.Location = new System.Drawing.Point(30, 302);
            this.spacingPanel.Name = "spacingPanel";
            this.spacingPanel.Size = new System.Drawing.Size(400, 42);
            this.spacingPanel.TabIndex = 2;
            // 
            // rComfortable
            // 
            this.rComfortable.Location = new System.Drawing.Point(4, 4);
            this.rComfortable.Name = "rComfortable";
            this.rComfortable.Size = new System.Drawing.Size(195, 34);
            this.rComfortable.Style = Lain.RadioStyle.Segment;
            this.rComfortable.TabIndex = 0;
            this.rComfortable.Text = "Comfortable";
            this.rComfortable.CheckedChanged += new System.EventHandler(this.rSpacing_CheckedChanged);
            // 
            // rCompact
            // 
            this.rCompact.Location = new System.Drawing.Point(201, 4);
            this.rCompact.Name = "rCompact";
            this.rCompact.Size = new System.Drawing.Size(195, 34);
            this.rCompact.Style = Lain.RadioStyle.Segment;
            this.rCompact.TabIndex = 1;
            this.rCompact.Text = "Compact";
            this.rCompact.CheckedChanged += new System.EventHandler(this.rSpacing_CheckedChanged);
            // 
            // lblBehavior
            // 
            this.lblBehavior.AutoSize = true;
            this.lblBehavior.Font = Fluent.UiSemibold(10F);
            this.lblBehavior.Location = new System.Drawing.Point(29, 360);
            this.lblBehavior.Name = "lblBehavior";
            this.lblBehavior.Text = "Behavior";
            // 
            // lblSecurity
            // 
            this.lblSecurity.AutoSize = true;
            this.lblSecurity.Font = Fluent.UiSemibold(10F);
            this.lblSecurity.Location = new System.Drawing.Point(459, 66);
            this.lblSecurity.Name = "lblSecurity";
            this.lblSecurity.Text = "Privacy & security";
            this.lblSecurity.UseMnemonic = false;
            // 
            // securityCard
            // 
            this.securityCard.Controls.Add(this.chkHidePass);
            this.securityCard.Controls.Add(this.chkAlwaysPass);
            this.securityCard.Controls.Add(this.chkAutoLock);
            this.securityCard.Controls.Add(this.timerBox);
            this.securityCard.Controls.Add(this.lblMins);
            this.securityCard.Controls.Add(this.chkClearClip);
            this.securityCard.Controls.Add(this.clipBox);
            this.securityCard.Controls.Add(this.lblSecs);
            this.securityCard.Location = new System.Drawing.Point(460, 88);
            this.securityCard.Name = "securityCard";
            this.securityCard.Size = new System.Drawing.Size(400, 176);
            this.securityCard.TabIndex = 3;
            // 
            // lblStartup
            // 
            this.lblStartup.AutoSize = true;
            this.lblStartup.Font = Fluent.UiSemibold(10F);
            this.lblStartup.Location = new System.Drawing.Point(459, 280);
            this.lblStartup.Name = "lblStartup";
            this.lblStartup.Text = "Startup";
            // 
            // startupCard
            // 
            this.startupCard.Controls.Add(this.chkAutoStart);
            this.startupCard.Location = new System.Drawing.Point(460, 302);
            this.startupCard.Name = "startupCard";
            this.startupCard.Size = new System.Drawing.Size(400, 42);
            this.startupCard.TabIndex = 4;
            // 
            // behaviorCard
            // 
            this.behaviorCard.Controls.Add(this.chkAutoSizeColumns);
            this.behaviorCard.Controls.Add(this.chkShowCreated);
            this.behaviorCard.Controls.Add(this.chkShowModified);
            this.behaviorCard.Location = new System.Drawing.Point(30, 382);
            this.behaviorCard.Name = "behaviorCard";
            this.behaviorCard.Size = new System.Drawing.Size(830, 56);
            this.behaviorCard.TabIndex = 5;
            // 
            // lblGenerator
            // 
            this.lblGenerator.AutoSize = true;
            this.lblGenerator.Font = Fluent.UiSemibold(10F);
            this.lblGenerator.Location = new System.Drawing.Point(29, 454);
            this.lblGenerator.Name = "lblGenerator";
            this.lblGenerator.Text = "Password generator";
            // 
            // generatorCard
            // 
            this.generatorCard.Controls.Add(this.chkMinLength);
            this.generatorCard.Controls.Add(this.minLengthBox);
            this.generatorCard.Controls.Add(this.lblMinLength);
            this.generatorCard.Controls.Add(this.chkMinPerType);
            this.generatorCard.Controls.Add(this.minPerTypeBox);
            this.generatorCard.Controls.Add(this.lblMinPerType);
            this.generatorCard.Controls.Add(this.lblSymbols);
            this.generatorCard.Controls.Add(this.symbolsBox);
            this.generatorCard.Controls.Add(this.btnSymbolPresets);
            this.generatorCard.Location = new System.Drawing.Point(30, 476);
            this.generatorCard.Name = "generatorCard";
            this.generatorCard.Size = new System.Drawing.Size(830, 96);
            this.generatorCard.TabIndex = 6;
            // 
            // lblSymbols
            // 
            this.lblSymbols.Location = new System.Drawing.Point(16, 50);
            this.lblSymbols.Name = "lblSymbols";
            this.lblSymbols.Size = new System.Drawing.Size(150, 36);
            this.lblSymbols.Text = "Allowed symbols";
            this.lblSymbols.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // symbolsBox
            // 
            this.symbolsBox.Controls.Add(this.txtSymbols);
            this.symbolsBox.Location = new System.Drawing.Point(170, 53);
            this.symbolsBox.Name = "symbolsBox";
            this.symbolsBox.Size = new System.Drawing.Size(492, 30);
            this.symbolsBox.TabIndex = 6;
            // 
            // txtSymbols
            // 
            this.txtSymbols.MaxLength = 64;
            this.txtSymbols.Name = "txtSymbols";
            this.txtSymbols.Size = new System.Drawing.Size(472, 18);
            this.txtSymbols.TabIndex = 0;
            this.txtSymbols.TextChanged += new System.EventHandler(this.txtSymbols_TextChanged);
            this.txtSymbols.Leave += new System.EventHandler(this.txtSymbols_Leave);
            // 
            // btnSymbolPresets
            // 
            this.btnSymbolPresets.Glyph = "\uE70D";
            this.btnSymbolPresets.Location = new System.Drawing.Point(674, 50);
            this.btnSymbolPresets.Name = "btnSymbolPresets";
            this.btnSymbolPresets.Size = new System.Drawing.Size(140, 36);
            this.btnSymbolPresets.TabIndex = 7;
            this.btnSymbolPresets.Text = "Presets";
            this.btnSymbolPresets.Click += new System.EventHandler(this.btnSymbolPresets_Click);
            // 
            // chkHidePass
            // 
            this.chkHidePass.Location = new System.Drawing.Point(16, 10);
            this.chkHidePass.Name = "chkHidePass";
            this.chkHidePass.Size = new System.Drawing.Size(370, 36);
            this.chkHidePass.Style = Lain.CheckStyle.Switch;
            this.chkHidePass.TabIndex = 0;
            this.chkHidePass.Text = "Hide passwords in the list";
            this.chkHidePass.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // chkAlwaysPass
            // 
            this.chkAlwaysPass.Location = new System.Drawing.Point(16, 50);
            this.chkAlwaysPass.Name = "chkAlwaysPass";
            this.chkAlwaysPass.Size = new System.Drawing.Size(370, 36);
            this.chkAlwaysPass.Style = Lain.CheckStyle.Switch;
            this.chkAlwaysPass.TabIndex = 1;
            this.chkAlwaysPass.Text = "Always ask for the master password";
            this.chkAlwaysPass.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chkAutoLock
            // 
            this.chkAutoLock.Location = new System.Drawing.Point(16, 90);
            this.chkAutoLock.Name = "chkAutoLock";
            this.chkAutoLock.Size = new System.Drawing.Size(240, 36);
            this.chkAutoLock.Style = Lain.CheckStyle.Switch;
            this.chkAutoLock.TabIndex = 2;
            this.chkAutoLock.Text = "Lock automatically after";
            this.chkAutoLock.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // timerBox
            // 
            this.timerBox.Controls.Add(this.txtTimer);
            this.timerBox.Location = new System.Drawing.Point(262, 93);
            this.timerBox.Name = "timerBox";
            this.timerBox.Size = new System.Drawing.Size(56, 30);
            this.timerBox.TabIndex = 3;
            // 
            // txtTimer
            // 
            this.txtTimer.Enabled = false;
            this.txtTimer.MaxLength = 4;
            this.txtTimer.Name = "txtTimer";
            this.txtTimer.Size = new System.Drawing.Size(36, 18);
            this.txtTimer.TabIndex = 0;
            this.txtTimer.Text = "2";
            this.txtTimer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtTimer.TextChanged += new System.EventHandler(this.txtTimer_TextChanged);
            this.txtTimer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTimer_KeyPress);
            // 
            // lblMins
            // 
            this.lblMins.Location = new System.Drawing.Point(324, 90);
            this.lblMins.Name = "lblMins";
            this.lblMins.Size = new System.Drawing.Size(70, 36);
            this.lblMins.Text = "minutes";
            this.lblMins.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkClearClip
            // 
            this.chkClearClip.Location = new System.Drawing.Point(16, 130);
            this.chkClearClip.Name = "chkClearClip";
            this.chkClearClip.Size = new System.Drawing.Size(240, 36);
            this.chkClearClip.Style = Lain.CheckStyle.Switch;
            this.chkClearClip.TabIndex = 4;
            this.chkClearClip.Text = "Clear copied passwords after";
            this.chkClearClip.CheckedChanged += new System.EventHandler(this.chkClearClip_CheckedChanged);
            // 
            // clipBox
            // 
            this.clipBox.Controls.Add(this.txtClipSeconds);
            this.clipBox.Location = new System.Drawing.Point(262, 133);
            this.clipBox.Name = "clipBox";
            this.clipBox.Size = new System.Drawing.Size(56, 30);
            this.clipBox.TabIndex = 5;
            // 
            // txtClipSeconds
            // 
            this.txtClipSeconds.MaxLength = 3;
            this.txtClipSeconds.Name = "txtClipSeconds";
            this.txtClipSeconds.Size = new System.Drawing.Size(36, 18);
            this.txtClipSeconds.TabIndex = 0;
            this.txtClipSeconds.Text = "30";
            this.txtClipSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtClipSeconds.TextChanged += new System.EventHandler(this.txtClipSeconds_TextChanged);
            this.txtClipSeconds.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTimer_KeyPress);
            this.txtClipSeconds.Leave += new System.EventHandler(this.txtClipSeconds_Leave);
            // 
            // lblSecs
            // 
            this.lblSecs.Location = new System.Drawing.Point(324, 130);
            this.lblSecs.Name = "lblSecs";
            this.lblSecs.Size = new System.Drawing.Size(70, 36);
            this.lblSecs.Text = "seconds";
            this.lblSecs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkMinLength
            // 
            this.chkMinLength.Location = new System.Drawing.Point(16, 10);
            this.chkMinLength.Name = "chkMinLength";
            this.chkMinLength.Size = new System.Drawing.Size(240, 36);
            this.chkMinLength.Style = Lain.CheckStyle.Switch;
            this.chkMinLength.TabIndex = 0;
            this.chkMinLength.Text = "Generated length at least";
            this.chkMinLength.CheckedChanged += new System.EventHandler(this.chkGenerator_CheckedChanged);
            // 
            // minLengthBox
            // 
            this.minLengthBox.Controls.Add(this.txtMinLength);
            this.minLengthBox.Location = new System.Drawing.Point(262, 13);
            this.minLengthBox.Name = "minLengthBox";
            this.minLengthBox.Size = new System.Drawing.Size(56, 30);
            this.minLengthBox.TabIndex = 1;
            // 
            // txtMinLength
            // 
            this.txtMinLength.MaxLength = 3;
            this.txtMinLength.Name = "txtMinLength";
            this.txtMinLength.Size = new System.Drawing.Size(36, 18);
            this.txtMinLength.TabIndex = 0;
            this.txtMinLength.Text = "12";
            this.txtMinLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMinLength.TextChanged += new System.EventHandler(this.txtGenerator_TextChanged);
            this.txtMinLength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTimer_KeyPress);
            this.txtMinLength.Leave += new System.EventHandler(this.txtGenerator_Leave);
            // 
            // lblMinLength
            // 
            this.lblMinLength.Location = new System.Drawing.Point(324, 10);
            this.lblMinLength.Name = "lblMinLength";
            this.lblMinLength.Size = new System.Drawing.Size(70, 36);
            this.lblMinLength.Text = "characters";
            this.lblMinLength.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkMinPerType
            // 
            this.chkMinPerType.Location = new System.Drawing.Point(446, 10);
            this.chkMinPerType.Name = "chkMinPerType";
            this.chkMinPerType.Size = new System.Drawing.Size(240, 36);
            this.chkMinPerType.Style = Lain.CheckStyle.Switch;
            this.chkMinPerType.TabIndex = 3;
            this.chkMinPerType.Text = "Each character type at least";
            this.chkMinPerType.CheckedChanged += new System.EventHandler(this.chkGenerator_CheckedChanged);
            // 
            // minPerTypeBox
            // 
            this.minPerTypeBox.Controls.Add(this.txtMinPerType);
            this.minPerTypeBox.Location = new System.Drawing.Point(692, 13);
            this.minPerTypeBox.Name = "minPerTypeBox";
            this.minPerTypeBox.Size = new System.Drawing.Size(56, 30);
            this.minPerTypeBox.TabIndex = 4;
            // 
            // txtMinPerType
            // 
            this.txtMinPerType.MaxLength = 1;
            this.txtMinPerType.Name = "txtMinPerType";
            this.txtMinPerType.Size = new System.Drawing.Size(36, 18);
            this.txtMinPerType.TabIndex = 0;
            this.txtMinPerType.Text = "1";
            this.txtMinPerType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMinPerType.TextChanged += new System.EventHandler(this.txtGenerator_TextChanged);
            this.txtMinPerType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTimer_KeyPress);
            this.txtMinPerType.Leave += new System.EventHandler(this.txtGenerator_Leave);
            // 
            // lblMinPerType
            // 
            this.lblMinPerType.Location = new System.Drawing.Point(754, 10);
            this.lblMinPerType.Name = "lblMinPerType";
            this.lblMinPerType.Size = new System.Drawing.Size(70, 36);
            this.lblMinPerType.Text = "times";
            this.lblMinPerType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.Location = new System.Drawing.Point(16, 3);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(370, 36);
            this.chkAutoStart.Style = Lain.CheckStyle.Switch;
            this.chkAutoStart.TabIndex = 0;
            this.chkAutoStart.Text = "Start Lain with Windows";
            this.chkAutoStart.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // chkAutoSizeColumns
            // 
            this.chkAutoSizeColumns.Location = new System.Drawing.Point(16, 10);
            this.chkAutoSizeColumns.Name = "chkAutoSizeColumns";
            this.chkAutoSizeColumns.Size = new System.Drawing.Size(260, 36);
            this.chkAutoSizeColumns.Style = Lain.CheckStyle.Switch;
            this.chkAutoSizeColumns.TabIndex = 0;
            this.chkAutoSizeColumns.Text = "Fit columns to their content";
            this.chkAutoSizeColumns.CheckedChanged += new System.EventHandler(this.chkAutoSizeColumns_CheckedChanged);
            // 
            // chkShowCreated
            // 
            this.chkShowCreated.Location = new System.Drawing.Point(288, 10);
            this.chkShowCreated.Name = "chkShowCreated";
            this.chkShowCreated.Size = new System.Drawing.Size(260, 36);
            this.chkShowCreated.Style = Lain.CheckStyle.Switch;
            this.chkShowCreated.TabIndex = 1;
            this.chkShowCreated.Text = "Show the created date";
            this.chkShowCreated.CheckedChanged += new System.EventHandler(this.chkShowDates_CheckedChanged);
            // 
            // chkShowModified
            // 
            this.chkShowModified.Location = new System.Drawing.Point(560, 10);
            this.chkShowModified.Name = "chkShowModified";
            this.chkShowModified.Size = new System.Drawing.Size(256, 36);
            this.chkShowModified.Style = Lain.CheckStyle.Switch;
            this.chkShowModified.TabIndex = 2;
            this.chkShowModified.Text = "Show the modified date";
            this.chkShowModified.CheckedChanged += new System.EventHandler(this.chkShowDates_CheckedChanged);
            // 
            // footer
            // 
            this.footer.Controls.Add(this.btnOk);
            this.footer.Controls.Add(this.btnChangePassword);
            this.footer.Controls.Add(this.btnSecurityKeys);
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.Location = new System.Drawing.Point(0, 592);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(890, 66);
            this.footer.TabIndex = 7;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Location = new System.Drawing.Point(764, 15);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(96, 36);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Done";
            this.btnOk.Variant = Lain.ButtonVariant.Primary;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnChangePassword.Glyph = "\uE8D7";
            this.btnChangePassword.Location = new System.Drawing.Point(30, 15);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(150, 36);
            this.btnChangePassword.TabIndex = 1;
            this.btnChangePassword.Text = "Master password";
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // btnSecurityKeys
            // 
            this.btnSecurityKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSecurityKeys.Glyph = "\uEA18";
            this.btnSecurityKeys.Location = new System.Drawing.Point(188, 15);
            this.btnSecurityKeys.Name = "btnSecurityKeys";
            this.btnSecurityKeys.Size = new System.Drawing.Size(132, 36);
            this.btnSecurityKeys.TabIndex = 2;
            this.btnSecurityKeys.Text = "Security keys";
            this.btnSecurityKeys.Click += new System.EventHandler(this.btnSecurityKeys_Click);
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(890, 658);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblAppearance);
            this.Controls.Add(this.rSystem);
            this.Controls.Add(this.rLight);
            this.Controls.Add(this.rDark);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.themeCard);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sizePanel);
            this.Controls.Add(this.lblSpacing);
            this.Controls.Add(this.spacingPanel);
            this.Controls.Add(this.lblSecurity);
            this.Controls.Add(this.securityCard);
            this.Controls.Add(this.lblBehavior);
            this.Controls.Add(this.lblStartup);
            this.Controls.Add(this.startupCard);
            this.Controls.Add(this.behaviorCard);
            this.Controls.Add(this.lblGenerator);
            this.Controls.Add(this.generatorCard);
            this.Controls.Add(this.footer);
            this.DoubleBuffered = true;
            this.Font = Fluent.Ui(10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.themeCard.ResumeLayout(false);
            this.sizePanel.ResumeLayout(false);
            this.spacingPanel.ResumeLayout(false);
            this.securityCard.ResumeLayout(false);
            this.behaviorCard.ResumeLayout(false);
            this.startupCard.ResumeLayout(false);
            this.generatorCard.ResumeLayout(false);
            this.symbolsBox.ResumeLayout(false);
            this.symbolsBox.PerformLayout();
            this.timerBox.ResumeLayout(false);
            this.timerBox.PerformLayout();
            this.clipBox.ResumeLayout(false);
            this.clipBox.PerformLayout();
            this.minLengthBox.ResumeLayout(false);
            this.minLengthBox.PerformLayout();
            this.minPerTypeBox.ResumeLayout(false);
            this.minPerTypeBox.PerformLayout();
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Lain.ModernButton btnChangePassword;
        private Lain.ModernButton btnSecurityKeys;
        private Lain.FluentLabel lblTitle;
        private Lain.FluentLabel lblAppearance;
        private Lain.MoonRadio rSystem;
        private Lain.MoonRadio rLight;
        private Lain.MoonRadio rDark;
        private Lain.FluentLabel label27;
        private Lain.Card themeCard;
        private MoonRadio rAmethyst;
        private MoonRadio rAzurite;
        private MoonRadio rRuby;
        private MoonRadio rJade;
        private MoonRadio rAmber;
        private MoonRadio rSilver;
        private Lain.FluentLabel label2;
        private Lain.Card sizePanel;
        private MoonRadio rSmall;
        private MoonRadio rNormal;
        private MoonRadio rLarge;
        private Lain.FluentLabel lblBehavior;
        private Lain.Card behaviorCard;
        private Lain.FluentLabel lblSecurity;
        private Lain.Card securityCard;
        private MoonCheck chkAutoSizeColumns;
        private MoonCheck chkShowCreated;
        private MoonCheck chkShowModified;
        private Lain.FluentLabel lblSpacing;
        private Lain.Card spacingPanel;
        private MoonRadio rComfortable;
        private MoonRadio rCompact;
        private MoonCheck chkHidePass;
        private MoonCheck chkAlwaysPass;
        private MoonCheck chkAutoStart;
        private MoonCheck chkAutoLock;
        private Lain.InputBox timerBox;
        private MoonCheck chkMinLength;
        private Lain.InputBox minLengthBox;
        private Lain.FluentTextBox txtMinLength;
        private Lain.FluentLabel lblMinLength;
        private MoonCheck chkMinPerType;
        private Lain.InputBox minPerTypeBox;
        private Lain.FluentTextBox txtMinPerType;
        private Lain.FluentLabel lblMinPerType;
        private Lain.FluentTextBox txtTimer;
        private Lain.FluentLabel lblMins;
        private MoonCheck chkClearClip;
        private Lain.InputBox clipBox;
        private Lain.FluentTextBox txtClipSeconds;
        private Lain.FluentLabel lblSecs;
        private Lain.FluentLabel lblStartup;
        private Lain.Card startupCard;
        private Lain.FluentLabel lblGenerator;
        private Lain.Card generatorCard;
        private Lain.FluentLabel lblSymbols;
        private Lain.InputBox symbolsBox;
        private Lain.FluentTextBox txtSymbols;
        private Lain.ModernButton btnSymbolPresets;
        private Lain.FooterPanel footer;
        private Lain.ModernButton btnOk;
    }
}
