using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;

using MyVote.UI.Contracts;
#if WINDOWS_PHONE
using System.Windows.Media.Imaging;
#elif __MOBILE__
using Xamarin.Forms;
#else
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace MyVote.UI.ViewModels
{
    public abstract class PollImageViewModelBase : MvxViewModel
    {
        public const string PollPicturesUrlBase = "http://myvotestorage.blob.core.windows.net/pollpictures/";

        public abstract Task AddImage();
        public abstract Task<string> UploadImage();

        public bool HasImage
        {
            get { return this.UploadViewModel != null; }
        }

        protected UploadViewModel UploadViewModel { get; set; }

#if __MOBILE__
        private Image pollImage;
        public Image PollImage
#else
        private BitmapImage pollImage;
        public BitmapImage PollImage
#endif
        {
            get { return this.pollImage; }
            set
            {
                pollImage = value;
                this.RaisePropertyChanged(() => this.PollImage);
            }
        }
    }
}