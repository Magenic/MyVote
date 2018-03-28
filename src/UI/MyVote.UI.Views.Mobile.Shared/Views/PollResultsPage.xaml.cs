using Xamarin.Forms;

namespace MyVote.UI.Views
{
	public partial class PollResultsPage : ContentPageBase
	{
		public PollResultsPage()
		{
            InitializeComponent();
            pollImage.PropertyChanged += PollImage_PropertyChanged;

		}


        protected override void OnAppearing()
        {
            base.OnAppearing();
            pollImage.PropertyChanged -= PollImage_PropertyChanged;
            pollImage.PropertyChanged += PollImage_PropertyChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            pollImage.PropertyChanged -= PollImage_PropertyChanged;
        }

        void PollImage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var image = (Image)sender;
            if (e.PropertyName == nameof(Image.IsLoading) && image.Source != null)
            {
                image.FadeTo(1, 2000, Easing.SinIn);
            }
        }
    }
}