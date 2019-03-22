using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Rawr.UI
{
    [TemplatePart(Name = "FullRect", Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = "HeaderRect", Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = "HeaderContainer", Type = typeof(ContentControl))]
    public class GroupBox : ContentControl
    {
#if !SILVERLIGHT
        static GroupBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(typeof(GroupBox)));
        }
#endif

        private RectangleGeometry FullRect;
        private RectangleGeometry HeaderRect;
        private ContentControl HeaderContainer;

        public GroupBox()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(GroupBox);
#endif
            this.SizeChanged += GroupBox_SizeChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            FullRect = (RectangleGeometry)GetTemplateChild("FullRect");
            HeaderRect = (RectangleGeometry)GetTemplateChild("HeaderRect");
            HeaderContainer = (ContentControl)GetTemplateChild("HeaderContainer");
            HeaderContainer.SizeChanged += HeaderContainer_SizeChanged;
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(GroupBox), null);
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(GroupBox), null);
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        private void GroupBox_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            FullRect.Rect = new Rect(new Point(), e.NewSize);
        }

        private void HeaderContainer_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            HeaderRect.Rect = new Rect(new Point(HeaderContainer.Margin.Left, 0), e.NewSize);
        }
    }
}
