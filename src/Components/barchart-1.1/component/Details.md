`BarChart` displays data as an interactive bar chart.  It supports iOS
and Android, and has customizable labels, legends, and other appearance
properties.

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

*Screenshot assemble with [PlaceIt](http://placeit.breezi.com/).*
