namespace Rawr
{
    partial class PaperDoll
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxEnforceGemRequirements = new System.Windows.Forms.CheckBox();
            this.comboBoxRegion = new System.Windows.Forms.ComboBox();
            this.comboBoxModel = new System.Windows.Forms.ComboBox();
            this.comboBoxClass = new System.Windows.Forms.ComboBox();
            this.comboBoxRace = new System.Windows.Forms.ComboBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxRealm = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.itemButtonBack = new Rawr.ItemButtonWithEnchant();
            this.itemButtonChest = new Rawr.ItemButtonWithEnchant();
            this.itemButtonFeet = new Rawr.ItemButtonWithEnchant();
            this.itemButtonFinger1 = new Rawr.ItemButtonWithEnchant();
            this.itemButtonFinger2 = new Rawr.ItemButtonWithEnchant();
            this.itemButtonHands = new Rawr.ItemButtonWithEnchant();
            this.itemButtonHead = new Rawr.ItemButtonWithEnchant();
            this.itemButtonRanged = new Rawr.ItemButtonWithEnchant();
            this.itemButtonLegs = new Rawr.ItemButtonWithEnchant();
            this.itemButtonNeck = new Rawr.ItemButton();
            this.itemButtonProjectileBag = new Rawr.ItemButton();
            this.itemButtonProjectile = new Rawr.ItemButton();
            this.itemButtonOffHand = new Rawr.ItemButtonWithEnchant();
            this.itemButtonShirt = new Rawr.ItemButton();
            this.itemButtonShoulders = new Rawr.ItemButtonWithEnchant();
            this.itemButtonTabard = new Rawr.ItemButton();
            this.itemButtonTrinket1 = new Rawr.ItemButton();
            this.itemButtonTrinket2 = new Rawr.ItemButton();
            this.itemButtonWaist = new Rawr.ItemButton();
            this.itemButtonMainHand = new Rawr.ItemButtonWithEnchant();
            this.itemButtonWrist = new Rawr.ItemButtonWithEnchant();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labResults = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxEnforceGemRequirements);
            this.groupBox3.Controls.Add(this.comboBoxRegion);
            this.groupBox3.Controls.Add(this.comboBoxModel);
            this.groupBox3.Controls.Add(this.comboBoxClass);
            this.groupBox3.Controls.Add(this.comboBoxRace);
            this.groupBox3.Controls.Add(this.textBoxName);
            this.groupBox3.Controls.Add(this.textBoxRealm);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.label25);
            this.groupBox3.Location = new System.Drawing.Point(91, 32);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(291, 112);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Character Definition";
            // 
            // checkBoxEnforceGemRequirements
            // 
            this.checkBoxEnforceGemRequirements.AutoSize = true;
            this.checkBoxEnforceGemRequirements.Enabled = false;
            this.checkBoxEnforceGemRequirements.Location = new System.Drawing.Point(6, 92);
            this.checkBoxEnforceGemRequirements.Name = "checkBoxEnforceGemRequirements";
            this.checkBoxEnforceGemRequirements.Size = new System.Drawing.Size(160, 17);
            this.checkBoxEnforceGemRequirements.TabIndex = 5;
            this.checkBoxEnforceGemRequirements.Text = "Enforce Gem Requirements*";
            this.checkBoxEnforceGemRequirements.UseVisualStyleBackColor = true;
            // 
            // comboBoxRegion
            // 
            this.comboBoxRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRegion.Enabled = false;
            this.comboBoxRegion.FormattingEnabled = true;
            this.comboBoxRegion.Items.AddRange(new object[] {
            "US",
            "EU",
            "KR",
            "TW",
            "CN"});
            this.comboBoxRegion.Location = new System.Drawing.Point(243, 27);
            this.comboBoxRegion.Name = "comboBoxRegion";
            this.comboBoxRegion.Size = new System.Drawing.Size(42, 21);
            this.comboBoxRegion.TabIndex = 4;
            // 
            // comboBoxModel
            // 
            this.comboBoxModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxModel.Enabled = false;
            this.comboBoxModel.FormattingEnabled = true;
            this.comboBoxModel.Location = new System.Drawing.Point(200, 65);
            this.comboBoxModel.Name = "comboBoxModel";
            this.comboBoxModel.Size = new System.Drawing.Size(85, 21);
            this.comboBoxModel.TabIndex = 4;
            // 
            // comboBoxClass
            // 
            this.comboBoxClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxClass.Enabled = false;
            this.comboBoxClass.FormattingEnabled = true;
            this.comboBoxClass.Items.AddRange(new object[] {
            "DeathKnight",
            "Druid",
            "Hunter",
            "Mage",
            "Paladin",
            "Priest",
            "Rogue",
            "Shaman",
            "Warlock",
            "Warrior"});
            this.comboBoxClass.Location = new System.Drawing.Point(103, 65);
            this.comboBoxClass.Name = "comboBoxClass";
            this.comboBoxClass.Size = new System.Drawing.Size(91, 21);
            this.comboBoxClass.TabIndex = 4;
            // 
            // comboBoxRace
            // 
            this.comboBoxRace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRace.Enabled = false;
            this.comboBoxRace.FormattingEnabled = true;
            this.comboBoxRace.Items.AddRange(new object[] {
            "Draenei",
            "Dwarf",
            "Gnome",
            "Human",
            "NightElf",
            "BloodElf",
            "Orc",
            "Tauren",
            "Troll",
            "Undead"});
            this.comboBoxRace.Location = new System.Drawing.Point(6, 65);
            this.comboBoxRace.Name = "comboBoxRace";
            this.comboBoxRace.Size = new System.Drawing.Size(91, 21);
            this.comboBoxRace.TabIndex = 4;
            // 
            // textBoxName
            // 
            this.textBoxName.Enabled = false;
            this.textBoxName.Location = new System.Drawing.Point(6, 27);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(135, 20);
            this.textBoxName.TabIndex = 3;
            // 
            // textBoxRealm
            // 
            this.textBoxRealm.Enabled = false;
            this.textBoxRealm.Location = new System.Drawing.Point(147, 27);
            this.textBoxRealm.Name = "textBoxRealm";
            this.textBoxRealm.Size = new System.Drawing.Size(90, 20);
            this.textBoxRealm.TabIndex = 3;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(6, 16);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(33, 12);
            this.label19.TabIndex = 1;
            this.label19.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(200, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Model:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(145, 16);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(35, 12);
            this.label21.TabIndex = 1;
            this.label21.Text = "Realm:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(103, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Class:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(241, 16);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(37, 12);
            this.label23.TabIndex = 1;
            this.label23.Text = "Region:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(6, 50);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(30, 12);
            this.label25.TabIndex = 1;
            this.label25.Text = "Race:";
            // 
            // itemButtonBack
            // 
            this.itemButtonBack.Character = null;
            this.itemButtonBack.CharacterSlot = Rawr.CharacterSlot.Back;
            this.itemButtonBack.FormItemSelection = null;
            this.itemButtonBack.ItemIcon = null;
            this.itemButtonBack.Location = new System.Drawing.Point(6, 245);
            this.itemButtonBack.Name = "itemButtonBack";
            this.itemButtonBack.SelectedItem = null;
            this.itemButtonBack.SelectedItemId = 0;
            this.itemButtonBack.Size = new System.Drawing.Size(70, 83);
            this.itemButtonBack.TabIndex = 20;
            this.itemButtonBack.UseVisualStyleBackColor = true;
            // 
            // itemButtonChest
            // 
            this.itemButtonChest.Character = null;
            this.itemButtonChest.CharacterSlot = Rawr.CharacterSlot.Chest;
            this.itemButtonChest.FormItemSelection = null;
            this.itemButtonChest.ItemIcon = null;
            this.itemButtonChest.Location = new System.Drawing.Point(6, 330);
            this.itemButtonChest.Name = "itemButtonChest";
            this.itemButtonChest.SelectedItem = null;
            this.itemButtonChest.SelectedItemId = 0;
            this.itemButtonChest.Size = new System.Drawing.Size(70, 83);
            this.itemButtonChest.TabIndex = 21;
            this.itemButtonChest.UseVisualStyleBackColor = true;
            // 
            // itemButtonFeet
            // 
            this.itemButtonFeet.Character = null;
            this.itemButtonFeet.CharacterSlot = Rawr.CharacterSlot.Feet;
            this.itemButtonFeet.FormItemSelection = null;
            this.itemButtonFeet.ItemIcon = null;
            this.itemButtonFeet.Location = new System.Drawing.Point(399, 277);
            this.itemButtonFeet.Name = "itemButtonFeet";
            this.itemButtonFeet.SelectedItem = null;
            this.itemButtonFeet.SelectedItemId = 0;
            this.itemButtonFeet.Size = new System.Drawing.Size(70, 83);
            this.itemButtonFeet.TabIndex = 19;
            this.itemButtonFeet.UseVisualStyleBackColor = true;
            // 
            // itemButtonFinger1
            // 
            this.itemButtonFinger1.Character = null;
            this.itemButtonFinger1.CharacterSlot = Rawr.CharacterSlot.Finger1;
            this.itemButtonFinger1.FormItemSelection = null;
            this.itemButtonFinger1.ItemIcon = null;
            this.itemButtonFinger1.Location = new System.Drawing.Point(399, 362);
            this.itemButtonFinger1.Name = "itemButtonFinger1";
            this.itemButtonFinger1.SelectedItem = null;
            this.itemButtonFinger1.SelectedItemId = 0;
            this.itemButtonFinger1.Size = new System.Drawing.Size(70, 83);
            this.itemButtonFinger1.TabIndex = 17;
            this.itemButtonFinger1.UseVisualStyleBackColor = true;
            // 
            // itemButtonFinger2
            // 
            this.itemButtonFinger2.Character = null;
            this.itemButtonFinger2.CharacterSlot = Rawr.CharacterSlot.Finger2;
            this.itemButtonFinger2.FormItemSelection = null;
            this.itemButtonFinger2.ItemIcon = null;
            this.itemButtonFinger2.Location = new System.Drawing.Point(399, 447);
            this.itemButtonFinger2.Name = "itemButtonFinger2";
            this.itemButtonFinger2.SelectedItem = null;
            this.itemButtonFinger2.SelectedItemId = 0;
            this.itemButtonFinger2.Size = new System.Drawing.Size(70, 83);
            this.itemButtonFinger2.TabIndex = 18;
            this.itemButtonFinger2.UseVisualStyleBackColor = true;
            // 
            // itemButtonHands
            // 
            this.itemButtonHands.Character = null;
            this.itemButtonHands.CharacterSlot = Rawr.CharacterSlot.Hands;
            this.itemButtonHands.FormItemSelection = null;
            this.itemButtonHands.ItemIcon = null;
            this.itemButtonHands.Location = new System.Drawing.Point(399, 3);
            this.itemButtonHands.Name = "itemButtonHands";
            this.itemButtonHands.SelectedItem = null;
            this.itemButtonHands.SelectedItemId = 0;
            this.itemButtonHands.Size = new System.Drawing.Size(70, 83);
            this.itemButtonHands.TabIndex = 25;
            this.itemButtonHands.UseVisualStyleBackColor = true;
            // 
            // itemButtonHead
            // 
            this.itemButtonHead.Character = null;
            this.itemButtonHead.CharacterSlot = Rawr.CharacterSlot.Head;
            this.itemButtonHead.FormItemSelection = null;
            this.itemButtonHead.ItemIcon = null;
            this.itemButtonHead.Location = new System.Drawing.Point(6, 3);
            this.itemButtonHead.Name = "itemButtonHead";
            this.itemButtonHead.SelectedItem = null;
            this.itemButtonHead.SelectedItemId = 0;
            this.itemButtonHead.Size = new System.Drawing.Size(70, 83);
            this.itemButtonHead.TabIndex = 26;
            this.itemButtonHead.UseVisualStyleBackColor = true;
            // 
            // itemButtonRanged
            // 
            this.itemButtonRanged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.itemButtonRanged.Character = null;
            this.itemButtonRanged.CharacterSlot = Rawr.CharacterSlot.Ranged;
            this.itemButtonRanged.FormItemSelection = null;
            this.itemButtonRanged.ItemIcon = null;
            this.itemButtonRanged.Location = new System.Drawing.Point(259, 593);
            this.itemButtonRanged.Name = "itemButtonRanged";
            this.itemButtonRanged.SelectedItem = null;
            this.itemButtonRanged.SelectedItemId = 0;
            this.itemButtonRanged.Size = new System.Drawing.Size(70, 83);
            this.itemButtonRanged.TabIndex = 22;
            this.itemButtonRanged.UseVisualStyleBackColor = true;
            // 
            // itemButtonLegs
            // 
            this.itemButtonLegs.Character = null;
            this.itemButtonLegs.CharacterSlot = Rawr.CharacterSlot.Legs;
            this.itemButtonLegs.FormItemSelection = null;
            this.itemButtonLegs.ItemIcon = null;
            this.itemButtonLegs.Location = new System.Drawing.Point(399, 192);
            this.itemButtonLegs.Name = "itemButtonLegs";
            this.itemButtonLegs.SelectedItem = null;
            this.itemButtonLegs.SelectedItemId = 0;
            this.itemButtonLegs.Size = new System.Drawing.Size(70, 83);
            this.itemButtonLegs.TabIndex = 23;
            this.itemButtonLegs.UseVisualStyleBackColor = true;
            // 
            // itemButtonNeck
            // 
            this.itemButtonNeck.Character = null;
            this.itemButtonNeck.CharacterSlot = Rawr.CharacterSlot.Neck;
            this.itemButtonNeck.FormItemSelection = null;
            this.itemButtonNeck.ItemIcon = null;
            this.itemButtonNeck.Location = new System.Drawing.Point(6, 88);
            this.itemButtonNeck.Name = "itemButtonNeck";
            this.itemButtonNeck.ReadOnly = false;
            this.itemButtonNeck.SelectedItem = null;
            this.itemButtonNeck.SelectedItemId = 0;
            this.itemButtonNeck.SelectedItemInstance = null;
            this.itemButtonNeck.Size = new System.Drawing.Size(70, 70);
            this.itemButtonNeck.TabIndex = 24;
            this.itemButtonNeck.Text = "Neck";
            this.itemButtonNeck.UseVisualStyleBackColor = true;
            // 
            // itemButtonProjectileBag
            // 
            this.itemButtonProjectileBag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.itemButtonProjectileBag.Character = null;
            this.itemButtonProjectileBag.CharacterSlot = Rawr.CharacterSlot.ProjectileBag;
            this.itemButtonProjectileBag.FormItemSelection = null;
            this.itemButtonProjectileBag.ItemIcon = null;
            this.itemButtonProjectileBag.Location = new System.Drawing.Point(347, 641);
            this.itemButtonProjectileBag.Name = "itemButtonProjectileBag";
            this.itemButtonProjectileBag.ReadOnly = false;
            this.itemButtonProjectileBag.SelectedItem = null;
            this.itemButtonProjectileBag.SelectedItemId = 0;
            this.itemButtonProjectileBag.SelectedItemInstance = null;
            this.itemButtonProjectileBag.Size = new System.Drawing.Size(35, 35);
            this.itemButtonProjectileBag.TabIndex = 16;
            this.itemButtonProjectileBag.Text = "ProjBag";
            this.itemButtonProjectileBag.UseVisualStyleBackColor = true;
            // 
            // itemButtonProjectile
            // 
            this.itemButtonProjectile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.itemButtonProjectile.Character = null;
            this.itemButtonProjectile.CharacterSlot = Rawr.CharacterSlot.Projectile;
            this.itemButtonProjectile.FormItemSelection = null;
            this.itemButtonProjectile.ItemIcon = null;
            this.itemButtonProjectile.Location = new System.Drawing.Point(347, 600);
            this.itemButtonProjectile.Name = "itemButtonProjectile";
            this.itemButtonProjectile.ReadOnly = false;
            this.itemButtonProjectile.SelectedItem = null;
            this.itemButtonProjectile.SelectedItemId = 0;
            this.itemButtonProjectile.SelectedItemInstance = null;
            this.itemButtonProjectile.Size = new System.Drawing.Size(35, 35);
            this.itemButtonProjectile.TabIndex = 9;
            this.itemButtonProjectile.Text = "Proj";
            this.itemButtonProjectile.UseVisualStyleBackColor = true;
            // 
            // itemButtonOffHand
            // 
            this.itemButtonOffHand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.itemButtonOffHand.Character = null;
            this.itemButtonOffHand.CharacterSlot = Rawr.CharacterSlot.OffHand;
            this.itemButtonOffHand.FormItemSelection = null;
            this.itemButtonOffHand.ItemIcon = null;
            this.itemButtonOffHand.Location = new System.Drawing.Point(172, 593);
            this.itemButtonOffHand.Name = "itemButtonOffHand";
            this.itemButtonOffHand.SelectedItem = null;
            this.itemButtonOffHand.SelectedItemId = 0;
            this.itemButtonOffHand.Size = new System.Drawing.Size(70, 83);
            this.itemButtonOffHand.TabIndex = 10;
            this.itemButtonOffHand.UseVisualStyleBackColor = true;
            // 
            // itemButtonShirt
            // 
            this.itemButtonShirt.Character = null;
            this.itemButtonShirt.CharacterSlot = Rawr.CharacterSlot.Shirt;
            this.itemButtonShirt.FormItemSelection = null;
            this.itemButtonShirt.ItemIcon = null;
            this.itemButtonShirt.Location = new System.Drawing.Point(6, 416);
            this.itemButtonShirt.Name = "itemButtonShirt";
            this.itemButtonShirt.ReadOnly = false;
            this.itemButtonShirt.SelectedItem = null;
            this.itemButtonShirt.SelectedItemId = 0;
            this.itemButtonShirt.SelectedItemInstance = null;
            this.itemButtonShirt.Size = new System.Drawing.Size(70, 70);
            this.itemButtonShirt.TabIndex = 8;
            this.itemButtonShirt.Text = "Shirt";
            this.itemButtonShirt.UseVisualStyleBackColor = true;
            // 
            // itemButtonShoulders
            // 
            this.itemButtonShoulders.Character = null;
            this.itemButtonShoulders.CharacterSlot = Rawr.CharacterSlot.Shoulders;
            this.itemButtonShoulders.FormItemSelection = null;
            this.itemButtonShoulders.ItemIcon = null;
            this.itemButtonShoulders.Location = new System.Drawing.Point(6, 160);
            this.itemButtonShoulders.Name = "itemButtonShoulders";
            this.itemButtonShoulders.SelectedItem = null;
            this.itemButtonShoulders.SelectedItemId = 0;
            this.itemButtonShoulders.Size = new System.Drawing.Size(70, 83);
            this.itemButtonShoulders.TabIndex = 6;
            this.itemButtonShoulders.UseVisualStyleBackColor = true;
            // 
            // itemButtonTabard
            // 
            this.itemButtonTabard.Character = null;
            this.itemButtonTabard.CharacterSlot = Rawr.CharacterSlot.Tabard;
            this.itemButtonTabard.FormItemSelection = null;
            this.itemButtonTabard.ItemIcon = null;
            this.itemButtonTabard.Location = new System.Drawing.Point(6, 487);
            this.itemButtonTabard.Name = "itemButtonTabard";
            this.itemButtonTabard.ReadOnly = false;
            this.itemButtonTabard.SelectedItem = null;
            this.itemButtonTabard.SelectedItemId = 0;
            this.itemButtonTabard.SelectedItemInstance = null;
            this.itemButtonTabard.Size = new System.Drawing.Size(70, 70);
            this.itemButtonTabard.TabIndex = 7;
            this.itemButtonTabard.Text = "Tabard";
            this.itemButtonTabard.UseVisualStyleBackColor = true;
            // 
            // itemButtonTrinket1
            // 
            this.itemButtonTrinket1.Character = null;
            this.itemButtonTrinket1.CharacterSlot = Rawr.CharacterSlot.Trinket1;
            this.itemButtonTrinket1.FormItemSelection = null;
            this.itemButtonTrinket1.ItemIcon = null;
            this.itemButtonTrinket1.Location = new System.Drawing.Point(399, 532);
            this.itemButtonTrinket1.Name = "itemButtonTrinket1";
            this.itemButtonTrinket1.ReadOnly = false;
            this.itemButtonTrinket1.SelectedItem = null;
            this.itemButtonTrinket1.SelectedItemId = 0;
            this.itemButtonTrinket1.SelectedItemInstance = null;
            this.itemButtonTrinket1.Size = new System.Drawing.Size(70, 70);
            this.itemButtonTrinket1.TabIndex = 14;
            this.itemButtonTrinket1.Text = "Trinket1";
            this.itemButtonTrinket1.UseVisualStyleBackColor = true;
            // 
            // itemButtonTrinket2
            // 
            this.itemButtonTrinket2.Character = null;
            this.itemButtonTrinket2.CharacterSlot = Rawr.CharacterSlot.Trinket2;
            this.itemButtonTrinket2.FormItemSelection = null;
            this.itemButtonTrinket2.ItemIcon = null;
            this.itemButtonTrinket2.Location = new System.Drawing.Point(399, 604);
            this.itemButtonTrinket2.Name = "itemButtonTrinket2";
            this.itemButtonTrinket2.ReadOnly = false;
            this.itemButtonTrinket2.SelectedItem = null;
            this.itemButtonTrinket2.SelectedItemId = 0;
            this.itemButtonTrinket2.SelectedItemInstance = null;
            this.itemButtonTrinket2.Size = new System.Drawing.Size(70, 70);
            this.itemButtonTrinket2.TabIndex = 15;
            this.itemButtonTrinket2.Text = "Trinket2";
            this.itemButtonTrinket2.UseVisualStyleBackColor = true;
            // 
            // itemButtonWaist
            // 
            this.itemButtonWaist.Character = null;
            this.itemButtonWaist.CharacterSlot = Rawr.CharacterSlot.Waist;
            this.itemButtonWaist.FormItemSelection = null;
            this.itemButtonWaist.ItemIcon = null;
            this.itemButtonWaist.Location = new System.Drawing.Point(399, 104);
            this.itemButtonWaist.Name = "itemButtonWaist";
            this.itemButtonWaist.ReadOnly = false;
            this.itemButtonWaist.SelectedItem = null;
            this.itemButtonWaist.SelectedItemId = 0;
            this.itemButtonWaist.SelectedItemInstance = null;
            this.itemButtonWaist.Size = new System.Drawing.Size(70, 70);
            this.itemButtonWaist.TabIndex = 13;
            this.itemButtonWaist.Text = "Waist";
            this.itemButtonWaist.UseVisualStyleBackColor = true;
            // 
            // itemButtonMainHand
            // 
            this.itemButtonMainHand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.itemButtonMainHand.Character = null;
            this.itemButtonMainHand.CharacterSlot = Rawr.CharacterSlot.MainHand;
            this.itemButtonMainHand.FormItemSelection = null;
            this.itemButtonMainHand.ItemIcon = null;
            this.itemButtonMainHand.Location = new System.Drawing.Point(85, 593);
            this.itemButtonMainHand.Name = "itemButtonMainHand";
            this.itemButtonMainHand.SelectedItem = null;
            this.itemButtonMainHand.SelectedItemId = 0;
            this.itemButtonMainHand.Size = new System.Drawing.Size(70, 83);
            this.itemButtonMainHand.TabIndex = 11;
            this.itemButtonMainHand.UseVisualStyleBackColor = true;
            // 
            // itemButtonWrist
            // 
            this.itemButtonWrist.Character = null;
            this.itemButtonWrist.CharacterSlot = Rawr.CharacterSlot.Wrist;
            this.itemButtonWrist.FormItemSelection = null;
            this.itemButtonWrist.ItemIcon = null;
            this.itemButtonWrist.Location = new System.Drawing.Point(6, 559);
            this.itemButtonWrist.Name = "itemButtonWrist";
            this.itemButtonWrist.SelectedItem = null;
            this.itemButtonWrist.SelectedItemId = 0;
            this.itemButtonWrist.Size = new System.Drawing.Size(70, 83);
            this.itemButtonWrist.TabIndex = 12;
            this.itemButtonWrist.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.labResults);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(91, 245);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(291, 87);
            this.flowLayoutPanel1.TabIndex = 30;
            // 
            // labResults
            // 
            this.labResults.AutoEllipsis = true;
            this.labResults.AutoSize = true;
            this.labResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labResults.Location = new System.Drawing.Point(3, 0);
            this.labResults.Name = "labResults";
            this.labResults.Size = new System.Drawing.Size(284, 24);
            this.labResults.TabIndex = 30;
            this.labResults.Text = "Score before optimizer : 1234";
            this.labResults.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PaperDoll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.itemButtonBack);
            this.Controls.Add(this.itemButtonChest);
            this.Controls.Add(this.itemButtonFeet);
            this.Controls.Add(this.itemButtonFinger1);
            this.Controls.Add(this.itemButtonFinger2);
            this.Controls.Add(this.itemButtonHands);
            this.Controls.Add(this.itemButtonHead);
            this.Controls.Add(this.itemButtonRanged);
            this.Controls.Add(this.itemButtonLegs);
            this.Controls.Add(this.itemButtonNeck);
            this.Controls.Add(this.itemButtonProjectileBag);
            this.Controls.Add(this.itemButtonProjectile);
            this.Controls.Add(this.itemButtonOffHand);
            this.Controls.Add(this.itemButtonShirt);
            this.Controls.Add(this.itemButtonShoulders);
            this.Controls.Add(this.itemButtonTabard);
            this.Controls.Add(this.itemButtonTrinket1);
            this.Controls.Add(this.itemButtonTrinket2);
            this.Controls.Add(this.itemButtonWaist);
            this.Controls.Add(this.itemButtonMainHand);
            this.Controls.Add(this.itemButtonWrist);
            this.Name = "PaperDoll";
            this.Size = new System.Drawing.Size(476, 678);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ItemButtonWithEnchant itemButtonBack;
        private ItemButtonWithEnchant itemButtonChest;
        private ItemButtonWithEnchant itemButtonFeet;
        private ItemButtonWithEnchant itemButtonFinger1;
        private ItemButtonWithEnchant itemButtonFinger2;
        private ItemButtonWithEnchant itemButtonHands;
        private ItemButtonWithEnchant itemButtonHead;
        private ItemButtonWithEnchant itemButtonRanged;
        private ItemButtonWithEnchant itemButtonLegs;
        private ItemButton itemButtonNeck;
        private ItemButton itemButtonProjectileBag;
        private ItemButton itemButtonProjectile;
        private ItemButtonWithEnchant itemButtonOffHand;
        private ItemButton itemButtonShirt;
        private ItemButtonWithEnchant itemButtonShoulders;
        private ItemButton itemButtonTabard;
        private ItemButton itemButtonTrinket1;
        private ItemButton itemButtonTrinket2;
        private ItemButton itemButtonWaist;
        private ItemButtonWithEnchant itemButtonMainHand;
        private ItemButtonWithEnchant itemButtonWrist;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxEnforceGemRequirements;
        private System.Windows.Forms.ComboBox comboBoxRegion;
        private System.Windows.Forms.ComboBox comboBoxModel;
        private System.Windows.Forms.ComboBox comboBoxClass;
        private System.Windows.Forms.ComboBox comboBoxRace;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxRealm;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labResults;
    }
}
