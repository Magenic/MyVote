﻿using System;
using System.Threading.Tasks;
using MyVote.UI.Contracts;
using Xamarin.Forms;

namespace MyVote.UI.Helpers
{
    public class ViewPresenter : IViewPresenter
    {
        private readonly IViewModelLoader viewModelLoader;
        private readonly ILogger logger;

        public ViewPresenter(IViewModelLoader viewModelLoader, ILogger logger)
        {
            this.viewModelLoader = viewModelLoader;
            this.logger = logger;
        }

        public async Task ShowAsync(ViewModelRequest request)
        {
            if (await AdaptiveTryShowPage(request))
                return;
            logger.Information($"Skipping request for {request.ViewModelType.Name}", request.ViewModelType.Name);
        }

        private async Task<bool> AdaptiveTryShowPage(ViewModelRequest request)
        {
            var page = PresenterHelpers.CreatePage(request, logger);
            if (page == null)
                return false;

            var viewModel = viewModelLoader.LoadViewModel(request.ViewModelType, request.Parameters);

            var mainPage = VmPageMappings.NavigationPage as NavigationPage;
            page.BindingContext = viewModel;

            try
            {
                await mainPage.PushAsync(page);
            }
            catch (Exception e)
            {
                logger.Log(e);
            }

            return true;
        }

        public void Close()
        {
            var mainPage = VmPageMappings.NavigationPage as NavigationPage;
            mainPage.PopAsync(true);
        }

        public void ChangePresentation(object hint)
        {
            if (hint is ClearBackstackHint)
            {
                var mainPage = VmPageMappings.NavigationPage as NavigationPage;
                if (mainPage != null && mainPage.CurrentPage != null)
                {
                    var navigation = mainPage.CurrentPage.Navigation;
                    for (var i = navigation.NavigationStack.Count - 1; i >= 0; i--)
                    {
                        var page = navigation.NavigationStack[i];
                        if (page != mainPage.CurrentPage)
                        {
                            navigation.RemovePage(page);
                        }
                    }
                }
            }
        }
    }
}
