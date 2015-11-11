using Autofac;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using System;
using System.Linq;

namespace MyVote.UI.Helpers
{
    public static class MvxPresenterHelpers
    {
		public static IMvxViewModel LoadViewModel(MvxViewModelRequest request)
		{
			var viewModelLoader = Mvx.Resolve<IMvxViewModelLoader>();
			var viewModel = viewModelLoader.LoadViewModel(request, null);
			return viewModel;
		}

		public static T CreatePage<T>(MvxViewModelRequest request) where T : class
		{
			var mappings = AutofacInject.Container.Resolve<IVmPageMappings>();
			var result = mappings.Mappings.SingleOrDefault(m => m.Key == request.ViewModelType);

			if (result.Key != request.ViewModelType)
			{
				Mvx.Trace("Page not found for {0}", request.ViewModelType.Name);
				return default(T);
			}

			var page = Activator.CreateInstance(result.Value) as T;
			if (page == null)
			{
				Mvx.Error("Failed to create ContentPage {0}", result.Value.Name);
			}
			return page;
		}
    }
}
