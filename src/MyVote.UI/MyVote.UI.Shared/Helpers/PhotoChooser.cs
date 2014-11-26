using MyVote.UI.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin;
#if !__MOBILE__
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
#elif __ANDROID__
using Xamarin.Forms.Labs.Services.Media;
using Xamarin.Forms.Labs.Droid.Services.Media;
#elif __IOS__
using Xamarin.Forms.Labs.Services.Media;
using Xamarin.Forms.Labs.iOS.Services.Media;
#endif

namespace MyVote.UI.Helpers
{
    public sealed class PhotoChooser : IPhotoChooser
    {
        public async Task<UploadViewModel> ShowChooser()
        {
            UploadViewModel uploadViewModel = null;
#if __MOBILE__
            var madiaPicker = new MediaPicker();

            try
            {
                var mediaFile = await madiaPicker.SelectPhotoAsync(new CameraMediaStorageOptions
                {
                    DefaultCamera = CameraDevice.Front,
                    MaxPixelDimension = 400
                });

                if (mediaFile != null)
                {
                    uploadViewModel = new UploadViewModel();
                    uploadViewModel.PictureStream = mediaFile.Source;
                    uploadViewModel.ImageIdentifier = Guid.NewGuid() + mediaFile.Exif.FileName;
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