using System.IO;
using Cirrious.MvvmCross.ViewModels;

namespace MyVote.UI.ViewModels
{
    public class UploadViewModel : MvxViewModel
    {
        private string imageIdentifier;
        public string ImageIdentifier
        {
            get { return this.imageIdentifier; }
            set
			{
				this.imageIdentifier = value;
                this.RaisePropertyChanged(() => this.ImageIdentifier);
			}
        }

#if WINDOWS_PHONE || __MOBILE__
		private Stream pictureStream;
		public Stream PictureStream
		{
			get { return this.pictureStream; }
			set
			{
				this.pictureStream = value;
                this.RaisePropertyChanged(() => this.PictureStream);
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
                this.RaisePropertyChanged(() => this.PictureFile);
			}
        }

		private IRandomAccessStream pictureStream;
		public IRandomAccessStream PictureStream
		{
			get { return this.pictureStream; }
			set
			{
				this.pictureStream = value;
                this.RaisePropertyChanged(() => this.PictureStream);
			}
		}
#endif // WINDOWS_PHONE
    }
}
