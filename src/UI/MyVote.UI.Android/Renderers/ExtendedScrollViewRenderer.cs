// Correction for scrollview issue as given at: 
// https://forums.xamarin.com/discussion/20834/horizontal-scrollview-within-vertical-scrollview#latest
using System;
using Android.Views;
using MyVote.UI.Controls;
using MyVote.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedScrollView), typeof(ExtendedScrollViewRenderer))]
namespace MyVote.UI.Renderers
{
	public sealed class ExtendedScrollViewRenderer : ScrollViewRenderer
	{
		float StartX, StartY;
		int IsHorizontal = -1;

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);
			if (((ScrollView)e.NewElement).Orientation == ScrollOrientation.Horizontal) IsHorizontal = 1;

		}
		public override bool DispatchTouchEvent(Android.Views.MotionEvent e)
		{
			switch (e.Action)
			{
				case MotionEventActions.Down:
					StartX = e.RawX;
					StartY = e.RawY;
					this.Parent.RequestDisallowInterceptTouchEvent(true);
					break;
				case MotionEventActions.Move:
					if (IsHorizontal * Math.Abs(StartX - e.RawX) < IsHorizontal * Math.Abs(StartY - e.RawY))
						this.Parent.RequestDisallowInterceptTouchEvent(false);
					break;
				case MotionEventActions.Up:
					this.Parent.RequestDisallowInterceptTouchEvent(false);
					break;
			}
			return base.DispatchTouchEvent(e);
		}
	}
}