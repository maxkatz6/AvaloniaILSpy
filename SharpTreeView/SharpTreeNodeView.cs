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

using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia.VisualTree;

namespace ICSharpCode.TreeView
{
	public class SharpTreeNodeView : TemplatedControl
	{
		public static readonly StyledProperty<IBrush> TextBackgroundProperty =
			AvaloniaProperty.Register<SharpTreeNodeView, IBrush>(nameof(TextBackground));

		public IBrush TextBackground
		{
			get { return GetValue(TextBackgroundProperty); }
			set { SetValue(TextBackgroundProperty, value); }
		}

		public static readonly DirectProperty<SharpTreeNodeView, object> IconProperty =
			AvaloniaProperty.RegisterDirect<SharpTreeNodeView, object>(nameof(Icon), (owner) => owner.Node);

		public object Icon
		{
			get {
				var expanded = Node?.IsExpanded;
				if (!expanded.HasValue) {
					return null;
				}
				return expanded.Value ? Node?.ExpandedIcon : Node?.Icon;
			}
		}


		public SharpTreeNode Node
		{
			get { return DataContext as SharpTreeNode; }
		}

		public SharpTreeViewItem ParentItem { get; private set; }
		
		public static readonly StyledProperty<Control> CellEditorProperty =
			AvaloniaProperty.Register<SharpTreeNodeView, Control>("CellEditor");
		
		public Control CellEditor {
			get { return (Control)GetValue(CellEditorProperty); }
			set { SetValue(CellEditorProperty, value); }
		}

		public SharpTreeView ParentTreeView {
			get { return ParentItem.ParentTreeView; }
		}

		internal LinesRenderer LinesRenderer { get; private set; }
		private Border textEditorContainer;
		private Control spacer;
		private ToggleButton expander;

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);
			LinesRenderer = e.NameScope.Get<LinesRenderer>("linesRenderer");
			textEditorContainer = e.NameScope.Get<Border>("textEditorContainer");
			textEditorContainer = e.NameScope.Get<Border>("textEditorContainer");
			spacer = e.NameScope.Get<Control>("spacer");
			expander = e.NameScope.Get<ToggleButton>("expander");

			UpdateTemplate();
		}

		protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);
			ParentItem = this.FindAncestorOfType<SharpTreeViewItem>()!;
			ParentItem.NodeView = this;
		}

		protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == DataContextProperty)
			{
				UpdateDataContext(e.OldValue as SharpTreeNode, e.NewValue as SharpTreeNode);
			}
		}

		void UpdateDataContext(SharpTreeNode oldNode, SharpTreeNode newNode)
		{
			if (newNode != null)
			{
				newNode.PropertyChanged += Node_PropertyChanged;
				if (Template != null)
				{
					UpdateTemplate();
				}
			}
			if (oldNode != null)
			{
				oldNode.PropertyChanged -= Node_PropertyChanged;
			}
		}

		void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsEditing")
			{
				OnIsEditingChanged();
			}
			else if (e.PropertyName == "IsLast")
			{
				if (ParentTreeView.ShowLines)
				{
					foreach (var child in Node.VisibleDescendantsAndSelf())
					{
						var container = ParentTreeView.ContainerFromItem(child) as SharpTreeViewItem;
						if (container != null && container.NodeView != null)
						{
							container.NodeView.LinesRenderer.InvalidateVisual();
						}
					}
				}
			}
			else if (e.PropertyName == "IsExpanded")
			{
				if (Node.IsExpanded)
					ParentTreeView.HandleExpanding(Node);
			}
		}

		void OnIsEditingChanged()
		{
			if (Node.IsEditing)
			{
				if (CellEditor == null)
					textEditorContainer.Child = new EditTextBox() { Item = ParentItem };
				else
					textEditorContainer.Child = CellEditor;
			}
			else
			{
				textEditorContainer.Child = null;
			}
		}

		void UpdateTemplate()
		{
			spacer.Width = CalculateIndent();

			if (ParentTreeView.Root == Node && !ParentTreeView.ShowRootExpander)
			{
				expander.IsVisible = false;
			}
			else
			{
				expander.ClearValue(IsVisibleProperty);
			}
		}

		internal double CalculateIndent()
		{
			var result = 19 * Node.Level;
			if (ParentTreeView.ShowRoot)
			{
				if (!ParentTreeView.ShowRootExpander)
				{
					if (ParentTreeView.Root != Node)
					{
						result -= 15;
					}
				}
			}
			else
			{
				result -= 19;
			}
			if (result < 0)
			{
				Debug.WriteLine("Negative indent level detected for node " + Node);
				return 0;
			}
			return result;
		}
	}
}
