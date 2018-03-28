using MyVote.UI.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace MyVote.UI.Views
{
    public class TabbedPageBase : TabbedPage
    {
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var context = this.BindingContext as NavigatingViewModelBase;
            if (context != null)
            {
                context.PropertyChanged += BindingContextOnPropertyChanged;
            }
        }

        private void BindingContextOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var context = this.BindingContext as NavigatingViewModelBase;
            if (context != null)
            {
                if (propertyChangedEventArgs.PropertyName == "IsBusy")
                {
                    this.IsBusy = context.IsBusy;
                }
                if (propertyChangedEventArgs.PropertyName == "IsEnabled")
                {
                    this.IsEnabled = context.IsEnabled;
                }
            }
        }

        public bool IsLandscape()
        {
            return this.Width > this.Height;
        }
    }
}
