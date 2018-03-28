using System.IO;
using MyVote.UI.Contracts;

namespace MyVote.UI.ViewModels
{
    public sealed class UploadViewModel : ViewModelBase
    {
		private string imageIdentifier;
		public string ImageIdentifier
		{
			get { return this.imageIdentifier; }
			set
			{
				this.imageIdentifier = value;
                this.RaisePropertyChanged(nameof(ImageIdentifier));
			}
		}

#if MOBILE
		private Stream pictureStream;
		public Stream PictureStream
		{
			get { return this.pictureStream; }
			set
			{
				this.pictureStream = value;
                this.RaisePropertyChanged(nameof(PictureStream));
			}
		}
#else
		private Windows.Storage.StorageFile pictureFile;
        public Windows.Storage.StorageFile PictureFile
        {
            get { return this.pictureFile; }
            set
			{ 
				this.pictureFile = value;
                this.RaisePropertyChanged(nameof(PictureFile));
			}
        }

		private Windows.Storage.Streams.IRandomAccessStream pictureStream;
		public Windows.Storage.Streams.IRandomAccessStream PictureStream
		{
			get { return this.pictureStream; }
			set
			{
				this.pictureStream = value;
                this.RaisePropertyChanged(nameof(PictureStream));
			}
		}
#endif // WINDOWS_PHONE
    }
}
