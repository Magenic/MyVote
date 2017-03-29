using System;
using System.Collections.ObjectModel;
using Csla.Xaml;
using MyVote.UI.ViewModels;
using Xamarin.Forms;

namespace MyVote.UI.Controls
{
    public class ExtendedPropertyInfo : PropertyInfo
    {
        public ExtendedPropertyInfo()
        {
        }

        public static readonly BindableProperty ExtraInfoProperty = BindableProperty.Create(nameof(ExtraInfo), typeof(object), typeof(ExtendedPropertyInfo), null);
        public object ExtraInfo
        {
            get { return GetValue(ExtraInfoProperty); }
            set
            {
                SetValue(ExtraInfoProperty, value);
            }
        }
    }
}
