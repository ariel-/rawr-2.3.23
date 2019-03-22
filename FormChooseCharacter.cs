using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    /**
     * FormChooseCharacter
     * @author Charinna
     * This class/form allows a user to select a Realm & Character from a list
     * of realms & characters found in a CharacterProfiler export.
     */
    public partial class FormChooseCharacter : Form
    {
        CharacterProfilerData m_characterList;

        public CharacterProfilerCharacter Character
        {
            get { return m_characterList.Realms[comboBoxRealm.SelectedIndex].Characters[comboBoxCharacter.SelectedIndex]; }
        }

        public FormChooseCharacter(CharacterProfilerData characterList)
        {
            InitializeComponent();

            m_characterList = characterList;

            foreach (CharacterProfilerFailedImport error in m_characterList.Errors)
            {
                object[] asRowData = { error.Realm, error.Character, error.Error };
                dataGridFailedImport.Rows.Add(asRowData);
            }

            foreach (CharacterProfilerRealm realm in m_characterList.Realms)
            {
                comboBoxRealm.Items.Add(realm.Name);
            }

            comboBoxRealm.SelectedIndex = 0;
            updateCharacterList();
        }

        private void comboBoxRealm_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateCharacterList();
        }

        private void comboBoxCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateCharacterSummary();
        }

        private void updateCharacterList()
        {
            comboBoxCharacter.Items.Clear();

            foreach (CharacterProfilerCharacter character in m_characterList.Realms[comboBoxRealm.SelectedIndex].Characters)
            {
                comboBoxCharacter.Items.Add(character.Name);
            }

            comboBoxCharacter.SelectedIndex = 0;
        }

        private void updateCharacterSummary()
        {
            labelDescription.Text = m_characterList.Realms[comboBoxRealm.SelectedIndex].Characters[comboBoxCharacter.SelectedIndex].Summary;
        }
    }
}
