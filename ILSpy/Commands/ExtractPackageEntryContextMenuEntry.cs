// Copyright (c) 2021 Siegfried Pammer
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Platform.Storage;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;

using Microsoft.Win32;

namespace ICSharpCode.ILSpy
{
	[ExportContextMenuEntry(Header = nameof(Resources.ExtractPackageEntry), Category = nameof(Resources.Save), Icon = "Images/Save")]
	sealed class ExtractPackageEntryContextMenuEntry : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			// Get all assemblies in the selection that are stored inside a package.
			var selectedNodes = context.SelectedTreeNodes.OfType<AssemblyTreeNode>()
				.Where(asm => asm.PackageEntry != null).ToArray();
			// Get root assembly to infer the initial directory for the save dialog.
			var bundleNode = selectedNodes.FirstOrDefault()?.Ancestors().OfType<AssemblyTreeNode>()
				.FirstOrDefault(asm => asm.PackageEntry == null);
			if (bundleNode == null)
				return;
			var assembly = selectedNodes[0].PackageEntry;
			
			var file = context.TopLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
			{
				SuggestedFileName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(assembly.Name)),
				SuggestedStartLocation = context.TopLevel.StorageProvider.TryGetFolderFromPathAsync(Path.GetDirectoryName(bundleNode.LoadedAssembly.FileName)!).WaitOnDispatcherFrame()
				// TODO Avalonia: map to FileTypeChoices 
				// FileTypeChoices = ".NET assemblies|*.dll;*.exe;*.winmd" + Resources.AllFiles; 
			}).WaitOnDispatcherFrame();

			if (file is null)
				return;
			
			Docking.DockWorkspace.Instance.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
				AvalonEditTextOutput output = new AvalonEditTextOutput();
				Stopwatch stopwatch = Stopwatch.StartNew();
				stopwatch.Stop();

				if (selectedNodes.Length == 1)
				{
					SaveEntry(output, selectedNodes[0].PackageEntry, file);
				}
				else
				{
					using var parent = file.GetParentAsync().WaitOnDispatcherFrame();
					foreach (var node in selectedNodes)
					{
						var fileName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(node.PackageEntry.Name));
						using var newFile = parent.CreateFileAsync(fileName!).WaitOnDispatcherFrame();
						SaveEntry(output, node.PackageEntry, newFile);
					}
				}
				output.WriteLine(Resources.GenerationCompleteInSeconds, stopwatch.Elapsed.TotalSeconds.ToString("F1"));
				output.WriteLine();
				output.AddButton(null, Resources.OpenExplorer, delegate { context.TopLevel.Launcher.LaunchFileAsync(file); });
				output.WriteLine();
				return output;
			}, ct)).Then(output => Docking.DockWorkspace.Instance.ShowText(output)).HandleExceptions();
		}

		void SaveEntry(ITextOutput output, PackageEntry entry, IStorageFile file)
		{
			output.Write(entry.Name + ": ");
			using Stream stream = entry.TryOpenStream();
			if (stream == null)
			{
				output.WriteLine("Could not open stream!");
				return;
			}

			stream.Position = 0;
			using Stream fileStream = file.OpenWriteAsync().WaitOnDispatcherFrame();
			stream.CopyTo(fileStream);
			output.WriteLine("Written to " + file.Name);
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			var selectedNodes = context.SelectedTreeNodes?.OfType<AssemblyTreeNode>()
				.Where(asm => asm.PackageEntry != null) ?? Enumerable.Empty<AssemblyTreeNode>();
			return selectedNodes.Any();
		}
	}
}
