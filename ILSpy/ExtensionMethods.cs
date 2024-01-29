// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// ExtensionMethods used in ILSpy.
	/// </summary>
	public static class ExtensionMethods
	{
		public static string ToSuffixString(this System.Reflection.Metadata.EntityHandle handle, bool showMetadataTokens, bool useBase10)
		{
			if (!showMetadataTokens)
				return string.Empty;

			int token = System.Reflection.Metadata.Ecma335.MetadataTokens.GetToken(handle);
			if (useBase10)
				return " @" + token;
			return " @" + token.ToString("x8");
		}

		/// <summary>
		/// Takes at most <paramref name="length" /> first characters from string, and appends '...' if string is longer.
		/// String can be null.
		/// </summary>
		public static string TakeStartEllipsis(this string s, int length)
		{
			if (string.IsNullOrEmpty(s) || length >= s.Length)
				return s;
			return s.Substring(0, length) + "...";
		}

		/// <summary>
		/// Equivalent to <code>collection.Select(func).ToArray()</code>, but more efficient as it makes
		/// use of the input collection's known size.
		/// </summary>
		public static U[] SelectArray<T, U>(this ICollection<T> collection, Func<T, U> func)
		{
			U[] result = new U[collection.Count];
			int index = 0;
			foreach (var element in collection)
			{
				result[index++] = func(element);
			}
			return result;
		}

		public static ICompilation? GetTypeSystemWithCurrentOptionsOrNull(this PEFile file)
		{
			return LoadedAssemblyExtensions.GetLoadedAssembly(file)
				.GetTypeSystemOrNull(DecompilerTypeSystem.GetOptions(MainWindow.Instance.CurrentDecompilerSettings));
		}

		public static void SelectItem(this DataGrid view, object item)
		{
			var container = (DataGridRow)view.ItemContainerGenerator.ContainerFromItem(item);
			if (container != null)
				container.IsSelected = true;
			view.Focus();
		}

		public static double ToGray(this Color? color)
		{
			return color?.R * 0.3 + color?.G * 0.6 + color?.B * 0.1 ?? 0.0;
		}
	}
}
