using MyVote.UI.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace MyVote.UI.Controls
{
	public class ExtendedPicker<T> : Picker
	{
        public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(ExtendedPicker<T>), string.Empty);
        public string PlaceholderText
        {
            get { return (string)this.GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(ExtendedPicker<T>), Color.Transparent);
        public Color PlaceholderColor
        {
            get { return (Color)this.GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }

        public ExtendedPicker()
		{
			this.SelectedIndexChanged += OnSelectedIndexChanged;
		}

        public static BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(ObservableCollection<SelectOptionViewModel<T>>), typeof(ExtendedPicker<T>), default(ObservableCollection<SelectOptionViewModel<T>>), propertyChanged: OnItemsSourceChanged);

        public static BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(T), typeof(ExtendedPicker<T>), default(T), propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ExtendedDatePicker), Color.Black);

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

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
		{
            var picker = (ExtendedPicker<T>)bindable;
			var bindingList = newValue as ObservableCollection<SelectOptionViewModel<T>>;
			picker.Items.Clear();
			if (newValue != null && bindingList != null)
			{
				//now it works like "subscribe once" but you can improve
				foreach (var item in bindingList)
				{
					picker.Items.Add(item.Display);
				}

                OnSelectedItemChanged(bindable, picker.SelectedItem, picker.SelectedItem);
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

		private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var picker = bindable as ExtendedPicker<T>;
			if (newValue != null && picker != null && picker.ItemsSource != null)
			{
				var item = picker.ItemsSource.FirstOrDefault(i => i.Value.Equals(newValue));
				if (item != null)
				{
					picker.SelectedIndex = picker.Items.IndexOf(item.Display);
				}
			}
		}
	}
}