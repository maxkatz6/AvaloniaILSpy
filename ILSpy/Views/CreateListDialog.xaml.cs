
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Interaction logic for Create.xaml
	/// </summary>
	public partial class CreateListDialog : Window
	{
		public CreateListDialog(string title)
		{
			InitializeComponent();
			this.Title = title;
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			okButton.IsEnabled = !string.IsNullOrWhiteSpace(ListNameBox.Text);
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(ListNameBox.Text))
			{
				Close(true);
			}
		}

		public string ListName {
			get => ListNameBox.Text;
			set => ListNameBox.Text = value;
		}
	}
}