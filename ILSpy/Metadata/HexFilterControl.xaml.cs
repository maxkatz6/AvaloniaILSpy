﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.Templates;

using DataGridExtensions;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Interaction logic for HexFilterControl.xaml
	/// </summary>
	public partial class HexFilterControl : TemplatedControl
	{
		TextBox textBox;

		public HexFilterControl()
		{
			InitializeComponent();
		}

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);
			textBox = e.NameScope.Find<TextBox>("textBox");
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var text = ((TextBox)sender)?.Text;

			Filter = new ContentFilter(text);
		}

		public IContentFilter Filter {
			get { return (IContentFilter)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}
		/// <summary>
		/// Identifies the Filter dependency property
		/// </summary>
		public static readonly StyledProperty<IContentFilter> FilterProperty = AvaloniaProperty.Register<HexFilterControl, IContentFilter>(nameof(Filter),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, args) => ((HexFilterControl)o).Filter_Changed(args.NewValue)));

		void Filter_Changed(object newValue)
		{
			var textBox = this.textBox;
			if (textBox == null)
				return;

			textBox.Text = (newValue as ContentFilter)?.Value ?? string.Empty;
		}

		class ContentFilter : IContentFilter
		{
			readonly string filter;

			public ContentFilter(string filter)
			{
				this.filter = filter;
			}

			public bool IsMatch(object value)
			{
				if (string.IsNullOrWhiteSpace(filter))
					return true;
				if (value == null)
					return false;

				return string.Format("{0:x8}", value).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
			}

			public string Value => filter;
		}
	}
}
