using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using Rawr.UI;

namespace Manas.Silverlight
{
    /// <summary>
    /// Indicates how to attach an event to the given target.
    /// </summary>
    /// <param name="sender">the object that fired the event</param>
    /// <param name="target">the object to which an event must be attached</param>
    /// <param name="method">the method to invoke in the event handler</param>
    public delegate void EventAttacher(object sender, DependencyObject target, MethodInfo method);

    /// <summary>Support for attaching to an event based on the name of a method.</summary>
    public static class EventSupport
    {
        /// <summary>
        /// Attaches an event to the first Parent of obj (which must be a 
        /// FrameworkElement) which declares a public method with the name 
        /// "handler" and arguments of types "types".
        /// 
        /// When that method is found, attacher is invoked with:
        /// - sender is "obj"
        /// - target is the Parent that declares the method
        /// - method is the method of Parent
        /// </summary>
        /// <param name="obj">the object to which to attach an event</param>
        /// <param name="handler">the name of the method to be found in some Parent of obj</param>
        /// <param name="attacher">indicates how to attach an event to the target</param>
        public static void AttachEvent(DependencyObject obj, string handler, Type[] types, EventAttacher attacher)
        {
            FrameworkElement fe = obj as FrameworkElement;
            if (fe == null)
            {
                throw new ArgumentException("Can only attach events to FrameworkElement instances, not to '" + obj.GetType() + "'");
            }

            fe.Loaded += (sender, e) =>
            {
                DependencyObject parent = sender as DependencyObject;

                MethodInfo info = null;
                while (info == null)
                {
                    info = parent.GetType().GetMethod(handler, types);
                    if (info != null)
                    {
                        attacher(sender, parent, info);
                        return;
                    }

                    if (parent is FrameworkElement)
                    {
                        parent = (parent as FrameworkElement).Parent;
                    }
                    else
                    {
                        parent = null;
                    }

                    if (parent == null)
                    {
                        throw new ArgumentException("Can't find handler '" + handler + "' (maybe it's not public?)");
                    }
                }
            };
        }
    }

    /// <summary>Allows reporting suggestions asynchronously.</summary>
    public delegate void SuggestCallback(object sender, Suggestion[] suggestions);

    /// <summary>Invoked when suggestions are needed.</summary>
    /// <param name="text">the text to autocomplete</param>
    /// <param name="callback">where to report the suggestions</param>
    public delegate void SuggestHandler(string text, SuggestCallback callback);

    /// <summary>Each suggestion to report for the autocompletion.</summary>
    public class Suggestion
    {
        /// <summary>What to show in the autocomplete popup.</summary>
        public string DisplayString { get; set; }

        /// <summary>What to insert as a replacement if this suggestion is selected.</summary>
        public string ReplaceString { get; set; }

        public override string ToString() { return DisplayString; }
    }

    /// <summary>Allows adding autocomplete capabilities to a TextBox.</summary>
    public class Autocomplete
    {
        #region XAML support
        public static readonly DependencyProperty Suggest = DependencyProperty.RegisterAttached("Suggest", typeof(string), typeof(Autocomplete), null);

        public static void SetSuggest(DependencyObject obj, string handler)
        {
            obj.SetValue(Suggest, handler);

            EventSupport.AttachEvent(obj, handler,
                new Type[] { typeof(string), typeof(SuggestCallback) },
                (sender, target, info) =>
                {
                    Autocomplete autocomplete = new Autocomplete(sender as TextBox);
                    autocomplete.SuggestHandler = (text, callback) =>
                    {
                        info.Invoke(target, new object[] { text, callback });
                    };
                });
        }

        public static string GetSuggest(DependencyObject obj)
        {
            return (string)obj.GetValue(Suggest);
        }
        #endregion

        private TextBox textBox;
        private ListBox listBox;

        internal Autocomplete(TextBox textBox)
        {
            this.textBox = textBox;

            textBox.LostFocus += textBox_LostFocus;
            textBox.GotFocus += (s, e) =>
            {
                OfferSuggestions();
            };
            textBox.KeyDown += textBox_KeyDown;
            textBox.KeyUp += textBlock_KeyUp;
        }

        /// <summary>Adds autocomplete support to the given TextBox.</summary>
        public Autocomplete(TextBox textBox, SuggestHandler handler) : this(textBox) {
            this.SuggestHandler = handler;
        }

        internal SuggestHandler SuggestHandler { get; set; }

        void textBox_LostFocus(object sender, RoutedEventArgs e) {
            object elem = App.GetFocusedElement();
            if (elem == listBox) {
                return;
            }

            ListBoxItem item = elem as ListBoxItem;
            if (item != null) {
                object parent = item.Parent;
                while (!(parent is ListBox) && parent is FrameworkElement) {
                    parent = (parent as FrameworkElement).Parent;
                }

                if (parent == listBox) {
                    InsertSuggestion();
                }
            }

            TextBox sndr = (TextBox)sender;
            Grid sndrprnt = (Grid)sndr.Parent;
            HideListBox(sndrprnt.Parent);
        }

