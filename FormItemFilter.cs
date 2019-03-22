using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    public partial class FormItemFilter : Form
    {
        public FormItemFilter()
        {
            InitializeComponent();
            comboBoxMinItemQuality.DataSource = Enum.GetValues(typeof(ItemQuality));
            comboBoxMaxItemQuality.DataSource = Enum.GetValues(typeof(ItemQuality));
        }

        private void itemFilterTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            buttonDelete.Enabled = itemFilterTreeView.SelectedNode != null;
            buttonNewSubfilter.Enabled = itemFilterTreeView.SelectedNode != null;
            buttonUp.Enabled = e.Node != null && e.Node.PrevNode != null;
            buttonDown.Enabled = e.Node != null && e.Node.NextNode != null;
            checkBoxAdditiveFilter.Enabled = (e.Node.Parent == null);
            ItemFilterRegex regex = (ItemFilterRegex)e.Node.Tag;
            bindingSourceItemFilter.DataSource = regex;
        }

        private void buttonNewFilter_Click(object sender, EventArgs e)
        {
            ItemFilterRegex regex = new ItemFilterRegex();
            TreeNode node = itemFilterTreeView.GetNode(regex);
            ItemFilter.FilterList.Add(regex);
            itemFilterTreeView.Nodes.Add(node);
            itemFilterTreeView.SelectedNode = node;
        }

        private void buttonNewSubfilter_Click(object sender, EventArgs e)
        {
            TreeNode parent = itemFilterTreeView.SelectedNode;
            ItemFilterRegex parentRegex = (ItemFilterRegex)parent.Tag;
            ItemFilterRegex regex = new ItemFilterRegex();
            TreeNode node = itemFilterTreeView.GetNode(regex);
            parentRegex.RegexList.Add(regex);
            parent.Nodes.Add(node);
            itemFilterTreeView.SelectedNode = node;
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            TreeNode node = itemFilterTreeView.SelectedNode;
            ItemFilterRegex regex = (ItemFilterRegex)node.Tag;
            int index = node.Index;
            TreeNode parent = node.Parent;
            if (parent == null)
            {
                itemFilterTreeView.Nodes.RemoveAt(index);
                itemFilterTreeView.Nodes.Insert(index - 1, node);
                ItemFilter.FilterList.RemoveAt(index);
                ItemFilter.FilterList.Insert(index - 1, regex);
            }
            else
            {
                parent.Nodes.RemoveAt(index);
                parent.Nodes.Insert(index - 1, node);
                ItemFilterRegex parentRegex = (ItemFilterRegex)parent.Tag;
                parentRegex.RegexList.RemoveAt(index);
                parentRegex.RegexList.Insert(index - 1, regex);
            }
            itemFilterTreeView.SelectedNode = node;            
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            TreeNode node = itemFilterTreeView.SelectedNode;
            ItemFilterRegex regex = (ItemFilterRegex)node.Tag;
            int index = node.Index;
            TreeNode parent = node.Parent;
            if (parent == null)
            {
                itemFilterTreeView.Nodes.RemoveAt(index);
                itemFilterTreeView.Nodes.Insert(index + 1, node);
                ItemFilter.FilterList.RemoveAt(index);
                ItemFilter.FilterList.Insert(index + 1, regex);
            }
            else
            {
                parent.Nodes.RemoveAt(index);
                parent.Nodes.Insert(index + 1, node);
                ItemFilterRegex parentRegex = (ItemFilterRegex)parent.Tag;
                parentRegex.RegexList.RemoveAt(index);
                parentRegex.RegexList.Insert(index + 1, regex);
            }
            itemFilterTreeView.SelectedNode = node;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            TreeNode node = itemFilterTreeView.SelectedNode;
            ItemFilterRegex regex = (ItemFilterRegex)node.Tag;
            int index = node.Index;
            TreeNode parent = node.Parent;
            if (parent == null)
            {
                itemFilterTreeView.Nodes.RemoveAt(index);
                ItemFilter.FilterList.RemoveAt(index);
            }
            else
            {
                parent.Nodes.RemoveAt(index);
                ItemFilterRegex parentRegex = (ItemFilterRegex)parent.Tag;
                parentRegex.RegexList.RemoveAt(index);
            }
            itemFilterTreeView.SelectedNode = null;
            bindingSourceItemFilter.DataSource = typeof(Rawr.ItemFilterRegex);
            buttonDelete.Enabled = false;
            buttonNewSubfilter.Enabled = false;
            buttonUp.Enabled = false;
            buttonDown.Enabled = false;
        }

        private void bindingSourceItemFilter_CurrentItemChanged(object sender, EventArgs e)
        {
            TreeNode node = itemFilterTreeView.SelectedNode;
            if (node != null)
            {
                ItemFilterRegex regex = (ItemFilterRegex)bindingSourceItemFilter.Current;
                // sanity check
                if (regex == node.Tag && node.Text != regex.Name)
                {
                    node.Text = regex.Name;
                }
            }
        }
    }
}
