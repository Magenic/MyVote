using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace MyVote.UI.ViewModels
{
	// Navigation criteria serialization based on: http://stackoverflow.com/questions/19058173/passing-complex-navigation-parameters-with-mvvmcross-showviewmodel
	public abstract class ViewModelBase : MvxViewModel
    {
		protected void ShowViewModel<TViewModel>(object parameter)
			where TViewModel : IMvxViewModel
		{
			var json = Mvx.Resolve<IMvxJsonConverter>().SerializeObject(parameter);
			base.ShowViewModel<TViewModel>(new Dictionary<string, string>()
            {
                {"parameter", json}
            });
		}

		public ICommand GoBack
		{
			get { return new MvxCommand(() => this.Close(this)); }
		}

		private bool isBusy;
		public bool IsBusy
		{
			get { return this.isBusy; }
			set
			{
				this.isBusy = value;
				this.RaisePropertyChanged(() => this.IsBusy);
			}
		}

        private bool isEnabled;
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                this.RaisePropertyChanged(() => this.IsEnabled);
            }
        }
    }

	public abstract class ViewModelBase<TCriteria>
		: ViewModelBase
	{
		public void Init(string parameter)
		{
			if (!string.IsNullOrEmpty(parameter))
			{
				var deserialized = Mvx.Resolve<IMvxJsonConverter>().DeserializeObject<TCriteria>(parameter);
				this.RealInit(deserialized);
			}
			else
			{
				this.RealInit(default(TCriteria));
			}
		}

		public abstract void RealInit(TCriteria parameter);
	}
}
