using MyVote.UI.Enums;
using Xamarin.Forms;

namespace MyVote.UI.Controls
{
    /// <summary>
    /// Creates a button with text and an image.
    /// The image can be on the left, above, on the right or below the text.
    /// </summary>
    public class ImageButton : Button
    {
        /// <summary>
        /// Backing field for the Image property.
        /// </summary>
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(ImageButton), (ImageSource)null, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate)null, (BindableProperty.BindingPropertyChangedDelegate)((bindable, oldvalue, newvalue) => ((VisualElement)bindable).ToString()), (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null);

        /// <summary>
        /// Gets or sets the ImageSource to use with the control.
        /// </summary>
        /// <value>
        /// The Source property gets/sets the value of the backing field, SourceProperty.
        /// </value>
        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Backing field for the orientation property.
        /// </summary>
        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(ImageOrientation), typeof(ImageButton), ImageOrientation.ImageToLeft);

        /// <summary>
        /// Gets or sets The orientation of the image relative to the text.
        /// </summary> 
        /// <value>
        /// The Orientation property gets/sets the value of the backing field, OrientationProperty.
        /// </value> 
        public ImageOrientation Orientation
        {
            get { return (ImageOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Backing field for the image height property.
        /// </summary>
        public static readonly BindableProperty ImageHeightRequestProperty = BindableProperty.Create(nameof(ImageHeightRequest), typeof(int), typeof(ImageButton), default(int));

        /// <summary>
        /// Gets or sets the requested height of the image.  If less than or equal to zero than a 
        /// height of 50 will be used.
        /// </summary>
        /// <value>
        /// The ImageHeightRequest property gets/sets the value of the backing field, ImageHeightRequestProperty.
        /// </value> 
        public int ImageHeightRequest
        {
            get { return (int)GetValue(ImageHeightRequestProperty); }
            set { SetValue(ImageHeightRequestProperty, value); }
        }

        /// <summary>
        /// Backing field for the image width property.
        /// </summary>
        public static readonly BindableProperty ImageWidthRequestProperty = BindableProperty.Create(nameof(ImageWidthRequest), typeof(int), typeof(ImageButton), default(int));

        /// <summary>
        /// Gets or sets the requested width of the image.  If less than or equal to zero than a 
        /// width of 50 will be used.
        /// </summary>
        /// <value>
        /// The ImageHeightRequest property gets/sets the value of the backing field, ImageHeightRequestProperty.
        /// </value> 
        public int ImageWidthRequest
        {
            get { return (int)GetValue(ImageWidthRequestProperty); }
            set { SetValue(ImageWidthRequestProperty, value); }
        }
    }
}