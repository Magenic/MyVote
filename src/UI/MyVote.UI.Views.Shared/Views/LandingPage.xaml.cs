using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyVote.UI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LandingPage : ResizablePage
    {
        public LandingPage()
        {
            this.InitializeComponent();

			this.Loaded += LandingPage_Loaded;
        }

		void LandingPage_Loaded(object sender, RoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "DoneLoading", true);

			if (this.DataContext != null)
			{
				this.ViewModel.PropertyChanged += viewModel_PropertyChanged;

				UpdateState();
			}
		}

		void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsBusy")
				UpdateState();
		}

		private void UpdateState()
		{
			var isSnapped = DetermineVisualState() == NarrowState;

			if (this.ViewModel == null)
				return;

			if (this.ViewModel.IsBusy)
				VisualStateManager.GoToState(this, string.Format("{0}Busy", isSnapped ? NarrowState : string.Empty), true);
			else
				VisualStateManager.GoToState(this, string.Format("{0}Idle", isSnapped ? NarrowState : string.Empty), true);
		}
    }
}
