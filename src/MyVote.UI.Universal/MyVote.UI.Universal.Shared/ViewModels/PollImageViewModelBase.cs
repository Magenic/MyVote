using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE
using System.Windows.Media.Imaging;
#else
using Windows.UI.Xaml.Media.Imaging;
#endif // WINDOWS_PHONE

namespace MyVote.UI.ViewModels
{
	public abstract class PollImageViewModelBase : Screen
	{
		public const string PollPicturesUrlBase = "http://myvotestorage.blob.core.windows.net/pollpictures/";

		public abstract Task AddImage();
		public abstract Task<string> UploadImage();

		public bool HasImage
		{
			get { return this.UploadViewModel != null; }
		}

		protected UploadViewModel UploadViewModel { get; set; }

		private BitmapImage pollImage;
		public BitmapImage PollImage
		{
			get { return this.pollImage; }
			set
			{
				pollImage = value;
				NotifyOfPropertyChange(() => this.PollImage);
			}
		}
	}
}
