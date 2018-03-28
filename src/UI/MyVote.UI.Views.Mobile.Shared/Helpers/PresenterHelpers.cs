using Autofac;
using System;
using System.Linq;

using Xamarin.Forms;
using MyVote.UI.ViewModels;
using System.Threading.Tasks;

namespace MyVote.UI.Helpers
{
    public static class PresenterHelpers
    {

		public static Page CreatePage(ViewModelRequest request, ILogger logger)
		{
			var mappings = Ioc.Container.Resolve<IVmPageMappings>();
			var result = mappings.Mappings.SingleOrDefault(m => m.Key == request.ViewModelType);

			if (result.Key != request.ViewModelType)
			{
                logger.Information($"Page not found for {request.ViewModelType.Name}", request.ViewModelType.Name);
			    return null;
			}

            var page = Activator.CreateInstance(result.Value) as Page;
            if (page == null)
            {
                logger.Information($"Failed to create ContentPage {result.Value.Name}", result.Value.Name);
            }
            return page;
		}
    }
}