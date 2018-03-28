using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyVote.UI.ViewModels
{
	public abstract class ViewModelBase : IViewModel, INotifyPropertyChanged
    {
        public virtual void Init(object parameter) { }

        public virtual void Start() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}