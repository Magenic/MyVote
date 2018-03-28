using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MyVote.UI.Controls
{
	public sealed class RepeaterView<T> : StackLayout
	{
		public RepeaterView()
		{
			this.Spacing = 0;
		}

		public ObservableCollection<T> ItemsSource
		{
			get { return (ObservableCollection<T>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(ObservableCollection<T>), typeof(RepeaterView<T>), new ObservableCollection<T>(), BindingMode.OneWay, null, (BindableObject bindable, object oldValue, object newValue) => ItemsChanged(bindable, oldValue, newValue));

		private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
		{
            var control = (RepeaterView<T>)bindable;
			control.ItemsSource.CollectionChanged += control.ItemsSource_CollectionChanged;
			control.Children.Clear();

            var newCollection = (ObservableCollection<T>)newValue;
            foreach (var item in newCollection)
			{
				var cell = control.ItemTemplate.CreateContent();
				var view = ((ViewCell)cell).View;
				view.BindingContext = item;
				control.Children.Add(view);
			}
            control.UpdateChildrenLayout();
            control.InvalidateLayout();
		}

		void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				this.Children.RemoveAt(e.OldStartingIndex);
				this.UpdateChildrenLayout();
				this.InvalidateLayout();
			}

			if (e.NewItems != null)
			{
				foreach (T item in e.NewItems)
				{
					var cell = this.ItemTemplate.CreateContent();
					var view = ((ViewCell)cell).View;
					view.BindingContext = item;
					this.Children.Insert(ItemsSource.IndexOf(item), view);
				}
			}
		}

		public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(RepeaterView<T>), default(DataTemplate));

		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
		}
	}
}