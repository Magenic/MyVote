using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

using BarChart;

namespace BarChart.Android.Sample {
	[Activity (Label = "BarChart.Android.Sample", MainLauncher = true)]
	public class Activity1 : Activity {
		static readonly BarModel[] TestData = new BarModel[] {
			new BarModel () { Value =   -1f, Legend = "0", Color = Color.Red },
			new BarModel () { Value =    2f, Legend = "1" },
			new BarModel () { Value =    0f, Legend = "2" },
			new BarModel () { Value =    1f, Legend = "3" },
			new BarModel () { Value =   -1f, Legend = "4", Color = Color.Red },
			new BarModel () { Value =    1f, Legend = "5" },
			new BarModel () { Value =   -1f, Legend = "6", Color = Color.Red },
			new BarModel () { Value =    2f, Legend = "7" },
			new BarModel () { Value = -0.1f, Legend = "8", Color = Color.Red }
		};

        BarChartView _BarChart;

        static readonly BarModel[] TestData2 = new BarModel[] {
            new BarModel () { Value =   -1f, Legend = "0", Color = Color.Red },
            new BarModel () { Value =    2f, Legend = "1" },
            new BarModel () { Value =    0f, Legend = "2" },
            new BarModel () { Value =    1f, Legend = "3" },
            new BarModel () { Value =   -1f, Legend = "4", Color = Color.Red },
            new BarModel () { Value =    1f, Legend = "5" },
            new BarModel () { Value =   -1f, Legend = "6", Color = Color.Red },
            new BarModel () { Value =    2f, Legend = "7" },
            new BarModel () { Value = 1f, Legend = "8", Color = Color.Red }
        };

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			_BarChart = FindViewById<BarChartView> (Resource.Id.barChart1);

			/* Also you can add bar chart manually, if you want
			 * var chart = new BarChartView(this);
			 * var layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);
			 * layoutParams.SetMargins (20, 20, 20, 20);
			 * chart.LayoutParameters = layoutParams;
			 * chart.MinimumValue = -2;
			 * chart.MaximumValue = 2;
			 * FindViewById<LinearLayout>(Resource.Id.rootLayout).AddView(chart);
			 */

			_BarChart.BarClick += HandleBarClick;
            _BarChart.ItemsSource = TestData;
		}

		void HandleBarClick (object sender, BarClickEventArgs e)
		{
			Console.WriteLine ("Bar {0} with value {1}", e.Bar.Legend, e.Bar.ValueCaption);

            _BarChart.ItemsSource = TestData2;
            _BarChart.PostInvalidate();
		}

	}
}


