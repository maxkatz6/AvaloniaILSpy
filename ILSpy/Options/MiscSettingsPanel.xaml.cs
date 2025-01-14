﻿// Copyright (c) 2017 AlphaSierraPapa for the SharpDevelop Team
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

using System.Xml.Linq;

using Avalonia.Controls;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Interaction logic for MiscSettingsPanel.xaml
	/// </summary>
	[ExportOptionPage(Title = nameof(Properties.Resources.Misc), Order = 30)]
	public partial class MiscSettingsPanel : UserControl, IOptionPage
	{
		public MiscSettingsPanel()
		{
			InitializeComponent();
		}

		public void Load(ILSpySettings settings)
		{
			this.DataContext = LoadMiscSettings(settings);
		}

		static MiscSettingsViewModel currentMiscSettings;

		public static MiscSettingsViewModel CurrentMiscSettings {
			get {
				return currentMiscSettings ?? (currentMiscSettings = LoadMiscSettings(ILSpySettings.Load()));
			}
		}

		public static MiscSettingsViewModel LoadMiscSettings(ILSpySettings settings)
		{
			var s = MiscSettings.Load(settings);
			return new MiscSettingsViewModel(s);
		}

		public void Save(XElement root)
		{
			var s = (MiscSettingsViewModel)this.DataContext;
			IMiscSettings.Save(root, s);

			currentMiscSettings = null; // invalidate cached settings
		}

		public void LoadDefaults()
		{
			currentMiscSettings = new MiscSettingsViewModel(MiscSettings.Load(ILSpySettings.Load()));
			this.DataContext = currentMiscSettings;
		}
	}
}
