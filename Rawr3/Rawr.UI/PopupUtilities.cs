using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace Rawr.UI
{
    public class PopupUtilities
    {
        private PopupUtilities()
        {
        }

        public static void RegisterPopup(Control rootControl, Popup popup, Action close)
        {
            RegisterPopup(rootControl, popup, (Control)null, close);
        }

        public static void RegisterPopup(Control rootControl, Popup popup, Control button, Action close)
        {
#if !SILVERLIGHT
            PopupUtilities util = new PopupUtilities();
            util.rootControl = rootControl;
            util.popup = popup;
            util.button = button;
            util.Close = close;
            rootControl.MouseDown += util.Control_MouseDown;
            rootControl.LostMouseCapture += util.Control_LostMouseCapture;
            popup.Opened += util.Popup_Opened;
            popup.Closed += util.Popup_Closed;
            // detect and zero out silverlight margins
            //<Grid Background="#00000000" Margin="-10000,-10000,-10000,-10000" IsHitTestVisible="True" MouseLeftButtonDown="Background_MouseLeftButtonDown">
            //<Border BorderThickness="1" Width="350" Height="500" Margin="10000,10000,10000,10000" CornerRadius="0,0,4,4" BorderBrush="#CC000066" Background="#F3F3F3FF">

            Grid grid = popup.Child as Grid;
            if (grid != null)
            {
                if (grid.Margin.Left == -10000)
                {
                    grid.Margin = new Thickness();
                }
                if (grid.Children.Count > 0)
                {
                    Border border = grid.Children[0] as Border;
                    if (border != null)
                    {
                        if (border.Margin.Left == 10000)
                        {
                            border.Margin = new Thickness();
                        }
                    }
                }
            }
#endif
        }

        public static void RegisterPopup(Control rootControl, Popup popup1, Popup popup2, Action close)
        {
#if !SILVERLIGHT
            PopupUtilities util = new PopupUtilities();
            util.rootControl = rootControl;
            util.popup = popup1;
            util.popup2 = popup2;
            util.button = null;
            util.Close = close;
            rootControl.MouseDown += util.Control_MouseDown;
            rootControl.LostMouseCapture += util.Control_LostMouseCapture;
            popup1.Opened += util.Popup_Opened;
            popup1.Closed += util.Popup_Closed;
            popup2.Opened += util.Popup_Opened;
            popup2.Closed += util.Popup_Closed;
#endif
        }

#if !SILVERLIGHT
        private Control rootControl;
        private Popup popup;
        private Popup popup2;
        private Control button;
        private Action Close;

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.Captured == rootControl && e.OriginalSource == rootControl)
            {
                Close();
            }
        }

        private bool IsDescendant(DependencyObject reference, DependencyObject node)
        {
            DependencyObject o = node;
            while (o != null)
            {
                if (o == reference)
                {
                    return true;
                }
                Popup popup = o as Popup;
                if (popup != null)
                {
                    o = popup.Parent;
                    if (o == null)
                    {
                        o = popup.PlacementTarget;
                    }
                }
                else
                {
                    //handle content elements separately
                    ContentElement contentElement = o as ContentElement;
                    if (contentElement != null)
                    {
                        DependencyObject parent = ContentOperations.GetParent(contentElement);
                        if (parent != null)
                        {
                            o = parent;
                        }
                        FrameworkContentElement fce = contentElement as FrameworkContentElement;
                        if (fce != null)
                        {
                            o = fce.Parent;
                        }
                        else
                        {
                            o = null;
                        }
                    }
                    else
                    {
                        //also try searching for parent in framework elements (such as DockPanel, etc)
                        FrameworkElement frameworkElement = o as FrameworkElement;
                        if (frameworkElement != null)
                        {
                            DependencyObject parent = frameworkElement.Parent;
                            if (parent != null)
                            {
                                o = parent;
                                continue;
                            }
                        }

                        //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
                        o = VisualTreeHelper.GetParent(o);
                    }
                }
            }
            return false;
        }

        private void Control_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (Mouse.Captured != rootControl)
            {
                if (button != null && e.OriginalSource == button && Mouse.Captured == null)
                {
                    // reclicking on button, if you close the click will cause toggle => open
                    return;
                }
                if (e.OriginalSource == rootControl)
                {
                    // If capture is null or it's not below the combobox, close. 
                    if (Mouse.Captured == null || !IsDescendant(rootControl, Mouse.Captured as DependencyObject))
                    {
                        Close();
                    }
                }
                else
                {
                    if (IsDescendant(rootControl, e.OriginalSource as DependencyObject))
                    {
                        // Take capture if one of our children gave up capture
                        if ((popup.IsOpen || (popup2 != null && popup2.IsOpen)) && Mouse.Captured == null)
                        {
                            Mouse.Capture(rootControl, CaptureMode.SubTree);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        Close();
                    }
                }
            }
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            Mouse.Capture(rootControl, CaptureMode.SubTree);
            if (popup2 != null)
            {
                if (sender == popup)
                {
                    if (popup2.IsOpen)
                    {
                        popup2.IsOpen = false;
                    }
                }
                if (sender == popup2)
                {
                    if (popup.IsOpen)
                    {
                        popup.IsOpen = false;
                    }
                }
            }
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            if (Mouse.Captured == rootControl)
            {
                Mouse.Capture(null);
            }
        }
#endif
    }
}
