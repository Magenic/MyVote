using System.IO;

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
				this.RaisePropertyChanged(() => this.ImageIdentifier);
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
				this.RaisePropertyChanged(() => this.PictureStream);
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
                this.RaisePropertyChanged(() => this.PictureFile);
			}
        }

		private Windows.Storage.Streams.IRandomAccessStream pictureStream;
		public Windows.Storage.Streams.IRandomAccessStream PictureStream
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
