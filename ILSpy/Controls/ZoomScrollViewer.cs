// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;

namespace ICSharpCode.ILSpy.Controls
{
	public class ZoomScrollViewer : ScrollViewer
	{
		private bool _computedZoomButtonCollapsed = true;
		private ScrollContentPresenter _contentPresenter;

		protected override Type StyleKeyOverride => typeof(ZoomScrollViewer);

		public static readonly StyledProperty<double> CurrentZoomProperty =
			AvaloniaProperty.Register<ZoomScrollViewer, double>(nameof(CurrentZoom), 1.0, false, BindingMode.TwoWay, coerce: CoerceZoom);

		public double CurrentZoom {
			get { return (double)GetValue(CurrentZoomProperty); }
			set { SetValue(CurrentZoomProperty, value); }
		}

		static double CoerceZoom(AvaloniaObject d, double baseValue)
		{
			var zoom = (double)baseValue;
			ZoomScrollViewer sv = (ZoomScrollViewer)d;
			return Math.Max(sv.MinimumZoom, Math.Min(sv.MaximumZoom, zoom));
		}

		public static readonly StyledProperty<double> MinimumZoomProperty = AvaloniaProperty.Register<ZoomScrollViewer, double>(nameof(MinimumZoom), 0.2);

		public double MinimumZoom {
			get { return (double)GetValue(MinimumZoomProperty); }
			set { SetValue(MinimumZoomProperty, value); }
		}

		public static readonly StyledProperty<double> MaximumZoomProperty = AvaloniaProperty.Register<ZoomScrollViewer, double>(nameof(MaximumZoom), 5.0);

		public double MaximumZoom {
			get { return (double)GetValue(MaximumZoomProperty); }
			set { SetValue(MaximumZoomProperty, value); }
		}

		public static readonly StyledProperty<bool> MouseWheelZoomProperty = AvaloniaProperty.Register<ZoomScrollViewer, bool>(nameof(MouseWheelZoom),true);

		public bool MouseWheelZoom {
			get { return (bool)GetValue(MouseWheelZoomProperty); }
			set { SetValue(MouseWheelZoomProperty, value); }
		}

		public static readonly StyledProperty<bool> AlwaysShowZoomButtonsProperty = AvaloniaProperty.Register<ZoomScrollViewer, bool>(nameof(AlwaysShowZoomButtons));

		public bool AlwaysShowZoomButtons {
			get { return (bool)GetValue(AlwaysShowZoomButtonsProperty); }
			set { SetValue(AlwaysShowZoomButtonsProperty, value); }
		}

		public static readonly DirectProperty<ZoomScrollViewer, bool> ComputedZoomButtonCollapsedProperty =
			AvaloniaProperty.RegisterDirect<ZoomScrollViewer, bool>(nameof(ComputedZoomButtonCollapsed),
				c => c.ComputedZoomButtonCollapsed);

		public bool ComputedZoomButtonCollapsed {
			get => _computedZoomButtonCollapsed;
			private set => SetAndRaise(ComputedZoomButtonCollapsedProperty, ref _computedZoomButtonCollapsed, value);
		}

		protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
		{
			base.OnPropertyChanged(change);

			if (change.Property == CurrentZoomProperty || change.Property == AlwaysShowZoomButtonsProperty)
			{
				ComputedZoomButtonCollapsed = (AlwaysShowZoomButtons == false) && (Math.Abs(CurrentZoom - 1.0) < 0.001);
			}
		}

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

			_contentPresenter = e.NameScope.Get<ScrollContentPresenter>("PART_ContentPresenter");
		}

		protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
		{
			if (!e.Handled && (e.KeyModifiers & KeyModifiers.Control) != 0 && MouseWheelZoom)
			{
				double oldZoom = CurrentZoom;
				double newZoom = RoundToOneIfClose(CurrentZoom * Math.Pow(1.001, e.Delta.X));
				newZoom = Math.Max(this.MinimumZoom, Math.Min(this.MaximumZoom, newZoom));

				// adjust scroll position so that mouse stays over the same virtual coordinate
				ContentPresenter presenter = _contentPresenter;
				Vector relMousePos;
				if (presenter != null)
				{
					Point mousePos = e.GetPosition(presenter);
					relMousePos = new Vector(mousePos.X / presenter.Bounds.Width, mousePos.Y / presenter.Bounds.Height);
				}
				else
				{
					relMousePos = new Vector(0.5, 0.5);
				}

				Point scrollOffset = new Point(this.Offset.X, this.Offset.Y);
				Vector oldHalfViewport = new Vector(this.Viewport.Width / 2, this.Viewport.Height / 2);
				Vector newHalfViewport = oldHalfViewport / newZoom * oldZoom;
				Point oldCenter = scrollOffset + oldHalfViewport;
				Point virtualMousePos = scrollOffset + new Vector(relMousePos.X * this.Viewport.Width, relMousePos.Y * this.Viewport.Height);

				// As newCenter, we want to choose a point between oldCenter and virtualMousePos. The more we zoom in, the closer
				// to virtualMousePos. We'll create the line x = oldCenter + lambda * (virtualMousePos-oldCenter).
				// On this line, we need to choose lambda between -1 and 1:
				// -1 = zoomed out completely
				//  0 = zoom unchanged
				// +1 = zoomed in completely
				// But the zoom factor (newZoom/oldZoom) we have is in the range [0,+Infinity].

				// Basically, I just played around until I found a function that maps this to [-1,1] and works well.
				// "f" is squared because otherwise the mouse simply stays over virtualMousePos, but I wanted virtualMousePos
				// to move towards the middle -> squaring f causes lambda to be closer to 1, giving virtualMousePos more weight
				// then oldCenter.

				double f = Math.Min(newZoom, oldZoom) / Math.Max(newZoom, oldZoom);
				double lambda = 1 - f * f;
				if (oldZoom > newZoom)
					lambda = -lambda;

				Debug.Print("old: " + oldZoom + ", new: " + newZoom);

				Point newCenter = oldCenter + lambda * (virtualMousePos - oldCenter);
				scrollOffset = newCenter - newHalfViewport;

				SetCurrentValue(CurrentZoomProperty, newZoom);

				Offset = scrollOffset;

				e.Handled = true;
			}
			base.OnPointerWheelChanged(e);
		}

		internal static double RoundToOneIfClose(double val)
		{
			if (Math.Abs(val - 1.0) < 0.001)
				return 1.0;
			else
				return val;
		}
	}

	sealed class IsNormalZoomConverter : IValueConverter
	{
		public static readonly IsNormalZoomConverter Instance = new IsNormalZoomConverter();

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter is bool && (bool)parameter)
				return true;
			return ((double)value) == 1.0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
