using Cirrious.CrossCore.Exceptions;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Views;
using UIKit;
using Xamarin.Forms;

namespace MyVote.UI.Helpers
{
    public class MvxFormsViewPresenter
        : MvxBaseTouchViewPresenter
    {
        private readonly IUIApplicationDelegate _applicationDelegate;
        private readonly UIWindow _window;

        public virtual INavigation MasterNavigationController { get; protected set; }

        protected virtual IUIApplicationDelegate ApplicationDelegate
        {
            get { return _applicationDelegate; }
        }

        protected virtual UIWindow Window
        {
            get { return _window; }
        }

        public MvxFormsViewPresenter(IUIApplicationDelegate applicationDelegate, UIWindow window)
        {
            _applicationDelegate = applicationDelegate;
            _window = window;
        }

        public override void Show(MvxViewModelRequest request)
        {
            var view = this.CreateViewControllerFor(request);

        }

        public override void ChangePresentation(MvxPresentationHint hint)
        {
            if (hint is MvxClosePresentationHint)
            {
                return;
            }

            base.ChangePresentation(hint);
        }

        public override bool PresentModalViewController(UIViewController viewController, bool animated)
        {
            return true;
        }

        public override void NativeModalViewControllerDisappearedOnItsOwn()
        {
            // ignored
        }
    }
}