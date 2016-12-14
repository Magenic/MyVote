using System;
using System.IO;
using Android.Graphics;
using Android.Util;

namespace MyVote.UI.Services
{
    public class ImageResize : IImageResize
    {

        public Stream ResizeImage(Stream imageStream, float maxWidth, float maxHeight)
        {
            
            var originalImage = BitmapFactory.DecodeStream(imageStream);
            var sourceSize = new Size((int)originalImage.GetBitmapInfo().Width, (int)originalImage.GetBitmapInfo().Height);

            var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);

            var width = (int)(maxResizeFactor * sourceSize.Width);
            var height = (int)(maxResizeFactor * sourceSize.Height);

            using (var bitmapScaled = Bitmap.CreateScaledBitmap(originalImage, width, height, true))
            {
                var ms = new MemoryStream();
                bitmapScaled.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                bitmapScaled.Recycle();
                originalImage.Recycle();
                var bytes = ms.ToArray();
                return new MemoryStream(bytes);
            }

        }
    }
}
