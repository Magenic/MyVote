using Caliburn.Micro;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace MyVote.UI.ViewModels
{
    public class UploadViewModel : Screen
    {
        private string imageIdentifier;
        public string ImageIdentifier
        {
            get { return this.imageIdentifier; }
            set
			{
				this.imageIdentifier = value;
				this.NotifyOfPropertyChange(() => this.ImageIdentifier);
			}
        }

#if WINDOWS_PHONE || ANDROID
		private Stream pictureStream;
		public Stream PictureStream
		{
			get { return this.pictureStream; }
			set
			{
				this.pictureStream = value;
				this.NotifyOfPropertyChange(() => this.PictureStream);
			}
		}
#else
		private StorageFile pictureFile;
        public StorageFile PictureFile
        {
            get { return this.pictureFile; }
            set
			{ 
				this.pictureFile = value;
				this.NotifyOfPropertyChange(() => this.PictureFile);
			}
        }

		private IRandomAccessStream pictureStream;
		public IRandomAccessStream PictureStream
		{
			get { return this.pictureStream; }
			set
			{
				this.pictureStream = value;
				NotifyOfPropertyChange(() => this.PictureStream);
			}
		}
#endif // WINDOWS_PHONE
    }
}
