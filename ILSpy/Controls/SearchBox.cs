﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace ICSharpCode.ILSpy.Controls
{
	// TODO Avalonia: replace with AutoCompleteBox
	public class SearchBox : TextBox
	{
		protected override Type StyleKeyOverride => typeof(SearchBox);

		#region Dependency properties

		public static readonly StyledProperty<string> WatermarkTextProperty = AvaloniaProperty.Register<SearchBox, string>(nameof(WatermarkText));

		public static readonly StyledProperty<Brush> WatermarkColorProperty = AvaloniaProperty.Register<SearchBox, Brush>(nameof(WatermarkColor));

		public static readonly StyledProperty<bool> HasTextProperty = AvaloniaProperty.Register<SearchBox, bool>(nameof(HasText));

		public static readonly StyledProperty<TimeSpan> UpdateDelayProperty =
			AvaloniaProperty.Register<SearchBox, TimeSpan>(nameof(UpdateDelay), TimeSpan.FromMilliseconds(200));

		#endregion

		#region Public Properties

		public string WatermarkText {
			get { return (string)GetValue(WatermarkTextProperty); }
			set { SetValue(WatermarkTextProperty, value); }
		}

		public Brush WatermarkColor {
			get { return (Brush)GetValue(WatermarkColorProperty); }
			set { SetValue(WatermarkColorProperty, value); }
		}

		public bool HasText {
			get { return (bool)GetValue(HasTextProperty); }
			private set { SetValue(HasTextProperty, value); }
		}

		public TimeSpan UpdateDelay {
			get { return (TimeSpan)GetValue(UpdateDelayProperty); }
			set { SetValue(UpdateDelayProperty, value); }
		}

		#endregion

		#region Handlers

		private void IconBorder_MouseLeftButtonUp(object obj, PointerReleasedEventArgs e)
		{
			if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased && this.HasText)
				this.Text = string.Empty;
		}

		#endregion

		#region Overrides

		DispatcherTimer timer;

		// protected override void OnTextChanged(TextChangedEventArgs e)
		// {
		// 	base.OnTextChanged(e);
		//
		// 	HasText = this.Text.Length > 0;
		// 	if (timer == null)
		// 	{
		// 		timer = new DispatcherTimer();
		// 		timer.Tick += timer_Tick;
		// 	}
		// 	timer.Stop();
		// 	timer.Interval = this.UpdateDelay;
		// 	timer.Start();
		//
		// 	UpdateWatermarkLabel();
		// }
		//
		// private void UpdateWatermarkLabel()
		// {
		// 	Label wl = (Label)GetTemplateChild("WatermarkLabel");
		// 	if (wl != null)
		// 		wl.Visibility = HasText ? Visibility.Hidden : Visibility.Visible;
		// }

		void timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();
			timer = null;
			// TODO Avalonia 11.1 (if needed)
			// var textBinding = GetBindingExpression(TextProperty);
			// if (textBinding != null)
			// {
			// 	textBinding.UpdateSource();
			// }
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			//UpdateWatermarkLabel();
			base.OnLostFocus(e);
		}

		protected override void OnGotFocus(GotFocusEventArgs e)
		{
			//UpdateWatermarkLabel();
			base.OnGotFocus(e);
		}

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

			Border iconBorder = e.NameScope.Get<Border>("PART_IconBorder");
			if (iconBorder != null)
			{
				iconBorder.PointerReleased += IconBorder_MouseLeftButtonUp;
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape && this.Text.Length > 0)
			{
				this.Text = string.Empty;
				e.Handled = true;
			}
			else
			{
				base.OnKeyDown(e);
			}
		}
		#endregion
	}
}
