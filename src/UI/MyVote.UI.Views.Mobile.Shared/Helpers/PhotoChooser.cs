using System.IO;
using MyVote.UI.ViewModels;
using System;
using System.Threading.Tasks;
#if __MOBILE__
using Xamarin;
#endif

namespace MyVote.UI.Helpers
{
    public sealed class PhotoChooser : IPhotoChooser
    {
		public async Task<UploadViewModel> ShowChooser()
		{
			UploadViewModel uploadViewModel = null;
#if ANDROID
		    var uiContext = new Services.UiContext();            
            var mediaPicker = new Xamarin.Media.MediaPicker(uiContext.CurrentContext);
#elif IOS
            var mediaPicker = new Xamarin.Media.MediaPicker();
#endif
#if __MOBILE__
            try
            {
                var mediaFile = await mediaPicker.PickPhotoAsync().ConfigureAwait(false);

                if (mediaFile != null)
                {
                    uploadViewModel = new UploadViewModel();
                    uploadViewModel.PictureStream = mediaFile.GetStream();
                    uploadViewModel.ImageIdentifier = Guid.NewGuid() + Path.GetExtension(mediaFile.Path);
                }
            }
			catch (TaskCanceledException ex)
			{
				// Happens if they don't select an image and press the 
				// back button. Just continue as it is an expected
				// exception.
				Insights.Report(ex);
			}
#else
            var openPicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.PicturesLibrary };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");
            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                uploadViewModel = new UploadViewModel();
                uploadViewModel.PictureFile = file;
                uploadViewModel.PictureStream = await file.OpenReadAsync();
                uploadViewModel.ImageIdentifier = Guid.NewGuid() + file.FileType;
            }
#endif
			return uploadViewModel;
		}
    }
}
