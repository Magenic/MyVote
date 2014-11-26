// NOTE: Created from Cheesebaron's MVVM Cross/Xamarin.Forms example at:
// https://github.com/Cheesebaron/Xam.Forms.Mvx

using Autofac;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using MyVote.UI.Helpers;
using System;
using System.Linq;
using Xamarin.Forms;

namespace MyVote.UI
{
    public static class MvxPresenterHelpers
    {
        public static IMvxViewModel LoadViewModel(MvxViewModelRequest request)
        {
            var viewModelLoader = Mvx.Resolve<IMvxViewModelLoader>();
            var viewModel = viewModelLoader.LoadViewModel(request, null);
            return viewModel;
        }

        public static ContentPage CreatePage(MvxViewModelRequest request)
        {
            var mappings = AutofacInject.Container.Resolve<IVmPageMappings>();
            var result = mappings.Mapings.SingleOrDefault(m => m.Key == request.ViewModelType);

            if (result.Key != request.ViewModelType)
            {
                Mvx.Trace("Page not found for {0}", request.ViewModelType.Name);
                return null;
            }

            var page = Activator.CreateInstance(result.Value) as ContentPage;
            if (page == null)
            {
                Mvx.Error("Failed to create ContentPage {0}", result.Value.Name);
            }
            return page;
        }
    }
}