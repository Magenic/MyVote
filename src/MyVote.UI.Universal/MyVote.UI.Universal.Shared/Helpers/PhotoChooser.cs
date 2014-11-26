using MyVote.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;

namespace MyVote.UI.Helpers
{
	public sealed class PhotoChooser : IPhotoChooser
	{
		public async Task<UploadViewModel> ShowChooser()
		{
			UploadViewModel uploadViewModel = null;

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

			return uploadViewModel;
		}
	}
}
