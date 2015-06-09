using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyVote.UI.Controls
{
	public sealed class ResultBar : Control
	{
		public ResultBar()
		{
			this.DefaultStyleKey = typeof(ResultBar);
		}


		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly DependencyProperty TotalResultCountProperty =
			DependencyProperty.Register("TotalResultCount", typeof(int), typeof(ResultBar), new PropertyMetadata(0));

		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly DependencyProperty ItemResultCountProperty =
			DependencyProperty.Register("ItemResultCount", typeof(int), typeof(ResultBar), new PropertyMetadata(0));


		public int TotalResultCount
		{
			get { return (int)GetValue(TotalResultCountProperty); }
			set { SetValue(TotalResultCountProperty, value); }
		}

		public int ItemResultCount
		{
			get { return (int)GetValue(ItemResultCountProperty); }
			set { SetValue(ItemResultCountProperty, value); }
		}

	}
}
