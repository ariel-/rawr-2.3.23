using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    public class ItemFilterTreeView : TreeView
    {
        public ItemFilterTreeView()
        {
            GenerateNodes();
        }

        private bool editMode;

        public bool EditMode
        {
            get { return editMode; }
            set 
            {
                editMode = value;
                GenerateNodes();
            }
        }

        public void GenerateNodes()
        {
            Nodes.Clear();
            CheckBoxes = !EditMode;
            foreach (ItemFilterRegex regex in ItemFilter.FilterList)
            {
                Nodes.Add(GetNode(regex));
            }
            if (!EditMode)
            {
                TreeNode otherNode = new TreeNode("Other");
                otherNode.Checked = ItemFilter.OtherEnabled;
                Nodes.Add(otherNode);
            }
        }

        public TreeNode GetNode(ItemFilterRegex regex)
        {
            TreeNode node = new TreeNode(regex.Name);
            node.Checked = regex.Enabled;
            node.Tag = regex;
            if (regex.RegexList.Count > 0)
            {
                foreach (ItemFilterRegex childRegex in regex.RegexList)
                {
                    node.Nodes.Add(GetNode(childRegex));
                }
                if (!EditMode)
                {
                    TreeNode otherNode = new TreeNode("Other");
                    otherNode.Checked = regex.OtherRegexEnabled;
                    node.Nodes.Add(otherNode);
                }
            }
            return node;
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);
            if (e.Action != TreeViewAction.Unknown)
            {
                // user changed checked state
                TreeNode node = e.Node;
                ItemFilterRegex regex = (ItemFilterRegex)node.Tag;
                if (regex != null)
                {
                    // enabled on a regex changed
                    if (node.Checked)
                    {
                        SetChildNodes(node, true);
                        // it got checked, make sure parents are checked also
                        while (regex != null)
                        {
                            node.Checked = true;
                            regex.Enabled = true;
                            node = node.Parent;
                            if (node != null)
                            {
                                regex = (ItemFilterRegex)node.Tag;
                            }
                            else
                            {
                                regex = null;
                            }
                        }
                    }
                    else
                    {
                        // it got unchecked, make sure all child nodes are unchecked
                        SetChildNodes(node, false);
                        AutoDisable(node.Parent);
                    }
                }
                else
                {
                    // enabled for other got changed, there are no child nodes
                    if (node.Parent == null)
                    {
                        // this is the global other
                        ItemFilter.OtherEnabled = node.Checked;
                    }
                    else
                    {
                        // other below a regex
                        if (node.Checked)
                        {
                            // make sure all parents are checked
                            node = node.Parent;
                            regex = (ItemFilterRegex)node.Tag;
                            regex.OtherRegexEnabled = true;
                            while (regex != null && !regex.Enabled)
                            {
                                node.Checked = true;
                                regex.Enabled = true;
                                node = node.Parent;
                                if (node != null)
                                {
                                    regex = (ItemFilterRegex)node.Tag;
                                }
                                else
                                {
                                    regex = null;
                                }
                            }
                        }
                        else
                        {
                            // just disable it
                            node = node.Parent;
                            regex = (ItemFilterRegex)node.Tag;
                            regex.OtherRegexEnabled = false;
                            AutoDisable(node.Parent);
                        }
                    }
                }
                // trigger item update
                FormMain.Instance.Cursor = Cursors.WaitCursor;
                ItemCache.OnItemsChanged();
                FormMain.Instance.Cursor = Cursors.Default;
            }
        }

        private void SetChildNodes(TreeNode node, bool value)
        {
            ItemFilterRegex regex = (ItemFilterRegex)node.Tag;
            node.Checked = value;
            if (regex != null)
            {
                regex.Enabled = value;
                regex.OtherRegexEnabled = value;
            }
            foreach (TreeNode child in node.Nodes)
            {
                SetChildNodes(child, value);
            }
        }

        private void AutoDisable(TreeNode node)
        {
            if (node != null)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Checked) return;
                }
                node.Checked = false;
                ItemFilterRegex regex = (ItemFilterRegex)node.Tag;
                regex.Enabled = false;
                AutoDisable(node.Parent);
            }
        }
    }
}
