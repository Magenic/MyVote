using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyVote.UI.Views
{
	public class ResizablePage : Page
	{
		protected const string FullScreenLandscapeState = "FullScreenLandscape";
		protected const string NarrowState = "Narrow";
		protected const string FullScreenPortraitState = "FullScreenPortrait";
		protected const string FilledState = "Filled";

		public ResizablePage()
		{
			this.Loaded += delegate
			{
                UpdateVisualState();
				Window.Current.SizeChanged += Current_SizeChanged;
			};

			this.Unloaded += delegate
			{
				Window.Current.SizeChanged -= Current_SizeChanged;
			};
		}

		private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
		{
            UpdateVisualState();
		}

        private void UpdateVisualState()
        {
            VisualStateManager.GoToState(this, DetermineVisualState(), false);
        }

		protected virtual string DetermineVisualState()
		{
			var visualState = string.Empty;
            var applicationView = ApplicationView.GetForCurrentView();
			var windowWidth = Window.Current.Bounds.Width;

            if (applicationView.IsFullScreen)
            {
                if (applicationView.Orientation == ApplicationViewOrientation.Landscape)
                    visualState = FullScreenLandscapeState;
                else
                    visualState = FullScreenPortraitState;
            }
            else
            {
                if (windowWidth <= 500)
                    visualState = NarrowState;
                else
                    visualState = FilledState;
            }

			return visualState;
		}
	}
}
