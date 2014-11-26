using MyVote.UI.Helpers;
using MyVote.UI.Services;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;

namespace MyVote.UI.ViewModels
{
	public sealed class PollImageViewModel : PollImageViewModelBase
	{
		private readonly IAzureStorageService azureStorageService;
		private readonly IPhotoChooser photoChooser;

		public PollImageViewModel(
			IAzureStorageService azureStorageService,
			IPhotoChooser photoChooser)
		{
			this.azureStorageService = azureStorageService;
			this.photoChooser = photoChooser;
		}

		public override async Task AddImage()
		{
			this.UploadViewModel = await this.photoChooser.ShowChooser();

			if (this.UploadViewModel != null)
			{
				this.PollImage = new BitmapImage();
				this.PollImage.SetSource(this.UploadViewModel.PictureStream);
			}
		}

		public override async Task<string> UploadImage()
		{
			await azureStorageService.UploadPicture(UploadViewModel);

			return string.Format("{0}{1}", PollImageViewModel.PollPicturesUrlBase, UploadViewModel.ImageIdentifier);
		}
	}
}
