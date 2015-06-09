using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Views;
using Cirrious.MvvmCross.WindowsCommon.Views;
using Cirrious.MvvmCross.WindowsCommon.Views.Suspension;
using MyVote.UI.ViewModels;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyVote.UI.Views
{
	public class ResizablePage : Page, IMvxWindowsView
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

		protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			this.OnViewCreate(e.Parameter as MvxViewModelRequest, () => this.LoadStateBundle(e));
		}

		protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);

			var bundle = this.CreateSaveStateBundle();
			this.SaveStateBundle(e, bundle);
		}

		protected virtual IMvxBundle LoadStateBundle(NavigationEventArgs e)
		{
			// nothing loaded by default
			var frameState = this.SuspensionManager.SessionStateForFrame(this.WrappedFrame);
			this.PageKey = "Page-" + this.Frame.BackStackDepth;
			IMvxBundle bundle = null;

			if (e.NavigationMode == NavigationMode.New)
			{
				// Clear existing state for forward navigation when adding a new page to the
				// navigation stack
				var nextPageKey = this.PageKey;
				int nextPageIndex = this.Frame.BackStackDepth;
				while (frameState.Remove(nextPageKey))
				{
					nextPageIndex++;
					nextPageKey = "Page-" + nextPageIndex;
				}
			}
			else
			{
				var dictionary = (IDictionary<string, string>)frameState[this.PageKey];
				bundle = new MvxBundle(dictionary);
			}

			return bundle;
		}

		protected virtual void SaveStateBundle(NavigationEventArgs navigationEventArgs, IMvxBundle bundle)
		{
			var frameState = this.SuspensionManager.SessionStateForFrame(this.WrappedFrame);
			frameState[this.PageKey] = bundle.Data;
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

		public void ClearBackStack()
		{
			throw new System.NotImplementedException();
		}

		private string PageKey { get; set; }

		IMvxViewModel IMvxView.ViewModel
		{
			get { return this.ViewModel; }
			set { this.ViewModel = value as ViewModelBase; }
		}

		private ViewModelBase viewModel;
		public ViewModelBase ViewModel
		{
			get { return this.viewModel; }
			set
			{
				if (this.viewModel == value)
				{
					return;
				}

				this.viewModel = value;
				this.DataContext = value;
			}
		}

		public IMvxWindowsFrame WrappedFrame
		{
			get { return new MvxWrappedFrame(this.Frame); }
		}

		private IMvxSuspensionManager suspensionManager;
		protected IMvxSuspensionManager SuspensionManager
		{
			get
			{
				suspensionManager = suspensionManager ?? Mvx.Resolve<IMvxSuspensionManager>();
				return suspensionManager;
			}
		}

		void IMvxWindowsView.ClearBackStack()
		{
			throw new System.NotImplementedException();
		}
	}
}
