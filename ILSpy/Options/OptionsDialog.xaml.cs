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
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Xml.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

using AvaloniaEdit.Highlighting;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Options
{
	public class TabItemViewModel
	{
		public TabItemViewModel(string header, Control content)
		{
			Header = header;
			Content = content;
		}

		public string Header { get; }
		public Control Content { get; }
	}

	/// <summary>
	/// Interaction logic for OptionsDialog.xaml
	/// </summary>
	public partial class OptionsDialog : Window
	{

		readonly Lazy<Control, IOptionsMetadata>[] optionPages;

		public OptionsDialog()
		{
			InitializeComponent();
			// These used to have [ImportMany(..., RequiredCreationPolicy = CreationPolicy.NonShared)], so they use their own
			// ExportProvider instance.
			// FIXME: Ideally, the export provider should be disposed when it's no longer needed.
			var ep = App.ExportProviderFactory.CreateExportProvider();
			this.optionPages = ep.GetExports<Control, IOptionsMetadata>("OptionPages").ToArray();
			ILSpySettings settings = ILSpySettings.Load();
			var itemsSource = new List<TabItemViewModel>();
			foreach (var optionPage in optionPages.OrderBy(p => p.Metadata.Order))
			{
				var tabItem = new TabItemViewModel(MainWindow.GetResourceString(optionPage.Metadata.Title), optionPage.Value);

				itemsSource.Add(tabItem);

				IOptionPage page = optionPage.Value as IOptionPage;
				if (page != null)
					page.Load(settings);
			}
			tabControl.ItemsSource = itemsSource;
		}

		void OKButton_Click(object sender, RoutedEventArgs e)
		{
			ILSpySettings.Update(
				delegate (XElement root) {
					foreach (var optionPage in optionPages)
					{
						IOptionPage page = optionPage.Value as IOptionPage;
						if (page != null)
							page.Save(root);
					}
				});
			Close(true);
		}

		private void DefaultsButton_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show(this, Properties.Resources.ResetToDefaultsConfirmationMessage, "ILSpy", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				var page = tabControl.SelectedValue as IOptionPage;
				if (page != null)
					page.LoadDefaults();
			}
		}
	}

	public interface IOptionsMetadata
	{
		string Title { get; }
		int Order { get; }
	}

	public interface IOptionPage
	{
		void Load(ILSpySettings settings);
		void Save(XElement root);
		void LoadDefaults();
	}

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExportOptionPageAttribute : ExportAttribute
	{
		public ExportOptionPageAttribute() : base("OptionPages", typeof(Visual))
		{ }

		public string Title { get; set; }

		public int Order { get; set; }
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._View), Header = nameof(Resources._Options), MenuCategory = nameof(Resources.Options), MenuOrder = 999)]
	sealed class ShowOptionsCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			OptionsDialog dlg = new OptionsDialog();
			if (dlg.ShowDialog<bool>(MainWindow.Instance).WaitOnDispatcherFrame())
			{
				new RefreshCommand().Execute(parameter);
			}
		}
	}
}