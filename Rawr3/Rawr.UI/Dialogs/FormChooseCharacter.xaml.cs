using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.UI
{
    public partial class FormChooseCharacter : ChildWindow
    {
        CharacterProfilerData m_characterList;

        public CharacterProfilerCharacter Character
        {
            get { return m_characterList.Realms[CB_Realm.SelectedIndex].Characters[CB_Character.SelectedIndex]; }
        }

        static FormChooseCharacter()
        {
            // Nothing to do here
        }

        public FormChooseCharacter(CharacterProfilerData characterList)
        {
            InitializeComponent();

            m_characterList = characterList;

            foreach (CharacterProfilerFailedImport error in m_characterList.Errors)
            {
                object[] asRowData = { error.Realm, error.Character, error.Error };
                //dataGridFailedImport.Rows.Add(asRowData);
            }

            foreach (CharacterProfilerRealm realm in m_characterList.Realms)
            {
                CB_Realm.Items.Add(realm.Name);
            }

            CB_Realm.SelectedIndex = 0;
            updateCharacterList();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Run Load Character from Profiler action
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void CB_Realm_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            updateCharacterList();
        }

        private void CB_Character_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            updateCharacterSummary();
        }

        private void updateCharacterList()
        {
            CB_Character.Items.Clear();

            foreach (CharacterProfilerCharacter character in m_characterList.Realms[CB_Realm.SelectedIndex].Characters)
            {
                CB_Character.Items.Add(character.Name);
            }

            CB_Character.SelectedIndex = 0;
        }

        private void updateCharacterSummary()
        {
            LB_Selection.Text = m_characterList.Realms[CB_Realm.SelectedIndex].Characters[CB_Character.SelectedIndex].Summary;
        }
    }
}

