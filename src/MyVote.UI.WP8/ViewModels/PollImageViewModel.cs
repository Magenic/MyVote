using MyVote.UI.Helpers;
using MyVote.UI.Services;
using System.Threading.Tasks;

namespace MyVote.UI.ViewModels
{
	public sealed class PollImageViewModel : PollImageViewModelBase
	{
		private readonly IPhotoChooser photoChooser;
		private readonly IAzureStorageService azureStorageService;

		public PollImageViewModel(
			IPhotoChooser photoChooser,
			IAzureStorageService azureStorageService)
		{
			this.photoChooser = photoChooser;
			this.azureStorageService = azureStorageService;
		}

		public override async Task AddImage()
		{
			this.UploadViewModel = await this.photoChooser.ShowChooser();

			this.PollImage = new System.Windows.Media.Imaging.BitmapImage();
			this.PollImage.SetSource(this.UploadViewModel.PictureStream);
		}

		public override async Task<string> UploadImage()
		{
			await this.azureStorageService.UploadPicture(this.UploadViewModel);

			return string.Format("{0}{1}", PollImageViewModel.PollPicturesUrlBase, UploadViewModel.ImageIdentifier);
		}
	}
}
