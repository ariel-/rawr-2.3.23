using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Rawr.Base;

namespace Rawr.DPSWarr
{
	public partial class AbilityTooltip : UserControl
	{
        public void Show(UIElement relativeTo) { Show(relativeTo, 0, 0); }
        public void Show(UIElement relativeTo, double offsetX, double offsetY)
        {
#if SILVERLIGHT
            try
            {
                GeneralTransform gt = relativeTo.TransformToVisual((UIElement)this.Parent);
                Point offset = gt.Transform(new Point(offsetX, offsetY));
                AbilityPopup.VerticalOffset = offset.Y + 18;
                AbilityPopup.HorizontalOffset = offset.X;
                AbilityPopup.IsOpen = true;

                /*AbilityGrid.Measure(Rawr.UI.App.Current.RootVisual.DesiredSize);

                GeneralTransform transform = relativeTo.TransformToVisual(App.Current.RootVisual);
                double distBetweenBottomOfPopupAndBottomOfWindow =
                    App.Current.RootVisual.RenderSize.Height - offsetY -
                    transform.Transform(new Point(0, AbilityGrid.DesiredSize.Height)).Y;
                if (distBetweenBottomOfPopupAndBottomOfWindow < 0)
                {
                    AbilityPopup.VerticalOffset += distBetweenBottomOfPopupAndBottomOfWindow;
                }*/
            }
            catch (ArgumentException)
            {
                // Value does not fall within the expected range
                // apparently happens if you call while it's still loading the visual tree or something
            }
#else
            AbilityPopup.PlacementTarget = relativeTo;
            AbilityPopup.PlacementRectangle = new Rect(0, offsetY, offsetX, relativeTo.RenderSize.Height);
            AbilityPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
            AbilityPopup.IsOpen = true;
#endif
        }

        public void Hide()
        {
            AbilityPopup.IsOpen = false;
        }

        public void Setup(string name, string desc, string whatitdo)
        {
            LB_Name.Text = name;
            TB_Desc.Text = desc;
            TB_WhatItDo.Text = whatitdo;
        }

        public AbilityTooltip()
		{
			// Required to initialize variables
			InitializeComponent();
        }
	}
}