using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyVote.UI.Helpers
{
    public sealed class ListViewItemClick
    {
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.RegisterAttached("MethodName", typeof(string), typeof(ListViewItemClick), new PropertyMetadata(null, MethodNamePropertyChanged));

		public static void SetMethodName(DependencyObject attached, string methodName)
		{
			if (attached == null)
			{
				throw new ArgumentNullException("attached");
			}
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}

			attached.SetValue(MethodNameProperty, methodName);
		}

		public static string GetMethodName(DependencyObject attached)
		{
			if (attached == null)
			{
				throw new ArgumentNullException("attached");
			}

			return (string)attached.GetValue(MethodNameProperty);
		}

		private static void MethodNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}

			((ListViewBase)d).ItemClick += OnItemClick;
		}

		private static void OnItemClick(object sender, ItemClickEventArgs e)
		{
			if (sender == null)
			{
				throw new ArgumentNullException("sender");
			}
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}

			var listView = (ListViewBase)sender;
			var methodName = GetMethodName(listView);

			if (!ExecuteMethod(e, e.ClickedItem, methodName))
			{
				ExecuteMethod(e, listView.DataContext, methodName);
			}
		}

		private static bool ExecuteMethod(ItemClickEventArgs e, object target, string methodName)
		{
			var method = target.GetType().GetTypeInfo().GetDeclaredMethod(methodName);

			if (method != null)
			{
				var parms = method.GetParameters();

				if (parms != null && parms.Length == 1)
				{
					method.Invoke(target, new[] { e.ClickedItem });
				}
				else
				{
					method.Invoke(target, null);
				}
				return true;
			}
			else
			{
				var property = target.GetType().GetTypeInfo().GetDeclaredProperty(methodName);

				if (property != null && property.PropertyType == typeof(ICommand))
				{
					var command = (ICommand)property.GetValue(target);
					command.Execute(e.ClickedItem);

					return true;
				}
			}

			return false;
		}
    }
}
