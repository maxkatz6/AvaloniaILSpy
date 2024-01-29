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

using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Input;

namespace ICSharpCode.TreeView
{
	public class SharpTreeViewItem : ListBoxItem
	{
		static SharpTreeViewItem()
		{
			DragDrop.DragEnterEvent.AddClassHandler<SharpTreeViewItem>((x, e) => x.OnDragEnter(e));
			DragDrop.DragLeaveEvent.AddClassHandler<SharpTreeViewItem>((x, e) => x.OnDragLeave(e));
			DragDrop.DragOverEvent.AddClassHandler<SharpTreeViewItem>((x, e) => x.OnDragOver(e));
			DragDrop.DropEvent.AddClassHandler<SharpTreeViewItem>((x, e) => x.OnDrop(e));
		}

		public SharpTreeNode Node {
			get { return DataContext as SharpTreeNode; }
		}

		public SharpTreeNodeView NodeView { get; internal set; }
		public SharpTreeView ParentTreeView { get; internal set; }

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.F2:
					if (Node.IsEditable && ParentTreeView != null && ParentTreeView.SelectedItems.Count == 1 && ParentTreeView.SelectedItems[0] == Node)
					{
						Node.IsEditing = true;
						e.Handled = true;
					}
					break;
				case Key.Escape:
					if (Node.IsEditing)
					{
						Node.IsEditing = false;
						e.Handled = true;
					}
					break;
			}
		}

		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new SharpTreeViewItemAutomationPeer(this);
		}

		#region Mouse

		Point startPoint;
		bool wasSelected;
		bool wasDoubleClick;

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			wasSelected = IsSelected;
			if (!IsSelected)
			{
				base.OnPointerPressed(e);
			}

			if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			{
				startPoint = e.GetPosition(this);
				e.Pointer.Capture(this);

				if (e.ClickCount == 2)
				{
					// TODO-Avalonia: Handle Double tapped instead.
					wasDoubleClick = true;
				}
			}
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			if (e.Pointer.Captured == this)
			{
				var currentPoint = e.GetPosition(this);
				const double MinimumDragDistance = 2.0;
				if (Math.Abs(currentPoint.X - startPoint.X) >= MinimumDragDistance ||
					Math.Abs(currentPoint.Y - startPoint.Y) >= MinimumDragDistance)
				{

					var selection = ParentTreeView.GetTopLevelSelection().ToArray();
					if (Node.CanDrag(selection))
					{
						Node.StartDrag(e, this, selection);
					}
				}
			}
			else
			{
				base.OnPointerMoved(e);
			}
		}

		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			if (wasDoubleClick)
			{
				wasDoubleClick = false;
				Node.ActivateItem(e);
				if (!e.Handled)
				{
					if (!Node.IsRoot || ParentTreeView.ShowRootExpander)
					{
						Node.IsExpanded = !Node.IsExpanded;
					}
				}
			}

			if (e.GetCurrentPoint(this).Properties.IsMiddleButtonPressed)
			{
				Node.ActivateItemSecondary(e);
			}

			e.Pointer.Capture(null);
			if (wasSelected)
			{
				base.OnPointerReleased(e);
			}
		}

		#endregion

		#region Drag and Drop

		protected virtual void OnDragEnter(DragEventArgs e)
		{
			ParentTreeView.HandleDragEnter(this, e);
		}

		protected virtual void OnDragOver(DragEventArgs e)
		{
			ParentTreeView.HandleDragOver(this, e);
		}

		protected virtual void OnDrop(DragEventArgs e)
		{
			ParentTreeView.HandleDrop(this, e);
		}

		protected virtual void OnDragLeave(DragEventArgs e)
		{
			ParentTreeView.HandleDragLeave(this, e);
		}

		#endregion
	}
}
