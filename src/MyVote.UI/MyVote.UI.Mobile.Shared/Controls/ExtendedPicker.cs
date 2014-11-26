using System;
using System.Collections.ObjectModel;
using System.Linq;
using MyVote.UI.ViewModels;
using Xamarin.Forms;

namespace MyVote.UI.Controls
{
    public class ExtendedPicker<T> : Picker
    {
        public ExtendedPicker()
        {
            this.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create<ExtendedPicker<T>, ObservableCollection<SelectOptionViewModel<T>>>(
                o => o.ItemsSource,
                default(ObservableCollection<SelectOptionViewModel<T>>),
                propertyChanged: OnItemsSourceChanged);

        public static BindableProperty SelectedItemProperty =
            BindableProperty.Create<ExtendedPicker<T>, T>(
                o => o.SelectedItem,
                default(T),
                propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty TextColorProperty = 
            BindableProperty.Create<ExtendedDatePicker, Color>(
                p => p.TextColor, Color.Black);

        public ObservableCollection<SelectOptionViewModel<T>> ItemsSource
        {
            get
            {
                return (ObservableCollection<SelectOptionViewModel<T>>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public T SelectedItem
        {
            get
            {
                return (T)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, ObservableCollection<SelectOptionViewModel<T>> oldvalue, ObservableCollection<SelectOptionViewModel<T>> newvalue)
        {
            var picker = bindable as ExtendedPicker<T>;
            var bindingList = newvalue as ObservableCollection<SelectOptionViewModel<T>>;
            picker.Items.Clear();
            if (newvalue != null && bindingList != null)
            {
                //now it works like "subscribe once" but you can improve
                foreach (var item in bindingList)
                {
                    picker.Items.Add(item.Display);
                }
            }
        }

        public Color TextColor
        {
            get { return (Color)this.GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            
            if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
            {
                SelectedItem = default(T);
            }
            else
            {
                var selectedItem = this.ItemsSource.FirstOrDefault(i => i.Display == Items[SelectedIndex]);
                if (selectedItem != null)
                {
                    SelectedItem = selectedItem.Value;                    
                }
            }
        }

        private static void OnSelectedItemChanged(BindableObject bindable, T oldvalue, T newvalue)
        {
            var picker = bindable as ExtendedPicker<T>;
            if (newvalue != null && picker != null && picker.ItemsSource != null)
            {
                var item = picker.ItemsSource.FirstOrDefault(i => i.Value.Equals(newvalue));
                if (item != null)
                {
                    picker.SelectedIndex = picker.Items.IndexOf(item.Display);                    
                }
            }
        }
    }
}