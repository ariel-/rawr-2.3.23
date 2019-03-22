using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{

    

    public partial class FormGemmingTemplates : Form, IFormItemSelectionProvider
    {

        private FormItemSelection _formItemSelection;

        //in future developments, the two lists could possibly be merged. I just
        //wanted to keep the class (gemming) the machinery for the class (listofgemmings)
        //apart from the interface (groupboxes) as I worked on them
        //iteratively.
        //private List<GemmingTemplate> cancelGemmings = new List<GemmingTemplate>(); // copy in case they cancel changes
        //private List<GemmingTemplate> _currentGemmings = new List<GemmingTemplate>();
        //private List<GroupBox> groupboxesForGemmings = new List<GroupBox>();
        private Character _currentCharacter = FormMain.Instance.Character;
		private List<string> _groups = new List<string>();
		private Panel _panelAddGemmingTemplate;

        public FormItemSelection FormItemSelection
        {
            get
            {
                if (_formItemSelection == null || _formItemSelection.IsDisposed)
                {
                    _formItemSelection = new FormItemSelection();
                    _formItemSelection.Character = FormMain.Instance.FormItemSelection.Character;
                    _formItemSelection.CurrentCalculations = FormMain.Instance.FormItemSelection.CurrentCalculations;
                }
                return _formItemSelection;
            }
        }

        public FormGemmingTemplates(){
        
            InitializeComponent();
			CreateGemmingTemplateControls();
            numericUpDownCountGemmingsShown.Value = Properties.GeneralSettings.Default.CountGemmingsShown;
            setLabelText(); // forces change on load as text not updated if value hasn't changed
		}

		public void CreateGemmingTemplateControls()
		{
			_groups.Clear();
			foreach (GemmingTemplate gemmingTemplate in _currentCharacter.CurrentGemmingTemplates)
            	if (!_groups.Contains(gemmingTemplate.Group)) 
					_groups.Add(gemmingTemplate.Group);
			if (!_groups.Contains("Custom")) _groups.Add("Custom");

			panelGemmingTemplates.Controls.Clear();
			foreach (string group in _groups)
			{
				Panel panelGroupHeader = new Panel();
				panelGroupHeader.Tag = group;
				panelGroupHeader.Height = 20;
				panelGroupHeader.Dock = DockStyle.Top;
				
				Button buttonExpand = new Button();
				buttonExpand.TextAlign = ContentAlignment.BottomCenter;
				buttonExpand.Font = new Font(buttonExpand.Font.FontFamily, 6.25f);
				buttonExpand.Width = buttonExpand.Height = 17;
				buttonExpand.Text = "-";
				buttonExpand.Left = buttonExpand.Top = 2;
				buttonExpand.UseVisualStyleBackColor = true;
				buttonExpand.Click += new EventHandler(buttonExpand_Click);
				panelGroupHeader.Controls.Add(buttonExpand);

				CheckBox checkBoxHeader = new CheckBox();
				checkBoxHeader.Font = new Font(checkBoxHeader.Font, FontStyle.Bold);
				checkBoxHeader.ThreeState = true;
				checkBoxHeader.Text = group;
				checkBoxHeader.CheckStateChanged += new EventHandler(checkBoxHeader_CheckStateChanged);
				checkBoxHeader.Left = 22;
				checkBoxHeader.Top = -1;
                checkBoxHeader.AutoSize = true;
                panelGroupHeader.Controls.Add(checkBoxHeader);

				panelGemmingTemplates.Controls.Add(panelGroupHeader);
				panelGroupHeader.BringToFront();

				bool allDisabled = group != "Custom";
                foreach (GemmingTemplate gemmingTemplate in _currentCharacter.CurrentGemmingTemplates)
				{
					if (gemmingTemplate.Group == group)
					{
						allDisabled = allDisabled && !gemmingTemplate.Enabled;
						GemmingTemplateControl gemmingTemplateControl = new GemmingTemplateControl();
						gemmingTemplateControl.Dock = DockStyle.Top;
						gemmingTemplateControl.GemmingTemplate = gemmingTemplate;
						gemmingTemplateControl.GemmingTemplateEnabledChanged += new EventHandler(gemmingTemplateControl_GemmingTemplateEnabledChanged);
						gemmingTemplateControl.DeleteClicked += new EventHandler(gemmingTemplateControl_DeleteClicked);
						panelGemmingTemplates.Controls.Add(gemmingTemplateControl);
						gemmingTemplateControl.BringToFront();
					}
				}
				if (allDisabled) buttonExpand.Text = "+";
			}
            foreach (GemmingTemplate gemmingTemplate in _currentCharacter.CustomGemmingTemplates)
            {
                if (gemmingTemplate.Model == _currentCharacter.CurrentModel)
                {
                    GemmingTemplateControl gemmingTemplateControl = new GemmingTemplateControl();
                    gemmingTemplateControl.Dock = DockStyle.Top;
                    gemmingTemplateControl.GemmingTemplate = gemmingTemplate;
                    gemmingTemplateControl.GemmingTemplateEnabledChanged += new EventHandler(gemmingTemplateControl_GemmingTemplateEnabledChanged);
                    gemmingTemplateControl.DeleteClicked += new EventHandler(gemmingTemplateControl_DeleteClicked);
                    panelGemmingTemplates.Controls.Add(gemmingTemplateControl);
                    gemmingTemplateControl.BringToFront();
                }
            }

			_panelAddGemmingTemplate = new Panel();
			_panelAddGemmingTemplate.Height = 50;
			_panelAddGemmingTemplate.Dock = DockStyle.Top;

			Button buttonAddGemmingTemplate = new Button();
			buttonAddGemmingTemplate.Text = "Add";
			buttonAddGemmingTemplate.Location = new System.Drawing.Point(3, 14);
			buttonAddGemmingTemplate.Size = new System.Drawing.Size(46, 23);
			buttonAddGemmingTemplate.UseVisualStyleBackColor = true;
			buttonAddGemmingTemplate.Click += new EventHandler(buttonAddGemmingTemplate_Click);
			_panelAddGemmingTemplate.Controls.Add(buttonAddGemmingTemplate);

			panelGemmingTemplates.Controls.Add(_panelAddGemmingTemplate);
			_panelAddGemmingTemplate.BringToFront();

			UpdateCheckStatesAndVisibles();
        }

		public void UpdateCheckStatesAndVisibles()
		{
			SuspendLayout();
			foreach (string group in _groups)
			{
				Panel panelHeader = null;
				bool hasEnabled = false;
				bool hasDisabled = false;
				List<GemmingTemplateControl> gemmingTemplateControls = new List<GemmingTemplateControl>();
				foreach (Control control in panelGemmingTemplates.Controls)
				{
					if ((string)control.Tag == group)
						panelHeader = control as Panel;
					else if (control is GemmingTemplateControl)
					{
						GemmingTemplateControl gemmingTemplateControl = control as GemmingTemplateControl;
						if (gemmingTemplateControl.GemmingTemplate.Group == group)
						{
							gemmingTemplateControls.Add(gemmingTemplateControl);
						}
					}
				}
				foreach (GemmingTemplateControl gemmingTemplateControl in gemmingTemplateControls)
				{
					if (gemmingTemplateControl.GemmingTemplateEnabled) hasEnabled = true;
					else hasDisabled = true;
					gemmingTemplateControl.Visible = panelHeader.Controls[0].Text == "-";
				}
				CheckBox checkBoxHeader = panelHeader.Controls[1] as CheckBox;
				checkBoxHeader.CheckState = hasEnabled && hasDisabled ? CheckState.Indeterminate : 
					(hasEnabled ? CheckState.Checked : CheckState.Unchecked);
				if (group == "Custom") _panelAddGemmingTemplate.Visible = panelHeader.Controls[0].Text == "-";
			}
			ResumeLayout();
		}

		void buttonAddGemmingTemplate_Click(object sender, EventArgs e)
		{
			SuspendLayout();
			GemmingTemplate gemmingTemplate = new GemmingTemplate() { Group = "Custom", Enabled = true, Model = Calculations.Instance.Name };
            _currentCharacter.CustomGemmingTemplates.Add(gemmingTemplate);

			GemmingTemplateControl gemmingTemplateControl = new GemmingTemplateControl();
			gemmingTemplateControl.Dock = DockStyle.Top;
			gemmingTemplateControl.GemmingTemplate = gemmingTemplate;
			gemmingTemplateControl.GemmingTemplateEnabledChanged += new EventHandler(gemmingTemplateControl_GemmingTemplateEnabledChanged);
			gemmingTemplateControl.DeleteClicked += new EventHandler(gemmingTemplateControl_DeleteClicked);
			panelGemmingTemplates.Controls.Add(gemmingTemplateControl);
			gemmingTemplateControl.BringToFront();
			_panelAddGemmingTemplate.BringToFront();
			ResumeLayout();
		}

		void gemmingTemplateControl_GemmingTemplateEnabledChanged(object sender, EventArgs e)
		{
			UpdateCheckStatesAndVisibles();
		}

		void gemmingTemplateControl_DeleteClicked(object sender, EventArgs e)
		{
			GemmingTemplateControl gemmingTemplateControl = (sender as GemmingTemplateControl);
            _currentCharacter.CustomGemmingTemplates.Remove(gemmingTemplateControl.GemmingTemplate);
			panelGemmingTemplates.Controls.Remove(gemmingTemplateControl);
		}

		void checkBoxHeader_CheckStateChanged(object sender, EventArgs e)
		{
			CheckBox checkBoxHeader = sender as CheckBox;
			Panel panelHeader = checkBoxHeader.Parent as Panel;
			if (checkBoxHeader.CheckState != CheckState.Indeterminate)
			{
				foreach (Control control in panelGemmingTemplates.Controls)
				{
					if (control is GemmingTemplateControl &&
						(control as GemmingTemplateControl).GemmingTemplate.Group == (string)panelHeader.Tag)
					{
						(control as GemmingTemplateControl).GemmingTemplateEnabled = 
							checkBoxHeader.CheckState == CheckState.Checked ? true : false;
					}
				}
			}
		}

		void buttonExpand_Click(object sender, EventArgs e)
		{
			Button buttonExpand = sender as Button;
			buttonExpand.Text = buttonExpand.Text == "-" ? "+" : "-";
			UpdateCheckStatesAndVisibles();
		}

		private void buttonOK_Click(object sender, EventArgs e)
        {
			Properties.GeneralSettings.Default.CountGemmingsShown = (int)numericUpDownCountGemmingsShown.Value;
			Properties.GeneralSettings.Default.Save();
			ItemCache.OnItemsChanged();
        }

		private void numericUpDownCountGemmingsShown_ValueChanged(object sender, EventArgs e)
		{
            setLabelText();
		}

        private void setLabelText()
        {
            labelCountGemmingsShownDescription.Text = string.Format(labelCountGemmingsShownDescription.Tag.ToString(), (int)numericUpDownCountGemmingsShown.Value);
        }

        /* Old Gemming System
        private Button removeButton(GroupBox parentBox){
        
            //creates a button in the groupboxes that is used to 
            //remove the associated gemming

            Button newButton = new Button();
            newButton.Height = 19;
            newButton.Width = 57;
            newButton.Text = "Remove";
            newButton.Tag = (int)parentBox.Tag;
            newButton.BackColor = Color.FromKnownColor(KnownColor.LightGray);
            newButton.Click += new System.EventHandler(this.buttonRemoveGemming_Click);
            newButton.Location = new Point(8 + (63 * parentBox.Controls.Count) + 8, 34);

            return newButton;
        }

        private Rawr.ItemButton gemButton(GroupBox parentBox, KnownColor colorReq){
        

            //creates an ItemButton for use in groupbox gemmings

            ItemButton newButton = new ItemButton();

            newButton.Height = 57;
            newButton.Width = 57;
            newButton.Tag = (int)parentBox.Tag;
            newButton.BackColor = Color.FromKnownColor(colorReq);
            newButton.Leave += new System.EventHandler(this.buttonGem_Click);
            newButton.Location = new Point(8 + (63 * parentBox.Controls.Count), 17);

            if (colorReq.Equals((KnownColor)KnownColor.Gray))
            {            
                newButton.CharacterSlot = CharacterSlot.Metas;
                newButton.Text = "Meta";
            }
            else if (colorReq.Equals((KnownColor)KnownColor.Silver))
            {
                newButton.CharacterSlot = CharacterSlot.Gems;
                newButton.Text = "Prismatic";
            }
            else
            {            
                newButton.CharacterSlot = CharacterSlot.Gems;
                newButton.Text = colorReq.ToString();            
            }

            return newButton;

        }
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        //this is going to be used with a list of groupboxes that generate the
        //buttons used in the interface.
        private void cycleGroups(){
        
            foreach (GroupBox box in groupboxesForGemmings)
                updateButtons(box);            
        }

        private void updateButtons(GroupBox incoming)
        {
            //sifts through the itemButtons in a particular groupbox and
            //updates the images in them to reflect the selectedItem 
            //in their associated gemming

            ItemButton iButton;
            int incomingTag = (int)incoming.Tag;
            
            if (incomingTag <= listOfGemmings.Count){
            
                foreach (Control tempControl in (incoming.Controls)) {
               
                    if (tempControl is ItemButton){
                    
                        iButton = (ItemButton)tempControl;

                        switch (iButton.BackColor.ToKnownColor()){
                        

                            case (KnownColor.Red):
                                iButton.SelectedItem = listOfGemmings[incomingTag].RedGem;
                                break;
                            case (KnownColor.Blue):
                                iButton.SelectedItem = listOfGemmings[incomingTag].BlueGem;
                                break;
                            case (KnownColor.Yellow):
                                iButton.SelectedItem = listOfGemmings[incomingTag].YellowGem;
                                break;
                            case (KnownColor.Gray):
                                iButton.SelectedItem = listOfGemmings[incomingTag].MetaGem;
                                break;
                            case (KnownColor.Silver):
                                iButton.SelectedItem = listOfGemmings[incomingTag].PrismaticGem;
                                break;
                            default:
                                throw new IndexOutOfRangeException("rawr.itembutton.backcolor is used for parsing buttons. make sure they're set correctly");
                                

                        }
                        
                        if (iButton.SelectedItem == null){
                        
                            if (iButton.BackColor.ToKnownColor().Equals(KnownColor.Gray))
                                iButton.Text = "Meta";
                            else if (iButton.BackColor.ToKnownColor().Equals(KnownColor.Silver))
                                iButton.Text = "Prismatic";
                            else
                                iButton.Text = iButton.BackColor.ToKnownColor().ToString();
                        }
                    }
                }
            }
        }

        
        

        private GroupBox createGroupBox(){
        
            //adds groupbox and associated buttons
            
            GroupBox newGroupBox = new GroupBox();

            newGroupBox.Parent = this;
            newGroupBox.Location = new Point(8, (84 * groupboxesForGemmings.Count) + 43);
            newGroupBox.Width = 397;
            newGroupBox.Height = 80;
            newGroupBox.Tag = (int)(groupboxesForGemmings.Count);
            newGroupBox.Text = "Gemming #" + ((int)newGroupBox.Tag + 1).ToString();
            newGroupBox.Visible = true;

            newGroupBox.Controls.Add(gemButton(newGroupBox, (KnownColor.Red)));
            newGroupBox.Controls.Add(gemButton(newGroupBox, (KnownColor.Yellow)));
            newGroupBox.Controls.Add(gemButton(newGroupBox, (KnownColor.Blue)));
            newGroupBox.Controls.Add(gemButton(newGroupBox, (KnownColor.Silver)));
            newGroupBox.Controls.Add(gemButton(newGroupBox, (KnownColor.Gray)));

            newGroupBox.Controls.Add(removeButton(newGroupBox));

            return newGroupBox;

        }

        private void buttonAddGemming_Click(object sender, EventArgs e) {
       
            //adds a slot for a new gemming, as well as the groupbox interface

            if (listOfGemmings.Count < 10)
            {            
                groupboxesForGemmings.Add(createGroupBox());
                listOfGemmings.Add(new GemmingTemplate());
                adjustform();           
            }
            else
                MessageBox.Show("You may have a maximum of 10 gemmings", "10 Gemmings Allowed");
        }

		//private void deleteDuplicateGemmings()
		//{
		//    List<Gemming> gemList = new List<Gemming>();
		//    List<int> hash = new List<int>();
		//    foreach (Gemming gem in listOfGemmings){
            
		//        if (!hash.Contains(gem.GetHashCode())){
                   
		//            gemList.Add(gem);
		//            hash.Add(gem.GetHashCode());
               
		//        }

		//        else{
               
		//            groupboxesForGemmings[groupboxesForGemmings.Count - 1].Dispose();
		//            groupboxesForGemmings.RemoveAt(groupboxesForGemmings.Count - 1);
              
		//        }

		//    }

		//    listOfGemmings.Clear();
		//    listOfGemmings = new List<Gemming>();

		//    foreach (Gemming popGem in gemList)
		//        listOfGemmings.Add(popGem);
            
		//    cycleGroups();
		//}

        private void adjustform(){
        
            //shifts form height

            if (listOfGemmings.Count == 0)
                this.Height = 170;
            else
                this.Height = (170 + (86 * (listOfGemmings.Count - 1)));
        }

        private void buttonRemoveGemming_Click(object sender, EventArgs e){
        
            //removes the gemming associated with the groupbox the button
            //that was pressed, as well as the last groupbox in groupboxesForGemmings
            //gems are cycled down to fill the appropriate slot.

            //I'd like to note I hate list -> array -> list indexing...

            int value = int.Parse(((Control)sender).Tag.ToString());
            int shifts = listOfGemmings.Count - (value + 1);

            if (shifts != 0)

                for (int i = 0; i < shifts; i++)
                {
                    listOfGemmings[value + (i)].RedGem = listOfGemmings[value + i + 1].RedGem;
                    listOfGemmings[value + (i)].BlueGem = listOfGemmings[value + i + 1].BlueGem;
                    listOfGemmings[value + (i)].YellowGem = listOfGemmings[value + i + 1].YellowGem;
                    listOfGemmings[value + (i)].PrismaticGem = listOfGemmings[value + i + 1].PrismaticGem;
                    listOfGemmings[value + (i)].MetaGem = listOfGemmings[value + i + 1].MetaGem;
                }
                            
            if (listOfGemmings.Count >= 1){
            
                listOfGemmings.RemoveAt(listOfGemmings.Count - 1);
                groupboxesForGemmings[groupboxesForGemmings.Count - 1].Dispose();
                groupboxesForGemmings.RemoveAt(groupboxesForGemmings.Count - 1);
            }

            cycleGroups();
            adjustform();

        }


        private void buttonGem_Click(object sender, EventArgs e){
        

            //this needs to be renamed, it's tied to the Leave event, as most of the
            //mouse events were already tied into Rawr.Itembox and I wanted to keep
            //this form reasonably isolated. 

            //It finds the button that sent the event, tracks it back to a 
            //groupbox via a tag, and then parses the button by it's BackColor

            ItemButton senderButton = (Rawr.ItemButton)sender;
            int intRowNumber = (int)senderButton.Tag;
            KnownColor currColor = senderButton.BackColor.ToKnownColor();

            switch (currColor){
            
                case (KnownColor.Red):
                    listOfGemmings[intRowNumber].RedGem = senderButton.SelectedItem;
                    break;
                case (KnownColor.Blue):
                    listOfGemmings[intRowNumber].BlueGem = senderButton.SelectedItem;
                    break;
                case (KnownColor.Yellow):
                    listOfGemmings[intRowNumber].YellowGem = senderButton.SelectedItem;
                    break;
                case (KnownColor.Gray):
                    listOfGemmings[intRowNumber].MetaGem = senderButton.SelectedItem;
                    break;
                case (KnownColor.Silver):
                    listOfGemmings[intRowNumber].PrismaticGem = senderButton.SelectedItem;
                    break;
                default:
                    throw new IndexOutOfRangeException("the name used for the gems is used in parsing their colors. Please follow the existing convention.");
                   
            }

        }

    }

//    public class Gemming
//    {
       
//        private Item _gemRed;
//        private Item _gemYellow;
//        private Item _gemBlue;
//        private Item _gemMeta;
        

//        public Gemming()
//        {

//        }

//        private bool allNull() {
       
//            if ((_gemRed == null) && (_gemYellow == null) && (_gemBlue == null) && (gemMeta == null))
//                return true;
//            else
//                return false;
//            }

//        public override int GetHashCode()
//        {
//            int a = 0, b = 0, c = 0, d = 0;

//            if (gemRed != null)
//                a = gemRed.Id;
           
//            if (gemYellow != null)
//                b = 2 * gemYellow.Id;
           
//            if (gemBlue != null)
//                c = 3 * gemBlue.Id;
            
//            if (gemMeta != null)
//                d = 4 * gemMeta.Id;

//            return (a + b + c + d);
//        }

//        public Gemming(Item Red, Item Yellow, Item Blue, Item Meta){
        
//            _gemRed = Red;
//            _gemYellow = Yellow;
//            _gemBlue = Blue;
//            _gemMeta = Meta;
//        }

//        public Item gemIt(Item itemToBeGemmed){
        
//            int[] gemID = { 0, 0, 0 };

//            if (itemToBeGemmed.Sockets.Color1 != ItemSlot.None)
//            {

//                switch (itemToBeGemmed.Sockets.Color1)
//                {

//                    case ItemSlot.Red:
//                        if (gemRed == null)
//                            gemID[0] = -1;
//                        else
//                            gemID[0] = gemRed.Id;
//                        break;

//                    case ItemSlot.Blue:
//                        if (gemBlue == null)
//                            gemID[0] = -1;
//                        else
//                            gemID[0] = gemBlue.Id;
//                        break;

//                    case ItemSlot.Yellow:
//                        if (gemYellow == null)
//                            gemID[0] = -1;
//                        else
//                            gemID[0] = gemYellow.Id;
//                        break;

//                    case ItemSlot.Meta:
//                        if (gemMeta == null)
//                            gemID[0] = -1;
//                        else
//                            gemID[0] = gemMeta.Id;
//                        break;
//                }
//            }
//            else
//                gemID[0] = -1;
            
//            if (itemToBeGemmed.Sockets.Color2 != ItemSlot.None) {
           
//                switch (itemToBeGemmed.Sockets.Color2){
                
//                    case ItemSlot.Red:
//                        if (gemRed == null)
//                            gemID[1] = -1;
//                        else
//                            gemID[1] = gemRed.Id;
//                        break;

//                    case ItemSlot.Blue:
//                        if (gemBlue == null)
//                            gemID[1] = -1;
//                        else
//                            gemID[1] = gemBlue.Id;
//                        break;

//                    case ItemSlot.Yellow:
//                        if (gemYellow == null)
//                            gemID[1] = -1;
//                        else
//                            gemID[1] = gemYellow.Id;
//                        break;

//                    case ItemSlot.Meta:
//                        if (gemMeta == null)
//                            gemID[1] = -1;
//                        else
//                            gemID[1] = gemMeta.Id;
//                        break;
//                }

               
//            }
//             else
//                gemID[1] = -1;
//                if (itemToBeGemmed.Sockets.Color3 != ItemSlot.None){
            
//                switch (itemToBeGemmed.Sockets.Color3){
                
//                    case ItemSlot.Red:
//                        if (gemRed == null)
//                            gemID[2] = -1;
//                        else
//                            gemID[2] = gemRed.Id;
//                        break;

//                    case ItemSlot.Blue:
//                        if (gemBlue == null)
//                            gemID[2] = -1;
//                        else
//                            gemID[2] = gemBlue.Id;
//                        break;

//                    case ItemSlot.Yellow:
//                        if (gemYellow == null)
//                            gemID[2] = -1;
//                        else
//                            gemID[2] = gemYellow.Id;
//                        break;

//                    case ItemSlot.Meta:
//                        if (gemMeta == null)
//                            gemID[2] = -1;
//                        else
//                            gemID[2] = gemMeta.Id;
//                        break;
//                }

                
//            }
//else
//                gemID[2]=-1;

//                if ((gemID[0] == -1 && gemID[1] == -1 && gemID[2] == -1)&&(!allNull()))
//                    return null;
//                else
//                {
//                    if (gemID[0] == -1) gemID[0]= 0;
//                    if (gemID[1] == -1) gemID[1] = 0;
//                    if (gemID[2] == -1) gemID[2] =  0;
//                }
//            Item copy = new Item(itemToBeGemmed.Name, itemToBeGemmed.Quality, itemToBeGemmed.Type, itemToBeGemmed.Id, itemToBeGemmed.IconPath, itemToBeGemmed.Slot, itemToBeGemmed.SetName, 
//                                 itemToBeGemmed.Unique, itemToBeGemmed.Stats.Clone(), itemToBeGemmed.Sockets.Clone(), gemID[0], gemID[1], gemID[2],
//                                 itemToBeGemmed.MinDamage, itemToBeGemmed.MaxDamage, itemToBeGemmed.DamageType, itemToBeGemmed.Speed, itemToBeGemmed.RequiredClasses);
            
//            return copy;
//            return null;
//        }

//        public void copyGemming(Gemming sourceGemming){
        
//            this._gemRed = sourceGemming.gemRed;
//            this._gemYellow = sourceGemming.gemYellow;
//            this._gemBlue = sourceGemming.gemBlue;
//            this._gemMeta = sourceGemming.gemMeta;
//        }

//        public string gemmingID(Item item){
        
//            string gem1ID = "0";
//            string gem2ID = "0";
//            string gem3ID = "0";

//            if (item.Sockets.Color1 != ItemSlot.None){
            
//                switch (item.Sockets.Color1){
                
//                    case ItemSlot.Red:
//                        if (this.gemRed == null)
//                            gem1ID = "0";
//                        else
//                            gem1ID = this.gemRed.Id.ToString();
//                        break;

//                    case ItemSlot.Blue:
//                        if (this.gemBlue == null)
//                            gem1ID = "0";
//                        else
//                            gem1ID = this.gemBlue.Id.ToString();
//                        break;

//                    case ItemSlot.Yellow:
//                        if (this.gemYellow == null)
//                            gem1ID = "0";
//                        else
//                            gem1ID = this.gemYellow.Id.ToString();
//                        break;

//                    case ItemSlot.Meta:
//                        if (this.gemMeta == null)
//                            gem1ID = "0";
//                        else
//                            gem1ID = this.gemMeta.Id.ToString();
//                        break;
//                }
//            }
//            if (item.Sockets.Color2 != ItemSlot.None){
            
//                switch (item.Sockets.Color2){
                
//                    case ItemSlot.Red:
//                        if (this.gemRed == null)
//                            gem2ID = "0";
//                        else
//                            gem2ID = this.gemRed.Id.ToString();
//                        break;

//                    case ItemSlot.Blue:
//                        if (this.gemBlue == null)
//                            gem2ID = "0";
//                        else
//                            gem2ID = this.gemBlue.Id.ToString();
//                        break;

//                    case ItemSlot.Yellow:
//                        if (this.gemYellow == null)
//                            gem2ID = "0";
//                        else
//                            gem2ID = this.gemYellow.Id.ToString();
//                        break;

//                    case ItemSlot.Meta:
//                        if (this.gemMeta == null)
//                            gem2ID = "0";
//                        else
//                            gem2ID = this.gemMeta.Id.ToString();
//                        break;
//                }
//            }
//            if (item.Sockets.Color3 != ItemSlot.None){
            
//                switch (item.Sockets.Color3){
                
//                    case ItemSlot.Red:
//                        if (this.gemRed == null)
//                            gem3ID = "0";
//                        else
//                            gem3ID = this.gemRed.Id.ToString();
//                        break;

//                    case ItemSlot.Blue:
//                        if (this.gemBlue == null)
//                            gem3ID = "0";
//                        else
//                            gem3ID = this.gemBlue.Id.ToString();
//                        break;

//                    case ItemSlot.Yellow:
//                        if (this.gemYellow == null)
//                            gem3ID = "0";
//                        else
//                            gem3ID = this.gemYellow.Id.ToString();
//                        break;

//                    case ItemSlot.Meta:
//                        if (this.gemMeta == null)
//                            gem3ID = "0";
//                        else
//                            gem3ID = this.gemMeta.Id.ToString();
//                        break;
//                }
//            }

//            return "." + gem1ID + "." + gem2ID + "." + gem3ID;
//        }

//        public Item gemRed {
       
//            get
//            {
//                return _gemRed;
//            }
//            set
//            {
//                _gemRed = value;
//            }
//        }
        
//        public Item gemYellow{
        
//            get
//            {
//                return _gemYellow;
//            }
//            set
//            {
//                _gemYellow = value;
//            }
//        }
        
//        public Item gemBlue{
        
//            get
//            {
//                return _gemBlue;
//            }
//            set
//            {
//                _gemBlue = value;
//            }
//        }
        
//        public Item gemMeta{
        
//            get
//            {
//                return _gemMeta;
//            }

//            set
//            {
//                _gemMeta = value;
//            }
//        }
		 */
	}
}