// Copyright (c) 2020 AlphaSierraPapa for the SharpDevelop Team
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.Diagnostics;

using Avalonia.Media.Immutable;

namespace ICSharpCode.TreeView
{
	class LinesRenderer : Control
	{
		static LinesRenderer()
		{
			pen = new ImmutablePen(Brushes.LightGray, 1);
		}

		static ImmutablePen pen;

		SharpTreeNodeView NodeView {
			get { return TemplatedParent as SharpTreeNodeView; }
		}

		public override void Render(DrawingContext dc)
		{
			if (NodeView.Node == null)
			{
				// This seems to happen sometimes with DataContext==DisconnectedItem,
				// though I'm not sure why WPF would call OnRender() on a disconnected node
				Debug.WriteLine($"LinesRenderer.OnRender() called with DataContext={NodeView.DataContext}");
				return;
			}
			var indent = NodeView.CalculateIndent();
			var p = new Point(indent + 4.5, 0);

			if (!NodeView.Node.IsRoot || NodeView.ParentTreeView.ShowRootExpander)
			{
				dc.DrawLine(pen, new Point(p.X, Bounds.Height / 2), new Point(p.X + 10, Bounds.Height / 2));
			}

			if (NodeView.Node.IsRoot)
				return;

			if (NodeView.Node.IsLast)
			{
				dc.DrawLine(pen, p, new Point(p.X, Bounds.Height / 2));
			}
			else
			{
				dc.DrawLine(pen, p, new Point(p.X, Bounds.Height));
			}

			var current = NodeView.Node;
			while (true)
			{
				p = p.WithX(p.X - 19);
				current = current.Parent;
				if (p.X < 0)
					break;
				if (!current.IsLast)
				{
					dc.DrawLine(pen, p, new Point(p.X, Bounds.Height));
				}
			}
		}
	}
}
