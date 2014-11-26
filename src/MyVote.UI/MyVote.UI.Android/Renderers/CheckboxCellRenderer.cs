using Android.Content;
using Android.Views;
using MyVote.UI.Controls;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using NativeCheckBox = Android.Widget.CheckBox;
using View = Android.Views.View;

namespace MyVote.UI.Renderers
{
    public class CheckboxCellRenderer : CellRenderer
    {
        private NativeCheckBox view;
        
        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case "Text":
                {
                    this.view.Text = ((CheckboxCell)this.Cell).Text;
                    break;
                }
                case "Checked":
                {
                    this.view.Checked = ((CheckboxCell)this.Cell).Checked;
                    break;
                }
            }
        }

        protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
        {
            if ((this.view = convertView as NativeCheckBox) == null)
            {
                this.view = new NativeCheckBox(context);
            }
            this.view.Checked = ((CheckboxCell)item).Checked;
            this.view.Text = ((CheckboxCell)item).Text;
            this.view.LayoutParameters.Width = -1;
            this.view.Click += ViewOnClick;
            return base.GetCellCore(item, convertView, parent, context);
        }

        private void ViewOnClick(object sender, EventArgs eventArgs)
        {
            ((CheckboxCell)this.Cell).Checked = this.view.Checked;
        }
    }
}