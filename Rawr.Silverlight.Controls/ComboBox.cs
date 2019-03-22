using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Rawr.Silverlight.Controls
{
    [TemplatePart(Name = ComboBox.DropDownButton, Type = typeof(Button))]
    [TemplatePart(Name = ComboBox.Popup, Type = typeof(Popup))]
    [TemplatePart(Name = ComboBox.PopupChild, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ComboBox.SelectedItemControl, Type = typeof(ContentControl))]
    public partial class ComboBox : ListBox
    {
        private const string DropDownButton = "DropDownButton";
        private const string Popup = "Popup";
        private const string PopupChild = "PopupChild";
        private const string SelectedItemControl = "SelectedItemControl";

        private Button dropDownButton;
        private Popup popup;
        private ScrollViewer scrollViewer;
        private FrameworkElement popupChild;
        private ContentControl selectedItemControl;

        public ComboBox()
        {
            DefaultStyleKey = typeof(ComboBox);

            SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
            LostFocus += new RoutedEventHandler(ComboBox_LostFocus);
        }

        void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (popupInitialized) popupChild.Visibility = Visibility.Collapsed;
        }

        private class DisplayMemberValueConverter : System.Windows.Data.IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value == null)
                {
                    return "";
                }
                return value.ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool flag = true;
            if (SelectedItem != null)
            {
                if (!(SelectedItem is ListBoxItem))
                {
                    if (ItemTemplate != null)
                    {
                        selectedItemControl.ContentTemplate = ItemTemplate;
                    }
                    else if (!string.IsNullOrEmpty(DisplayMemberPath))
                    {
                        System.Windows.Data.Binding binding = new System.Windows.Data.Binding(DisplayMemberPath);
                        binding.Converter = new DisplayMemberValueConverter();
                        selectedItemControl.SetBinding(ContentControl.ContentProperty, binding);
                        flag = false;
                    }
                    if (flag)
                    {
                        selectedItemControl.Content = SelectedItem;
                    }
                }
                else
                {
                    selectedItemControl.Content = ((ListBoxItem)SelectedItem).Content;
                }
                if ((ItemContainerStyle != null) && (selectedItemControl.Style == null))
                {
                    selectedItemControl.Style = ItemContainerStyle;
                }
            }
            else
            {
                if (ItemTemplate != null)
                {
                    flag = true;
                }
                else if (!string.IsNullOrEmpty(DisplayMemberPath))
                {
                    selectedItemControl.SetBinding(ContentControl.ContentProperty, null);
                    flag = false;
                }
                if (flag)
                {
                    selectedItemControl.Content = null;
                }
            }
            popupChild.Visibility = Visibility.Collapsed;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            dropDownButton = GetTemplateChild(DropDownButton) as Button;
            popup = GetTemplateChild(Popup) as Popup;
            scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            popupChild = GetTemplateChild(PopupChild) as FrameworkElement;
            selectedItemControl = GetTemplateChild(SelectedItemControl) as ContentControl;

            if (dropDownButton != null)
            {
                dropDownButton.Click += new RoutedEventHandler(dropDownButton_Click);
            }
        }

        private bool popupInitialized;

        void dropDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (!popupInitialized)
            {
                Type parentType = popupChild.Parent.GetType();
                object[] contentAttributes = parentType.GetCustomAttributes(typeof(System.Windows.Markup.ContentPropertyAttribute), true);
                if (contentAttributes.Length > 0)
                {
                    // remove from original container
                    string contentProperty = ((System.Windows.Markup.ContentPropertyAttribute)contentAttributes[0]).Name;
                    Type contentType = parentType.GetProperty(contentProperty).PropertyType;
                    Type collectionInterface = contentType.GetInterface("ICollection`1", false);
                    if (collectionInterface != null)
                    {                        
                        object children = parentType.GetProperty(contentProperty).GetValue(popupChild.Parent, null);
                        collectionInterface.GetMethod("Remove").Invoke(children, new object[] { popupChild });
                    }
                    else if (contentType.IsSubclassOf(typeof(DependencyObject)))
                    {
                        parentType.GetProperty(contentProperty).SetValue(popupChild.Parent, null, null);
                    }
                    else
                    {
                        // check what else it can be
                        contentType = null;
                    }
                    // position in popup
                    popup.Child = popupChild;
                    popupChild.Visibility = Visibility.Collapsed;
                }
                popupInitialized = true;
            }
            
            if (popup != null)
            {
                if (popupChild.Visibility == Visibility.Visible)
                {
                    popupChild.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //popup.Width = this.ActualWidth - 20.0;
                    //popup.Height = 100.0;
                    scrollViewer.Width = this.ActualWidth - 24.0;
                    //scrollViewer.Height = 100.0;
                    GeneralTransform transform = this.TransformToVisual(Application.Current.RootVisual);
                    Point abs = transform.Transform(new Point(0.0, ActualHeight));
                    popup.HorizontalOffset = abs.X;
                    popup.VerticalOffset = abs.Y;
                    popupChild.Visibility = Visibility.Visible;
                    popup.IsOpen = true;
                }
            }
        }
    }
}