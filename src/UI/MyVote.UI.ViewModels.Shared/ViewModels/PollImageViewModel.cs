using MyVote.UI.Helpers;
using MyVote.UI.Services;
using System.Threading.Tasks;
using System.IO;

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
        private readonly IImageResize imageResize;

		public PollImageViewModel(
			IAzureStorageService azureStorageService,
			IPhotoChooser photoChooser,
            IImageResize imageResize)
		{
			this.azureStorageService = azureStorageService;
			this.photoChooser = photoChooser;
            this.imageResize = imageResize;
		}

		public override async Task AddImage()
		{
			this.UploadViewModel = await this.photoChooser.ShowChooser();

			if (this.UploadViewModel != null)
			{
				// TODO: Fix this for UWP
                //this.UploadViewModel.PictureStream = imageResize.ResizeImage(UploadViewModel.PictureStream, 600, 600);
#if __MOBILE__
                this.PollImage = new Image();
                var newStream = new MemoryStream();
                UploadViewModel.PictureStream.CopyTo(newStream);
                newStream.Position = 0;
                var source = ImageSource.FromStream(() => newStream);
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