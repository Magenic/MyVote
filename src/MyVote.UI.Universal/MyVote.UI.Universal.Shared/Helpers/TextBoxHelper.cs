using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyVote.UI.Helpers
{
	public sealed class TextBoxHelper
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly DependencyProperty BindableTextProperty = DependencyProperty.RegisterAttached(
			 "BindableText",
			 typeof(string),
			 typeof(TextBoxHelper),
			 new PropertyMetadata(null, new PropertyChangedCallback((sender, args) =>
			 {
				 var textBox = sender as TextBox;
				 if (textBox != null)
				 {
					 textBox.Text = (string)args.NewValue;
					 if (textBox.Tag == null)
					 {
						 textBox.Tag = new TextBoxHelper(textBox);
					 }
				 }
			 }))
		   );

		public TextBoxHelper(TextBox textBox)
		{
			textBox.TextChanged += (o, a) =>
			{
				textBox.SetValue(BindableTextProperty, textBox.Text);
			};
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void SetBindableText(TextBox element, string value)
		{
			if (element != null)
			{
				element.SetValue(BindableTextProperty, value);
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static string GetBindableText(TextBox element)
		{
			if (element != null)
			{
				return (string)element.GetValue(BindableTextProperty);
			}

			return null;
		}
	}
}
