using System;
using MyVote.UI.ViewModels;
using System.Threading.Tasks;

namespace MyVote.UI.Helpers
{
	public class ViewModelLoader : IViewModelLoader
	{
		public TViewModel LoadViewModel<TViewModel>(object parameter = null, IBundle savedState = null)
			where TViewModel : IViewModel
		{
			var viewModel = Ioc.Resolve<TViewModel>();

			RunLifecycleEvents(viewModel, parameter, savedState);
			
			return viewModel;
		}

        public IViewModel LoadViewModel(Type viewModelType, object parameter = null, IBundle savedState = null)
        {
            var viewModel = (IViewModel)Ioc.Resolve(viewModelType);

            RunLifecycleEvents(viewModel, parameter, savedState);

            return viewModel;
        }

        private void RunLifecycleEvents(IViewModel viewModel, object parameter = null, IBundle savedState = null)
        {
            viewModel.Init(parameter);

            viewModel.Start();
        }

        private void RunLifecycleEvents<TParameter>(IViewModel viewModel, TParameter parameter, IBundle savedState = null)
        {
            viewModel.Init(parameter);

            viewModel.Start();
        }
    }
}
