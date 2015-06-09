using MyVote.UI.Helpers;
using MyVote.UI.Services;
using System.Threading.Tasks;

#if __MOBILE__
using Xamarin.Forms;
#else
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;
#endif

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
#if __MOBILE__
                this.PollImage = new Image();
                var source = ImageSource.FromStream(() => this.UploadViewModel.PictureStream);
                this.PollImage.Source = source;
#else
				this.PollImage = new BitmapImage();
				this.PollImage.SetSource(this.UploadViewModel.PictureStream);
#endif
			}
		}

		public override async Task<string> UploadImage()
		{
			await azureStorageService.UploadPicture(UploadViewModel);

			return string.Format("{0}{1}", PollImageViewModel.PollPicturesUrlBase, UploadViewModel.ImageIdentifier);
		}
	}
}