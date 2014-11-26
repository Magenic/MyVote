using System;
using Android.Graphics;
using Android.Widget;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedImage), typeof(ExtendedImageRenderer))]
namespace MyVote.UI.Renderers
{
    public class ExtendedImageRenderer : ViewRenderer<ExtendedImage, ImageView>
    {
        protected async override void OnElementChanged(ElementChangedEventArgs<ExtendedImage> e)
        {
            try
            {
                base.OnElementChanged(e);
                if (e.OldElement == null)
                    this.SetNativeControl(new ImageView(this.Context));
                using (var sourceBitmap = await this.GetBitmapAsync(this.Element.Source))
                {
                    this.ReshapeImage(sourceBitmap);
                    sourceBitmap.Recycle();
                }
                this.UpdateAspect();
            }
            catch (Java.Lang.OutOfMemoryError ex)
            {
                //We are loading a lot of images and the UI does not
                // control for it.  The resizing is sometimes causing an
                // out of memory error.  This code will have to be
                // optimized.  See the following article:
                // http://developer.xamarin.com/recipes/android/resources/general/load_large_bitmaps_efficiently/
            }
            catch (Exception ex)
            {
                // todo: See above, even worse error sometimes comes accross 
                // as undefined so no way to catch it with a specific
                // error as above.  We need to refactor this to stop
                // the image errors from happening in the first place.
            }
        }

        protected async override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Image.SourceProperty.PropertyName)
            {
                using (var sourceBitmap = await this.GetBitmapAsync(this.Element.Source))
                {
                    this.ReshapeImage(sourceBitmap);
                    sourceBitmap.Recycle();
                }
            }
            else
            {
                if (!(e.PropertyName == Image.AspectProperty.PropertyName))
                    return;
                this.UpdateAspect();
            }
        }

        //private void ControlOnLayoutChange(object sender, LayoutChangeEventArgs layoutChangeEventArgs)
        //{
        //    if (layoutChangeEventArgs.Bottom - layoutChangeEventArgs.Top > 0
        //        && layoutChangeEventArgs.Right - layoutChangeEventArgs.Left > 0)
        //    {
        //        this.ReshapeImage(this.Control);
        //    }
        //}

        private void ReshapeImage(Bitmap sourceBitmap)
        {
            //May need some scaling code
            if (sourceBitmap != null)
            {
                var sourceRect = GetScaledRect(sourceBitmap);
                var rect = this.GetTargetRect(sourceBitmap);
                using (var output = Bitmap.CreateBitmap(rect.Width(), rect.Height(), Bitmap.Config.Argb8888))
                {
                    var canvas = new Canvas(output);

                    var paint = new Paint();
                    var rectF = new RectF(rect);
                    var roundPx = rect.Width() / 2;

                    paint.AntiAlias = true;
                    canvas.DrawARGB(0, 0, 0, 0);
                    paint.Color = Android.Graphics.Color.ParseColor("#ff424242");
                    canvas.DrawRoundRect(rectF, roundPx, roundPx, paint);

                    paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
                    canvas.DrawBitmap(sourceBitmap, sourceRect, rect, paint);

                    this.Control.SetImageBitmap(output);
                    // Internally forces the internal method of InvalidateMeasure to be called.
                    this.Element.WidthRequest = this.Element.WidthRequest;
                }
            }
        }

        private Rect GetScaledRect(Bitmap sourceBitmap)
        {
            int height = 0;
            int width = 0;
            int top = 0;
            int left = 0;

            switch (this.Element.Aspect)
            {
                case Aspect.AspectFill:
                    height = sourceBitmap.Height;
                    width = sourceBitmap.Width;
                    height = this.MakeSquare(height, ref width);
                    left = (int)((sourceBitmap.Width - width) / 2);
                    top = (int)((sourceBitmap.Height - height) / 2);
                    break;
                case Aspect.Fill:
                    height = sourceBitmap.Height;
                    width = sourceBitmap.Width;
                    height = this.MakeSquare(height, ref width);
                    break;
                default:
                    height = sourceBitmap.Height;
                    width = sourceBitmap.Width;
                    height = this.MakeSquare(height, ref width);
                    left = (int)((sourceBitmap.Width - width) / 2);
                    top = (int)((sourceBitmap.Height - height) / 2);
                    break;
            }

            var rect = new Rect(left, top, width + left, height + top);

            return rect;
        }

        private int MakeSquare(int height, ref int width)
        {
            if (height < width)
            {
                width = height;
            }
            else
            {
                height = width;
            }
            return height;
        }

        private Rect GetTargetRect(Bitmap sourceBitmap)
        {
            int height = 0;
            int width = 0;

            height = this.Element.HeightRequest > 0
                       ? (int)System.Math.Round(this.Element.HeightRequest, 0)
                       : sourceBitmap.Height; 
            width = this.Element.WidthRequest > 0
                       ? (int)System.Math.Round(this.Element.WidthRequest, 0)
                       : sourceBitmap.Width; 

            // Make Square
            if (height < width)
            {
                width = height;
            }
            else
            {
                height = width;
            }

            var rect = new Rect(0, 0, width, height);

            return rect;
        }

        private void UpdateAspect()
        {
            using (ImageView.ScaleType scaleType = this.ToScaleType(this.Element.Aspect))
                this.Control.SetScaleType(scaleType);
            this.Control.RequestLayout();
        }


        private ImageView.ScaleType ToScaleType(Aspect aspect)
        {
            switch (aspect)
            {
                case Aspect.AspectFill:
                    return ImageView.ScaleType.CenterCrop;
                case Aspect.Fill:
                    return ImageView.ScaleType.FitXy;
                default:
                    return ImageView.ScaleType.FitCenter;
            }
        }

        private async Task<Bitmap> GetBitmapAsync(ImageSource source) 
        { 
             var handler = GetHandler(source);
             TaskAwaiter<Bitmap> taskAwaiter = new TaskAwaiter<Bitmap>();
             var returnValue = await handler.LoadImageAsync(source, this.Context, new CancellationToken()); 
             return returnValue; 
         }

        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
        }
    }
}