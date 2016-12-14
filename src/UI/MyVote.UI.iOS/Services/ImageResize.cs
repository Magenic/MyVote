using System;
using System.Drawing;
using CoreGraphics;
using UIKit;
using System.IO;

namespace MyVote.UI.Services
{
    public class ImageResize : IImageResize
    {
        public Stream ResizeImage(Stream imageStream, float maxWidth, float maxHeight)
        {
            byte[] bytes;

            using (BinaryReader br = new BinaryReader(imageStream))
            {
                bytes = br.ReadBytes((int)imageStream.Length);
            }
            var originalImage = ImageFromByteArray(bytes);
            var orientation = originalImage.Orientation;
            var sourceSize = originalImage.Size;
            var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);

            var width = maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;

            UIGraphics.BeginImageContextWithOptions(new CGSize((float)width, (float)height), true, 1.0f);
            originalImage.Draw(new CGRect(0, 0, (float)width, (float)height));

            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            var returnBytes = resultImage.AsJPEG().ToArray();
            return new MemoryStream(returnBytes);
        }


        public UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIImage image;
            image = new UIImage(Foundation.NSData.FromArray(data));
            return image;
        }
    }
}
