using System;
using System.IO;

namespace MyVote.UI.ViewModels
{
    public class UploadViewModel
    {
        private string imageIdentifier;
        public string ImageIdentifier
        {
            get { return this.imageIdentifier; }
            set
            {
                this.imageIdentifier = value;
            }
        }

        private Stream pictureStream;
        public Stream PictureStream
        {
            get { return this.pictureStream; }
            set
            {
                this.pictureStream = value;
            }
        }
    }
}
