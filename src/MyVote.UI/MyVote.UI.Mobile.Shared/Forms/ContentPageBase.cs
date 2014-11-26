using System.ComponentModel;
using MyVote.UI.ViewModels;
using Xamarin.Forms;

namespace MyVote.UI.Forms
{
    public abstract class ContentPageBase : ContentPage
    {
        public static readonly BindableProperty HasBackButtonProperty = BindableProperty.Create<Polls, bool>(p => p.HasBackButton, false);
        public bool HasBackButton
        {
            get { return (bool)this.GetValue(HasBackButtonProperty); }
            set { this.SetValue(HasBackButtonProperty, value); }
        }
        
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var context = this.BindingContext as PageViewModelBase;
            if (context != null)
            {
                context.PropertyChanged += BindingContextOnPropertyChanged;
            }
        }

        private void BindingContextOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "IsBusy")
            {
                var context = this.BindingContext as PageViewModelBase;
                if (context != null)
                {
                    this.IsBusy = context.IsBusy;
                }
            }
        }
    }
}
