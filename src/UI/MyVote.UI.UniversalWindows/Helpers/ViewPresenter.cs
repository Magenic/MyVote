using MyVote.UI.Contracts;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Autofac;

namespace MyVote.UI.Helpers
{
	public class ViewPresenter : IViewPresenter
	{
		private Frame Frame { get; set; }

		public ViewPresenter(Frame frame)
		{
			Frame = frame;
		}

		public void ChangePresentation(object hint)
		{
			if (hint is ClearBackstackHint)
			{
				while (Frame.CanGoBack)
				{
					Frame.GoBack();
				}
			}
		}

		public void Close()
		{
			Frame.GoBack();
		}

		public Task ShowAsync(ViewModelRequest request)
		{
			var viewType = ResolveViewType(request.ViewModelType);
			var viewModel = Ioc.Container.Resolve<IViewModelLoader>().LoadViewModel(request.ViewModelType, request.Parameters);

			Frame.Navigate(viewType, viewModel);
			/*if (request.Parameters != null)
			{
				Frame.Navigate(viewType, request.Parameters);//this.serializer.Serialize(parameter));
			}
			else
			{
				Frame.Navigate(viewType);
			}*/

			return Task.FromResult(false);
		}

		private static Type ResolveViewType(Type viewModelType)
		{
			var viewName = viewModelType.AssemblyQualifiedName.Replace(
				viewModelType.Name,
				viewModelType.Name.Replace("ViewModel", string.Empty));

			return Type.GetType(viewName.Replace("Model", string.Empty));
		}
	}
}