        void textBox_KeyDown(object sender, KeyEventArgs e) {
            if (listBox == null) return;

            if (e.Key == Key.Down) {
                listBox.SelectedIndex = (listBox.SelectedIndex + 1) % listBox.Items.Count;
            } else if (e.Key == Key.Up) {
                if (listBox.SelectedIndex == 0) {
                    listBox.SelectedIndex = listBox.Items.Count - 1;
                } else {
                    listBox.SelectedIndex--;
                }
            } else if (e.Key == Key.Enter) {
                InsertSuggestion();
            }
        }

        void textBlock_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up) return;

            OfferSuggestions();
        }

        void OfferSuggestions()
        {
            SuggestHandler(textBox.Text, Callback);
        }

        void Callback(object sender, Suggestion[] suggestions)
        {
            // If there are no suggestions, do nothing
            //                    or
            // If there is only one suggestion but it's the text that we have in the
            // text box, do nothing
            if (suggestions == null ||
                suggestions.Length == 0 ||
                (suggestions.Length == 1 && suggestions[0].ReplaceString == textBox.Text))
            {
                //TextBox sndr = (TextBox)sender;
                //Grid sndrprnt = (Grid)sndr.Parent;
                //HideListBox(sndrprnt.Parent);
                HideListBox(sender);
                return;
            }

            ShowListBox(sender, suggestions.Length);

            listBox.Items.Clear();
            foreach (var suggestion in suggestions)
            {
                listBox.Items.Add(suggestion);
            }

            this.listBox.Dispatcher.BeginInvoke((Action) delegate
            {
                listBox.SelectedIndex = 0;
            });
        }

        void ShowListBox(object sender, int count)
        {
            //int line = 0;
            //try {
                if (listBox == null)
                {
                    listBox = new ListBox();
                    if (count >= 5) { ScrollViewer.SetVerticalScrollBarVisibility(listBox, ScrollBarVisibility.Visible); }
                    listBox.SelectionChanged += (s, e) =>
                    {
                        if (listBox.SelectedItem != null && listBox.Items.Contains(listBox.SelectedItem))
                        {
                            try
                            {
                                listBox.ScrollIntoView(listBox.SelectedItem);
                            }
                            catch { }
                        }
                    };
                    listBox.MinWidth = textBox.RenderSize.Width;

                    ArmoryLoadDialog dialog = (ArmoryLoadDialog)sender;

                    //var canvas = (Grid)System.Windows.Application.Current.RootVisual;
                    var canvas = (Grid)dialog.LayoutRoot;
                    var transform = textBox.TransformToVisual(canvas);
                    var topLeft = transform.Transform(new Point(0, 0));

                    Canvas.SetLeft(listBox, topLeft.X);
                    Canvas.SetTop(listBox, topLeft.Y + textBox.RenderSize.Height);
                    canvas.Children.Add(listBox);
                }

                if (count >= 5)
                {
                    count = 5;
                }
                listBox.MaxHeight = count * 21;
            //} catch (Exception ex) {
                //new Rawr.DPSWarr.ErrorBoxDPSWarr("AutoComplete issue", ex.Message, "ShowListBox", "No Additional Info", ex.StackTrace, line);
            //}
        }

        private void InsertSuggestion()
        {
            Suggestion suggestion = listBox.SelectedItem as Suggestion;
            if (suggestion != null)
            {
                textBox.Text = suggestion.ReplaceString;
                textBox.Select(textBox.Text.Length, 0);
            }
        }

        void HideListBox(object sender)
        {
            //string info = "No Additional Info";
            //try {
                if (listBox != null)
                {
                    //Grid sndr = (Grid)sender;
                    //info = "\r\n- Name: " + sndr.Name + "\r\n- ParentType: " + sndr.Parent.GetType();
                    //info += "\r\n- ControlType: " + sender.ToString() + "\r\n- Type: " + sender.GetType().ToString();
                    ArmoryLoadDialog dialog = (ArmoryLoadDialog)((Grid)sender).Parent;
                    //var canvas = (Grid)System.Windows.Application.Current.RootVisual;
                    var canvas = (Grid)dialog.LayoutRoot;// info = "No Additional Info";
                    canvas.Children.Remove(listBox);
                    listBox = null;
                }
            //} catch (Exception ex) {
                //new Rawr.DPSWarr.ErrorBoxDPSWarr("AutoComplete issue", ex.Message, "HideListBox", info, ex.StackTrace, 0);
            //}
        }
    }
}
