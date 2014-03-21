using Microsoft.Phone.Tasks;
using MyVote.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.Helpers
{
	public sealed class PhotoChooser : IPhotoChooser
	{
		private readonly PhotoChooserTask photoChooserTask;

		public PhotoChooser()
		{
			this.photoChooserTask = new PhotoChooserTask();
		}

		public async Task<UploadViewModel> ShowChooser()
		{
			return await Task.Run(() =>
				{
					var task = new TaskCompletionSource<UploadViewModel>();

					EventHandler<PhotoResult> completedHandler = null;
					completedHandler = (sender, e) =>
						 {
							 this.photoChooserTask.Completed -= completedHandler;

							 if (e.Error != null)
							 {
								 task.SetException(e.Error);
							 }
							 else
							 {
								 UploadViewModel uploadViewModel = null;
								 if (e.ChosenPhoto != null)
								 {
									 uploadViewModel = new UploadViewModel
									 {
										 PictureStream = e.ChosenPhoto,
										 ImageIdentifier = string.Format("{0}.{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(e.OriginalFileName))
									 };
								 }

								 task.SetResult(uploadViewModel);
							 }
						 };
					this.photoChooserTask.Completed += completedHandler;
					this.photoChooserTask.Show();

					return task.Task;
				});
		}
	}
}
