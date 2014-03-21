`BarChart` displays data as an interactive bar chart.  It supports iOS
and Android, and has customizable labels, legends, and other appearance
properties.

## Examples

### Adding a `BarChart` to your iOS app:

```csharp
using BarChart;
...

public override void ViewDidLoad ()
{
  base.ViewDidLoad ();  

  var data = new [] { 1f, 2f, 4f, 8f, 16f, 32f };
  var chart = new BarChartView {
    Frame = View.Frame,
    ItemsSource = Array.ConvertAll (data, v => new BarModel { Value = v })
  };

  View.AddSubview (chart);
}
```

### Adding a `BarChart` to your Android app:

```csharp
using BarChart;
...

protected override void OnCreate (Bundle bundle)
{
  base.OnCreate (bundle);
  
  var data = new [] { 1f, 2f, 4f, 8f, 16f, 32f };
  var chart = new BarChartView (this) {
    ItemsSource = Array.ConvertAll (data, v => new BarModel { Value = v })
  };

  AddContentView (chart, new ViewGroup.LayoutParams (
    ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
}
```

### Events and Customization

By subscribing to `BarClick`, you can be notified when the user
touches a bar:

```csharp
chart.BarClick += (sender, args) => {
  BarModel bar = args.Bar;
  Console.WriteLine ("Pressed {0}", bar);
};
```  

Chart values are limited by the minimum and maximum values in the
`ItemsSource`. Alternatively, you can set minimum and maximum values
manually:

```csharp
chart.MinimumValue = 5;
chart.MaximumValue = 8;
```  

To return to default behavior (automatic fitting):

```csharp
chart.MinimumValue = null;
chart.MaximumValue = null;
```    

You can mix automatic and manual fitting:

```csharp
chart.MinimumValue = -2;
chart.MaximumValue = null;
```    

Y-axis tick marks are placed automatically at 1/4 intervals in the range
of `Value` in your `ItemsSource`. To set place tick marks manually:

```csharp
chart.AutoLevelsEnabled = false;
chart.AddLevelIndicator (0, title: "zero");
chart.AddLevelIndicator (5);
```

Resetting `AutoLevelsEnabled` to true removes all custom tick marks and
reverts to the default behavior.

Other customizable appearance properties:

```csharp
chart.GridHidden = true;
chart.LegendHidden = true;

chart.BarWidth = 40;
chart.BarOffset = 2;
```  

Changing bar and caption colors:

```csharp
//iOS
chart.BarColor = UIColor.Green;
chart.BarCaptionInnerColor = UIColor.White;
chart.BarCaptionInnerShadowColor = UIColor.Black;
chart.BarCaptionOuterColor = UIColor.Black;
chart.BarCaptionOuterShadowColor = UIColor.White;

//Android
chart.BarColor = Android.Graphics.Color.Green;
chart.BarCaptionInnerColor = Android.Graphics.Color.White;
chart.BarCaptionOuterColor = Android.Graphics.Color.Black;
```  

You may also set `BarModel` appearance properties on an individual
basis:

```csharp
var bar = new BarModel {
  Value = 100500,
  Color = UIColor.Green,
  Legend = "Unit Sales",
  ValueCaptionHidden = false,
  ValueCaption = "100k"
};
```

### Adding BarChart to AXML Layouts

Or using axml layout:

```xml
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android" ... >

  <barchart.BarChartView
    android:id="@+id/barChart"
    android:layout_width="fill_parent"
    android:layout_height="fill_parent"
    min_value="5"
    max_value="8"
    bar_width="40"
    bar_offset="2"
    bar_color="#FF0000"
    bar_caption_fontSize="30"
    bar_caption_innerColor="#000000"
    bar_caption_outerColor="#FFFFFF" />

</LinearLayout>
```
