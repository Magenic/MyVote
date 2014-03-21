using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyVote.UI.Services;
using System.Threading.Tasks;
using MyVote.UI.Helpers;
using MyVote.UI.ViewModels;
using Android.Graphics;
using System.IO;

namespace MyVote.UI.Droid
{
	public class AndroidPollImageViewModel
	{


		private readonly IAzureStorageService azureStorageService;

		public const string PollPicturesUrlBase = "http://myvotestorage.blob.core.windows.net/pollpictures/";

		public AndroidPollImageViewModel(
			IAzureStorageService azureStorageService
			/*IPhotoChooser photoChooser*/)
		{
			this.azureStorageService = azureStorageService;
			//this.photoChooser = photoChooser;
            this.UploadViewModel = new UploadViewModel();
		}


		public bool HasImage
		{
			get { return this.UploadViewModel != null; }
		}

		protected UploadViewModel UploadViewModel { get; set; }

        private Bitmap pollImage { get; set; }
		
		private readonly IPhotoChooser photoChooser;

		public async Task<string> UploadImage(Bitmap bitmap)
		{
            var uploadVM = new UploadViewModel();

            var stream = new MemoryStream();

            bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
            uploadVM.PictureStream = stream;
            uploadVM.ImageIdentifier = Guid.NewGuid() + ".bmp";

			await azureStorageService.UploadPicture(uploadVM);

            return string.Format("{0}{1}", AndroidPollImageViewModel.PollPicturesUrlBase, uploadVM.ImageIdentifier);
		}
	}

	
}

