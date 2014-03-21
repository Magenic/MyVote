//
//  Series.h
//
//  Copyright (c) 2012 Infragistcs. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "DataPoint.h"
#import "Enums.h"
#import "Core.h"
#import "Axis.h"

@class IGSeries;
@class IGLegendBase;
@class IGBrush;

/*!
 The IGSeriesDataSource protocol is adopted by an object that mediates the application's data model for an IGSeries object. The data source provides the series object with the information it needs to construct and modify a chart view. 
 
 The required methods of the protocol provide the points to be displayed by the chart view along with the total number of points in the series. There are also optional methods for creating point tooltips. 
 */
@protocol IGSeriesDataSource <NSObject>

///------------------------
///@name Configuring series data source
///------------------------

@required
/** Asks the data source for the total number of points in the series.
 @param series Series requesting the information.
 @return Returns the number of points in the series.
 */
-(int)numberOfPointsInSeries:(IGSeries*)series;

/** Asks the data source for an IGDataPoint to be placed at a given location.
 @param series Series requesting the information.
 @param index Index of the created data point.
 @return Returns an object inheriting IGDataPoint that will be used by the series.
 */
-(IGDataPoint*)series:(IGSeries*)series pointAtIndex:(int)index;

@optional

/** Askds the data source for all IGDataPoint objects. 
 
 This method should only be implemented if your data is already in the IGDataPoint format. Otherwise you should covert your data point by point using the series:pointAtIndex: method.
 
 @param series Series requesting the information.
 @return Returns an array of IGDataPoint objects.
 */
-(NSArray*)allPointsForSeries:(IGSeries*)series;

/* Looks up the point at the corresponding index, and updates its values.
 @param series Series requesting the information.
 @param index Index of the data point that will be updated.
 @return Returns the updated data point.
 */
-(IGDataPoint*)series:(IGSeries*)series updatePointAtIndex:(int)index;

/** Asks the data source for a tooltip that will be used for a point at a specified index.
 @param series Series requesting the information.
 @param index Index of the data point, to which the tooltip will be assigned.
 @return Returns a view that will be used as a tooltip.
 */
-(UIView*)series:(IGSeries*)series tooltipForPointAt:(int)index;
@end


/***** Series *****/

/*!
 IGSeries is the base class for all series objects. It contains common properties, such as dataSource, legend and various brushes. This class should not be allocated.
 */
@interface IGSeries : NSObject
{
    NSMutableArray* _dataPoints;
    int _numberOfPoints;

    NSString *_key;
    NSArray *_dashArray;
    UIView *_toolTip;
}

/** Returns the resolved brush of the series. (read-only)
 A series brush can be set by the series or the chart view. The brush can also be automatically generated. This property returns the resolved brush.
 */
@property (nonatomic, readonly) IGBrush *actualBrush;

/** Returns the resovled outline of the series. (read-only)
 A series outline can be set by the series or the chart view. The outline can also be automatically generated. This property returns the resolved outline.
 */
@property (nonatomic, readonly) IGBrush *actualOutline;

/** Returns the resolved legend of the series. (read-only)
 A series legend can be set by the series or the chart view. This property returns the resolved legend.
 */
@property (nonatomic, readonly) IGLegendBase *actualLegend;

/** A brush used by the series.
 This property is used to set a brush for this series. This property takes precedence over chart view's brushes array and themes.
 */
@property (nonatomic, retain) IGBrush *brush;

/** A brush used as an outline by the series.
 This propety is used to set an outline for this series. This property takes precedence over chart view's outlines array and themes.
 */
@property (nonatomic, retain) IGBrush *outline;

/** Specifies the data source used by the series.
 This property represents the data source used by the series. A series must provide a data source in order to display shapes. Different series types has special requirements for their data sources. Chart view has several data source helper classes that can create valid data sources for different series. This could also be a custom data source that conforms to IGSeriesDataSource protocol.
 */
@property (nonatomic, assign) id<IGSeriesDataSource> dataSource;

/** Returns an array of data points used by the series. (read-only)
 */
@property (nonatomic, readonly) NSMutableArray *dataPoints;

/** A string value used as the series identifier.
 */
@property (nonatomic, readonly) NSString *key;

/** A string that represents the series title.
 This property is used by the series to create title text.
 */
@property (nonatomic, retain) NSString *title;

/** A value that determines the series rendering resolution.
 This property is used to determine which points to omit when displaying high amounts of data. The default value is 1. The higher values will result is fewer point being used to draw the series. This will improve the performance at the possible cost of graph's accuracy.
 */
@property (nonatomic) float resolution;

/** A value that determines the border thickness of the series.
 */
@property (nonatomic) float thickness;

/** An enumeration value that determines the stroke dash cap.
 This propery specifies the shape used to draw line caps when the series uses a dashed border.
 */
@property (nonatomic) CGLineCap dashCap;

/** An array of values used to create a dash pattern for the series border.
 This property uses an array of CGFloats to construct a repeating pattern of alternating dashes and spaces.
 */
@property (nonatomic, retain) NSArray *dashArray;

/** A value that determines the length of animation.
 This property is used to determine how long (in seconds) the point animation lasts.
 */
@property (nonatomic) float transitionDuration;

/** Specifies the legend used with the series.
 This property is used to specify a legend for the series. This can be an IGItemLegend to display eevry point in the series, IGLegend to display one legend item per series. If the series is a IGBubbleSeries, then an IGScaleLegend can be used to display a bubble scale.
 */
@property (nonatomic, weak) IGLegendBase *legend;

/** A Boolean value tha determines whether legend items should be visible.
 */
@property (nonatomic) BOOL legendItemIsVisible;

///-----------------------
///@name Initializing series
///-----------------------

/** Initializes the series with a key.
 @param key String identifier of the series.
 @return Returns an initialized series.
 */
-(id)initWithKey:(NSString*)key;
@end



/***** Marker Series *****/
/*!
 IGMarkerSeries is the base class for all series that provide marker support. This class should not be allocated.
 */
@interface IGMarkerSeries : IGSeries

/** An enumeration property that determines the shape of the marker.
 This property is used to set one of the predefined shapes for the markers. When set to IGMarkerTypeAutomatic, a marker will be picked based on the series index.
 */
@property (nonatomic) IGMarkerType markerType;

/** Returns the resolved marker brush of the series. (read-only)
 A series marker brush can be set by the series or the chart view. The brush can also be automatically generated. This property returns the resolved brush.
 */
@property (nonatomic, readonly) IGBrush *actualMarkerBrush;

/** Returns the resovled marker outline of the series. (read-only)
 A series marker outline can be set by the series or the chart view. The outline can also be automatically generated. This property returns the resolved outline.
 */
@property (nonatomic, readonly) IGBrush *actualMarkerOutline;

/** A brush used by the series markers.
 This property is used to set a marker brush for this series. This property takes precedence over chart view's markerBrushes array and themes.
 */
@property (nonatomic, retain) IGBrush *markerBrush;

/** A brush used as a marker outline by the series.
 This propety is used to set a marker outline for this series. This property takes precedence over chart view's markerOutlines array and themes.
 */
@property (nonatomic, retain) IGBrush *markerOutline;

/** A Boolean value that determines whether simpler markers should be used.
 This property is used to reduce the overhead associated with series markers. When set to YES, templated markers will be disabled.
 */
@property (nonatomic) BOOL useLightWeightMarkers;
@end



/***** Anchored Category Series *****/
/*!
 This series is a base class for category series. This class should not be allocated.
 */
@interface IGAnchoredCategorySeries : IGMarkerSeries
{
    NSArray *_trendLineDashArray;
}

/** An enumeration property that determines which trend line to use.
 */
@property (nonatomic) IGTrendLineType trendLineType;

/** Returns the resolved brush for the series trend line. (read-only)
 */
@property (nonatomic, readonly) IGBrush *actualTrendLineBrush;

/** This property determines the brush of the series trend line.
 */ 
@property (nonatomic, retain) IGBrush *trendLineBrush;

/** An array of values used to create a dash pattern for the series trend line.
 This property uses an array of CGFloats to construct a repeating pattern of alternating dashes and spaces.
 */
@property (nonatomic, retain) NSArray *trendLineDashArray;

/** An enumeration value that determines the trend line's dash cap.
 This propery specifies the shape used to draw line caps when the trend line uses a dashed pattern.
 */
@property (nonatomic) CGLineCap trendLineDashCap;

/** A value that determines the moving average period.
 This property only applies to the follwing trend lines: ExponentialAverage, ModifiedAverage, SimpleAverage, WeightedAverage.
 */
@property (nonatomic) int trendLinePeriod;

/** A value that determines the thickness (in pixels) of the trend line.
 */
@property (nonatomic) float trendLineThickness;

/** A value that determines the Z-index of the trend line.
 Values greater than 1000 will result in the trend line being rendered in front of the series data. Values below 1000 will cause the trend line to render behind the series.

@property (nonatomic) int trendLineZIndex; */
@end



/***** Horizontal Anchored Category Series *****/
/*!
 This series type is the base class for horizontal anchored category series. Every such series contains IGNumericYAxis and either IGCategoryXAxis or IGCategoryDateTimeXAxis. This class should not be allocated.
 */
@interface IGHorizontalAnchoredCategorySeries : IGAnchoredCategorySeries
{
    IGCategoryAxisBase *_xAxis;
    IGNumericYAxis *_yAxis;
}

/** Specifies a category axis to be used as the X axis.
 */
@property (nonatomic, retain) IGCategoryAxisBase *xAxis;

/** Specifies a numeric axis to be used as the Y axis.
 */
@property (nonatomic, retain) IGNumericYAxis *yAxis;
@end



/***** Vertical Anchored Category Series *****/
/*!
 This series type is the base class for vertical anchored category series. Every such series contains IGNumericXAxis and IGCategoryYAxis. This class should not be allocated.
 */
@interface IGVerticalAnchoredCategorySeries : IGAnchoredCategorySeries 
{
    IGCategoryYAxis *_yAxis;
    IGNumericXAxis *_xAxis;
}

/** Specifies a category axis to be used as the Y axis.
 */
@property (nonatomic, retain) IGCategoryYAxis *yAxis;

/** Specifies a numeric axis to be used as the X axis.
 */
@property (nonatomic, retain) IGNumericXAxis *xAxis;
@end



/***** Spline Series Base *****/
/*!
 This series type is the base class for spline series. This class should not be allocated.
 */
@interface IGSplineSeriesBase : IGHorizontalAnchoredCategorySeries

/** An enumeration property that determines the shape of the spline.
 The spline can be a natural spline or it can be clamped. A clamped spline does not interpolate beyond its end points.
 */
@property (nonatomic) IGSplineType splineType;
@end



/***** Range Category Series *****/
/*!
 This series type is the base class for range category series. This class should not be allocated.
 */
@interface IGRangeCategorySeries : IGMarkerSeries
@end



/***** Horizontal Range Category Series *****/
/*!
 This series type is the base class for horizontal range category series. This class should not be allocated.
 */
@interface IGHorizontalRangeCategorySeries : IGRangeCategorySeries
{
    IGCategoryAxisBase *_xAxis;
    IGNumericYAxis *_yAxis;
}

/** Specifies the category axis to be used as X axis.
 */
@property (nonatomic, retain) IGCategoryAxisBase *xAxis;

/** Specifies the numeric axis to be used as Y axis.
 */
@property (nonatomic, retain) IGNumericYAxis *yAxis;
@end



/***** Scatter Base *****/
/*!
 This series type is the base class for scatter series. This class should not be allocated.
 */
@interface IGScatterBase : IGMarkerSeries
{
    IGNumericXAxis *_xAxis;
    IGNumericYAxis *_yAxis;
    NSArray *_trendLineDashArray;
}

/** A value that determines the maximum amount of markers displayed by the series.
 */
@property (nonatomic) int maximumMarkers;

/** Specifies the numeric axis used as X axis.
 */
@property (nonatomic, retain) IGNumericXAxis *xAxis;

/** Specifies the numeric axis used as Y axis.
 */
@property (nonatomic, retain) IGNumericYAxis *yAxis;

/** An enumeration property that determines which trend line to use.
 */
@property (nonatomic) IGTrendLineType trendLineType;

/** Returns the resolved brush for the series trend line. (read-only)
 */
@property (nonatomic, readonly) IGBrush *actualTrendLineBrush;

/** This property determines the brush of the series trend line.
 */ 
@property (nonatomic, retain) IGBrush *trendLineBrush;

/** An array of values used to create a dash pattern for the series trend line.
 This property uses an array of CGFloats to construct a repeating pattern of alternating dashes and spaces.
 */
@property (nonatomic, retain) NSArray *trendLineDashArray;

/** An enumeration value that determines the trend line's dash cap.
 This propery specifies the shape used to draw line caps when the trend line uses a dashed pattern.
 */
@property (nonatomic) CGLineCap trendLineDashCap;

/** A value that determines the moving average period.
 This property only applies to the follwing trend lines: ExponentialAverage, ModifiedAverage, SimpleAverage, WeightedAverage.
 */
@property (nonatomic) int trendLinePeriod;

/** A value that determines the thickness (in pixels) of the trend line.
 */
@property (nonatomic) float trendLineThickness;

/** A value that determines the Z-index of the trend line.
 Values greater than 1000 will result in the trend line being rendered in front of the series data. Values below 1000 will cause the trend line to render behind the series.

@property (nonatomic) int trendLineZIndex; */
@end



/***** Polar Base *****/
/*!
 This series type is the base class for polar series. This class should not be allocated.
 */
@interface IGPolarBase : IGMarkerSeries
{
    IGNumericAngleAxis *_angleAxis;
    IGNumericRadiusAxis *_radiusAxis;
    NSArray *_trendLineDashArray;
}

/** Specifies the circular angle axis.
 The angle axis is the circular axis displayed as the outer ring of the series. This axis typically displays numeric angle values.
 */
@property (nonatomic, retain) IGNumericAngleAxis *angleAxis;

/** Specifies the radius axis.
 This axis is the line displayed along the radius of the series. It displays numeric values from the data source.
 */
@property (nonatomic, retain) IGNumericRadiusAxis *radiusAxis;

/** A Boolean value that determines how series shapes cross the polar origin.
 When the series has inner extent, it creates a circular region in the middle. This property determines how series shapes behave when crossing that region. When set to YES, shapes will be clipped to be outside the center. Otherwise, they will be allowed to cross the middle.
 */
@property (nonatomic) BOOL clipSeriesToBounds;

/** A Boolean value that determines whether Cartesian interpolation is used.
 When set to YES, Cartesian interpolation is used. Otherwise, the series uses Archimedian spiral based interpolation.
 */
@property (nonatomic) BOOL useCartesianInterpolation;

/** A value that determines the maximum number of markers displayed by the series.
 */
@property (nonatomic) int maximumMarkers;

/** An enumeration property that determines which trend line to use.
 */
@property (nonatomic) IGTrendLineType trendLineType;

/** Returns the resolved brush for the series trend line. (read-only)
 */
@property (nonatomic, readonly) IGBrush *actualTrendLineBrush;

/** This property determines the brush of the series trend line.
 */ 
@property (nonatomic, retain) IGBrush *trendLineBrush;

/** An array of values used to create a dash pattern for the series trend line.
 This property uses an array of CGFloats to construct a repeating pattern of alternating dashes and spaces.
 */
@property (nonatomic, retain) NSArray *trendLineDashArray;

/** An enumeration value that determines the trend line's dash cap.
 This propery specifies the shape used to draw line caps when the trend line uses a dashed pattern.
 */
@property (nonatomic) CGLineCap trendLineDashCap;

/** A value that determines the moving average period.
 This property only applies to the follwing trend lines: ExponentialAverage, ModifiedAverage, SimpleAverage, WeightedAverage.
 */
@property (nonatomic) int trendLinePeriod;

/** A value that determines the thickness (in pixels) of the trend line.
 */
@property (nonatomic) float trendLineThickness;

/** A value that determines the Z-index of the trend line.
 Values greater than 1000 will result in the trend line being rendered in front of the series data. Values below 1000 will cause the trend line to render behind the series.

@property (nonatomic) int trendLineZIndex; */
@end



/***** Radial Base *****/
/*!
 This series type is the base class for radial series.
 */
@interface IGRadialBase : IGMarkerSeries
{
    IGCategoryAngleAxis *_angleAxis;
    IGNumericRadiusAxis *_valueAxis;
}

/** Specifies the category angle axis.
 This axis is the circular axis displayed as the outer ring of the series. This axis typically displays data point labels.
 */
@property (nonatomic, retain) IGCategoryAngleAxis *angleAxis;

/** Specifies the value axis.
 This axis is the line displayed along the radius of the series. It displays numeric values from the data source.
 */
@property (nonatomic, retain) IGNumericRadiusAxis *valueAxis;

/** A Boolean value that determines how series shapes cross the polar origin.
 When the series has inner extent, it creates a circular region in the middle. This property determines how series shapes behave when crossing that region. When set to YES, shapes will be clipped to be outside the center. Otherwise, they will be allowed to cross the middle.
 */
@property (nonatomic) BOOL clipSeriesToBounds;
@end



/***** Anchored Radial Series *****/
/*!
 This series type is the base class for anchored radial series. This class should not be allocated.
 */
@interface IGAnchoredRadialSeries : IGRadialBase
{
    NSArray *_trendLineDashArray;
}

/** An enumeration property that determines which trend line to use.
 */
@property (nonatomic) IGTrendLineType trendLineType;

/** Returns the resolved brush for the series trend line. (read-only)
 */
@property (nonatomic, readonly) IGBrush *actualTrendLineBrush;

/** This property determines the brush of the series trend line.
 */ 
@property (nonatomic, retain) IGBrush *trendLineBrush;

/** An array of values used to create a dash pattern for the series trend line.
 This property uses an array of CGFloats to construct a repeating pattern of alternating dashes and spaces.
 */
@property (nonatomic, retain) NSArray *trendLineDashArray;

/** An enumeration value that determines the trend line's dash cap.
 This propery specifies the shape used to draw line caps when the trend line uses a dashed pattern.
 */
@property (nonatomic) CGLineCap trendLineDashCap;

/** A value that determines the moving average period.
 This property only applies to the follwing trend lines: ExponentialAverage, ModifiedAverage, SimpleAverage, WeightedAverage.
 */
@property (nonatomic) int trendLinePeriod;

/** A value that determines the thickness (in pixels) of the trend line.
 */
@property (nonatomic) float trendLineThickness;

/** A value that determines the Z-index of the trend line.
 Values greater than 1000 will result in the trend line being rendered in front of the series data. Values below 1000 will cause the trend line to render behind the series.

@property (nonatomic) int trendLineZIndex; */
@end



/***** Financial Series *****/
/*!
 This series type is the base class for financial series. This class should not be allocated.
 */
@interface IGFinancialSeries : IGSeries
{
    IGCategoryAxisBase *_xAxis;
    IGNumericYAxis *_yAxis;
}

/** Specifies the brush used to color the negative sections of data.
 When this property is set, all negative data will be colored with the specified brush.
 */
@property (nonatomic, retain) IGBrush *negativeBrush;

/** Specifies the category axis used as X axis.
 */
@property (nonatomic, retain) IGCategoryAxisBase *xAxis;

/** Specifies the numeric axis used as Y axis.
 */
@property (nonatomic, retain) IGNumericYAxis *yAxis;
@end



/***** Financial Indicator *****/
/*!
 This series type is the base class for financial indicators. This class should not be allocated.
 */
@interface IGFinancialIndicator : IGFinancialSeries
{
    NSArray *_trendLineDashArray;
}

/** An enumeration value that determines which indicator is displayed.
 */
@property (nonatomic) IGIndicatorDisplayType displayType;

/** A value that determines the number of values to hide at the beginning of the indicator.
 */
@property (nonatomic) int ignoreFirst;
/** An enumeration property that determines which trend line to use.
 */
@property (nonatomic) IGTrendLineType trendLineType;

/** Returns the resolved brush for the series trend line. (read-only)
 */
@property (nonatomic, readonly) IGBrush *actualTrendLineBrush;

/** This property determines the brush of the series trend line.
 */ 
@property (nonatomic, retain) IGBrush *trendLineBrush;

/** An array of values used to create a dash pattern for the series trend line.
 This property uses an array of CGFloats to construct a repeating pattern of alternating dashes and spaces.
 */
@property (nonatomic, retain) NSArray *trendLineDashArray;

/** An enumeration value that determines the trend line's dash cap.
 This propery specifies the shape used to draw line caps when the trend line uses a dashed pattern.
 */
@property (nonatomic) CGLineCap trendLineDashCap;

/** A value that determines the moving average period.
 This property only applies to the follwing trend lines: ExponentialAverage, ModifiedAverage, SimpleAverage, WeightedAverage.
 */
@property (nonatomic) int trendLinePeriod;

/** A value that determines the thickness (in pixels) of the trend line.
 */
@property (nonatomic) float trendLineThickness;

/** A value that determines the Z-index of the trend line.
 Values greater than 1000 will result in the trend line being rendered in front of the series data. Values below 1000 will cause the trend line to render behind the series.

@property (nonatomic) int trendLineZIndex; */
@end





/***** Column Series *****/
/*!
 IGColumnSeries displays vertical rectangular columns, with string labels along the X axis and numeric values along the Y axis. This series uses IGCategoryXAxis and IGNumericYAxis. The data source for this series is a collection of single value, single label points. IGCategorySeriesDataSourceHelper can be used as the data source for IGColumnSeries.
 
 Mmultiple series of the IGColumnSeries type that share the same X axis are rendered in clusters where each cluster represents a data point. The first series in the series collection of the IGChartView control renders as a column on the left of the cluster. Each successive series gets rendered to the right of the previous series. However, if series do not share the X axis, they are rendered in layers with each successive series rendered in front of the previous one.
 
 Markers, when enabled, are placed in the middle of the column's top.
 */
@interface IGColumnSeries : IGHorizontalAnchoredCategorySeries
@end


/***** Line Series *****/
/*!
 IGLineSeries displays a line that passes through each of the series data points, with string labels along the X axis and numeric values along the Y axis. This series uses IGCategoryXAxis or IGCategoryDateTimeXAxis and IGNumericYAxis. The data source for this series is a collection of single value, single label points. IGCategorySeriesDataSourceHelper can be used as the data source for IGLineSeries.
 
 Mmultiple series of the IGLineSeries type are handled the same, regardless whether or not they share the same X axis. The line series with the higher index will be drawn in front of the previous series.
 
 Markers, when enabled, are placed at the locations of data points.
 */
@interface IGLineSeries : IGHorizontalAnchoredCategorySeries
{
    IGUnknownValuePlotting _unknownValuePlotting;
}

/** An enumeration value that determines how empty values are handled.
 This property specifies whether null values are treated as zeroes, skipped over or if an interpolation is used.
 */
@property (nonatomic) IGUnknownValuePlotting unknownValuePlotting;
@end


/***** Area Series *****/
/*!
 IGAreaSeries displays a polygon, the outline of which passes through each of the series data points and the X axis, with string labels along the X axis and numeric values along the Y axis. This series uses IGCategoryXAxis or IGCategoryDateTimeXAxis and IGNumericYAxis. The data source for this series is a collection of single value, single label points. IGCategorySeriesDataSourceHelper can be used as the data source for IGAreaSeries.
 
 Mmultiple series of the IGAreaSeries type are handled the same, regardless whether or not they share the same X axis. The area series with the higher index will be drawn in front of the previous series.
 
 Markers, when enabled, are placed at the locations of data points.
 */
@interface IGAreaSeries : IGHorizontalAnchoredCategorySeries
{
    IGUnknownValuePlotting _unknownValuePlotting;
}

/** An enumeration value that determines how empty values are handled.
 This property specifies whether null values are treated as zeroes, skipped over or if an interpolation is used.
 */
@property (nonatomic) IGUnknownValuePlotting unknownValuePlotting;
@end


/***** Bar Series *****/
/*!
 IGBarSeries displays horizontal rectangular bars, with string labels along the Y axis and numeric values along the X axis. This series uses IGCategoryYAxis and IGNumericXAxis. The data source for this series is a collection of single value, single label points. IGCategorySeriesDataSourceHelper can be used as the data source for IGBarSeries.
 
 Mmultiple series of the IGBarSeries type that share the same Y axis are rendered in clusters where each cluster represents a data point. The first series in the series collection of the IGChartView control renders as a bar on the bottom of the cluster. Each successive series gets rendered at the top of the previous series. However, if series do not share the Y axis, they are rendered in layers with each successive series rendered in front of the previous one.
 
 Markers, when enabled, are placed in the middle of the bar's right side.
 */
@interface IGBarSeries : IGVerticalAnchoredCategorySeries
@end


/***** Spline Series *****/
/*!
 IGSplineSeries displays a curved line that passes through each of the series data points, with string labels along the X axis and numeric values along the Y axis. This series uses IGCategoryXAxis or IGCategoryDateTimeXAxis and IGNumericYAxis. The data source for this series is a collection of single value, single label points. IGCategorySeriesDataSourceHelper can be used as the data source for IGSplineSeries.
 
 Mmultiple series of the IGSplineSeries type are handled the same, regardless whether or not they share the same X axis. The line series with the higher index will be drawn in front of the previous series.
 
 Markers, when enabled, are placed at the locations of data points.
 */
@interface IGSplineSeries : IGSplineSeriesBase
@end


/***** SplineArea Series *****/
/*!
 IGSplineAreaSeries displays a curved polygon, the outline of which passes through each of the series data points and the X axis, with string labels along the X axis and numeric values along the Y axis. This series uses IGCategoryXAxis or IGCategoryDateTimeXAxis and IGNumericYAxis. The data source for this series is a collection of single value, single label points. IGCategorySeriesDataSourceHelper can be used as the data source for IGSplineAreaSeries.
 
 Mmultiple series of the IGSplineAreaSeries type are handled the same, regardless whether or not they share the same X axis. The area series with the higher index will be drawn in front of the previous series.
 
 Markers, when enabled, are placed at the locations of data points.
 */
@interface IGSplineAreaSeries : IGSplineSeriesBase
@end


/***** Point Series *****/
/*!
 IGPointSeries displays a marker at each of the series data points, with string labels along the X axis and numeric values along the Y axis. This series uses IGCategoryXAxis or IGCategoryDateTimeXAxis and IGNumericYAxis. The data source for this series is a collection of single value, single label points. IGCategorySeriesDataSourceHelper can be used as the data source for IGPointSeries. Markers are always enabled for this series.
 */
@interface IGPointSeries : IGHorizontalAnchoredCategorySeries
@end


/***** RangeArea Series *****/
/*!
IGRangeAreaSeries belongs to a group of category series and it is rendered using two lines with the area between the lines filled in. This type of series emphasizes the amount of change between low values and high values in the same data point over a period of time or compares multiple items. Range values are represented on the Y axis (IGNumericYAxis) and categories are displayed on the X axis (IGCategoryXAxis or IGCategoryDateTimeXAxis).
 
 IGRangeAreaSeries supports data points with two numeric values and a label. IGHighLowSeriesDataSourceHelper can be used as the data source for IGRangeAreaSeries.
 */
@interface IGRangeAreaSeries : IGHorizontalRangeCategorySeries
@end


/***** RangeColumn Series *****/
/*!
 IGRangeColumnSeries belongs to a group of category series and it is rendered using a collection of vertical columns that show the difference between two values of a data point. This type of series emphasizes the amount of change between low values and high values in the same data point over a period of time or compares multiple items. Range values are represented on the Y axis (IGNumericYAxis) and categories are displayed on the X axis (IGCategoryXAxis). IGHighLowSeriesDataSourceHelper can be used as the data source for IGRangeColumnSeries.
 
 Mmultiple series of the IGRangeColumnSeries type that share the same X axis are rendered in clusters where each cluster represents a data point. The first series in the series collection of the IGChartView control renders as a column on the left of the cluster. Each successive series gets rendered to the right of the previous series. However, if series do not share the X axis, they are rendered in layers with each successive series rendered in front of the previous one.

 */
@interface IGRangeColumnSeries : IGHorizontalRangeCategorySeries
@end


/***** StepArea Series *****/
/*!
 IGStepAreaSeries displays a polygon, the outline of which is a set of connected parallel vertical lines that pass through each of the series data points and the X axis. This series has string labels along the X axis and numeric values along the Y axis. This series uses IGCategoryXAxis or IGCategoryDateTimeXAxis and IGNumericYAxis. The data source for this series is a collection of single value, single label points. IGCategorySeriesDataSourceHelper can be used as the data source for IGStepAreaSeries.
 
 Mmultiple series of the IGStepAreaSeries type are handled the same, regardless whether or not they share the same X axis. The series with the higher index will be drawn in front of the previous series.
 
 Markers, when enabled, are placed at the locations of data points.
 */
@interface IGStepAreaSeries : IGHorizontalAnchoredCategorySeries
@end


/***** StepLine Series *****/
/*!
 IGStepLineSeries displays a set of connected parallel vertical lines that pass through each of the series data points, with string labels along the X axis and numeric values along the Y axis. This series uses IGCategoryXAxis or IGCategoryDateTimeXAxis and IGNumericYAxis. The data source for this series is a collection of single value, single label points. IGCategorySeriesDataSourceHelper can be used as the data source for IGStepLineSeries.
 
 Mmultiple series of the IGStepLineSeries type are handled the same, regardless whether or not they share the same X axis. The series with the higher index will be drawn in front of the previous series.
 
 Markers, when enabled, are placed at the locations of data points.
 */
@interface IGStepLineSeries : IGHorizontalAnchoredCategorySeries
@end


/***** Waterfall Series *****/
/*!
IGWaterfallSeries belongs to a group of category series and it is rendered using a collection of vertical columns that show the difference between consecutive data points. The columns are color coded for distinguishing between positive and negative changes in value. Values are represented on the Y axis (IGNumericYAxis) and categories are displayed on the X axis (IGCategoryXAxis or IGCategoryDateTimeXAxis). The IGWaterfallSeries is similar in appearance to IGRangeColumnSeries but it requires only one numeric value rather than two for each data point.

Mmultiple series of the IGWaterfallSeries type that share the same X axis are rendered in clusters where each cluster represents a data point. The first series in the series collection of the IGChartView control renders as a column on the left of the cluster. Each successive series gets rendered to the right of the previous series. However, if series do not share the X axis, they are rendered in layers with each successive series rendered in front of the previous one.
*/
@interface IGWaterfallSeries : IGHorizontalAnchoredCategorySeries

/** Specifies the brush used to color the negative sections of data.
 When this property is set, all negative data will be colored with the specified brush.
 */
@property (nonatomic, retain) IGBrush* negativeBrush;
@end


/***** Scatter Series *****/
/*!
 IGScatterSeries uses cartesian coordinates to display markers. Each marker is placed at a point denoted by a pair of numeric values. IGScatterSeries uses IGNumericXAxis and IGNumericYAxis. The data source is a collection of scatter points, each with two numeric values and a label. IGScatterSeriesDataSourceHelper can be used as the datasource for IGScatterSeries.
 */
@interface IGScatterSeries : IGScatterBase
@end


/***** Scatter Line Series *****/
/*!
 IGScatterLineSeries uses cartesian coordinates to display markers connected with lines. Each marker is placed at a point denoted by a pair of numeric values. IGScatterLineSeries uses IGNumericXAxis and IGNumericYAxis. The data source is a collection of scatter points, each with two numeric values and a label. IGScatterSeriesDataSourceHelper can be used as the datasource for IGScatterLineSeries.
 */
//@interface IGScatterLineSeries : IGScatterBase
//{
//    IGUnknownValuePlotting _unknownValuePlotting;
//}
//@property (nonatomic) IGUnknownValuePlotting unknownValuePlotting;
//@end



/***** Scatter Spline Series *****/
/*!
 IGScatterSpineSeries uses cartesian coordinates to display markers connected with curved lines. Each marker is placed at a point denoted by a pair of numeric values. IGScatterSpineSeries uses IGNumericXAxis and IGNumericYAxis. The data source is a collection of scatter points, each with two numeric values and a label. IGScatterSeriesDataSourceHelper can be used as the datasource for IGScatterSpineSeries.
 */
//@interface IGScatterSplineSeries : IGScatterBase
//{
//    float _stiffness;
//}
//
//@property (nonatomic) float stiffness;
//@end


/***** Bubble Series *****/
/*!
 IGBubbleSeries uses cartesian coordinates to display markers of varying sizes. Each marker is placed at a point denoted by a pair of numeric values. The size of the marker is determined by the radius value of the data point. IGBubbleSeries uses IGNumericXAxis and IGNumericYAxis. The data source is a collection of points, each with two numeric values, a radius and a label. IGBubbleSeriesDataSourceHelper can be used as the datasource for IGBubbleSeries.
 
 IGBubbleSeries supports several scales. Fill scale is used to color the bubble markers based on a custom palette and radius scale is used to size the markers according to the scale's properties.
 */
@interface IGBubbleSeries : IGScatterBase
{
    IGBrushScale *_fillScale;
    IGSizeScale *_radiusScale;
}

/** Specifies the scale for marker brushes.
 This property is used to set a brush scale to the bubble series. The markers will be colored using the brushes from IGBrushScale. The brushes can be used sequentially or they can be interpolated.
 */
@property (nonatomic, retain) IGBrushScale *fillScale;

/** Specifies the scale for marker sizes.
 This property is used to set a numeric scale to the marker sizes. When the scale is set, the smallest marker will be set to the smallest value of the scale, while the largest marker will be set to the largest value of the scale. The rest of the markers will be scalled accordingly.
 */
@property (nonatomic, retain) IGSizeScale *radiusScale;
@end


/***** PolarArea Series *****/
/*!
IGPolarAreaSeries has a shape of a filled polygon with vertices or corners located at the polar coordinates of data points. The IGPolarAreaSeries uses the same concepts of data plotting as the IGScatterSeries but wraps data points around a circle rather than stretching them along a horizontal line. Like with other series types, multiple IGPolarAreaSeries can be plotted in the same chart view and they can be overlaid on top of each other to show differences and similarities between data sets.
 
 IGPolarAreaSeries uses IGNumericAngleAxis and IGNumericRadiusAxis. Markers are placed at the locations of data points. IGPolarSeriesDataSourceHelper can be used as the data source for this series.
 */
@interface IGPolarAreaSeries : IGPolarBase

/** An enumeration value that determines how empty values are handled.
 This property specifies whether null values are treated as zeroes, skipped over or if an interpolation is used.
 */
@property (nonatomic) IGUnknownValuePlotting unknownValuePlotting;
@end


/***** PolarLine Series *****/
/*!
 IGPolarLineSeries is rendered using a collection of straight lines connecting data points. The IGPolarLineSeries uses the same concepts of data plotting as the IGScatterSeries but wraps data points around a circle rather than stretching them along a horizontal line. Like with other series types, multiple IGPolarLineSeries can be plotted in the same chart view and they can be overlaid on top of each other to show differences and similarities between data sets.
 
 IGPolarLineSeries uses IGNumericAngleAxis and IGNumericRadiusAxis. Markers are placed at the locations of data points. IGPolarSeriesDataSourceHelper can be used as the data source for this series.
 */
@interface IGPolarLineSeries : IGPolarBase

/** An enumeration value that determines how empty values are handled.
 This property specifies whether null values are treated as zeroes, skipped over or if an interpolation is used.
 */
@property (nonatomic) IGUnknownValuePlotting unknownValuePlotting;
@end


/***** PolarScatter Series *****/
/*!
 IGPolarScatterSeries is rendered using a collection of shapes placed at data point locations. The IGPolarLineSeries uses the same concepts of data plotting as the IGScatterSeries but wraps data points around a circle rather than stretching them along a horizontal line. Like with other series types, multiple IGPolarScatterSeries can be plotted in the same chart view and they can be overlaid on top of each other to show differences and similarities between data sets.
 
 IGPolarScatterSeries uses IGNumericAngleAxis and IGNumericRadiusAxis. Markers are placed at the locations of data points. IGPolarSeriesDataSourceHelper can be used as the data source for this series.
 */
@interface IGPolarScatterSeries : IGPolarBase
@end


///***** PolarSpline Series *****/
///*!
// IGPolarSplineSeries is rendered using a collection of curved lines connecting data points. The IGPolarSplineSeries uses the same concepts of data plotting as the IGScatterSeries but wraps data points around a circle rather than stretching them along a horizontal line. Like with other series types, multiple IGPolarSplineSeries can be plotted in the same chart view and they can be overlaid on top of each other to show differences and similarities between data sets.
// 
// IGPolarSplineSeries uses IGNumericAngleAxis and IGNumericRadiusAxis. Markers are placed at the locations of data points. IGPolarSeriesDataSourceHelper can be used as the data source for this series.
// */
//@interface IGPolarSplineSeries : IGPolarBase
//{
//    float _stiffness;
//}
//
///** A value that determines the curvature of the spline.
// Smaller values will cause the spline to be more shallow, whle larger values will make the spline more curved.
// */
//@property (nonatomic) float stiffness;
//@end


///***** PolarSplineArea Series *****/
///*!
// IGPolarSplineAreaSeries has a shape of a filled curved polygon with vertices or corners located at the polar coordinates of data points. The IGPolarSplineAreaSeries uses the same concepts of data plotting as the IGScatterSeries but wraps data points around a circle rather than stretching them along a horizontal line. Like with other series types, multiple IGPolarSplineAreaSeries can be plotted in the same chart view and they can be overlaid on top of each other to show differences and similarities between data sets.
// 
// IGPolarSplineAreaSeries uses IGNumericAngleAxis and IGNumericRadiusAxis. Markers are placed at the locations of data points. IGPolarSeriesDataSourceHelper can be used as the data source for this series.
// */
//@interface IGPolarSplineAreaSeries : IGPolarBase
//{
//    float _stiffness;
//}
//
///** A value that determines the curvature of the spline.
// Smaller values will cause the spline to be more shallow, whle larger values will make the spline more curved.
// */
//@property (nonatomic) float stiffness;
//@end


/***** RadialColumn Series *****/
/*!
 IGRadialColumnSeries displays rectangular columns that extend from the center of the chart towards the locations of data points. This series uses IGCategoryAngleAxis and IGNumericRadiusAxis. This series behaves similar to IGColumnSeries, but the category axis is wrapped around a circle. IGCategoryDataSourceHelper can be used as a data source for this series.
 */
@interface IGRadialColumnSeries : IGAnchoredRadialSeries
@end


/***** RadialLine Series *****/
/*!
 IGRadialLineSeries displays straight lines connecting data points. This series uses IGCategoryAngleAxis and IGNumericRadiusAxis. This series behaves similar to IGLineSeries, but the category axis is wrapped around a circle. IGCategoryDataSourceHelper can be used as a data source for this series.
 */
@interface IGRadialLineSeries : IGAnchoredRadialSeries

/** An enumeration value that determines how empty values are handled.
 This property specifies whether null values are treated as zeroes, skipped over or if an interpolation is used.
 */
@property (nonatomic) IGUnknownValuePlotting unknownValuePlotting;
@end



/***** RadiaArea Series *****/
//@interface IGRadialAreaSeries : IGAnchoredRadialSeries
//{
//    IGUnknownValuePlotting _unknownValuePlotting;
//}
//@property (nonatomic) IGUnknownValuePlotting unknownValuePlotting;
//@end



/***** RadialPie Series *****/
/*!
 IGRadialPieSeries displays filled pie slices that extend from the center of the chart towards the locations of data points. This series uses IGCategoryAngleAxis and IGNumericRadiusAxis. This series behaves similar to IGColumnSeries, but the category axis is wrapped around a circle. IGCategoryDataSourceHelper can be used as a data source for this series.
 */
@interface IGRadialPieSeries : IGAnchoredRadialSeries
@end



/***** Financial Price Series *****/
/*!
IGFinancialPriceSeries is used to plot stock prices and show the stock's high, low, open and close prices over time. In addition, it can display trend lines for stock prices. IGFinancialPriceSeries is often used in combination with a number of other financial indicators to show price trends. IGFinancialPriceSeries uses IGCategoryXAxis or IGCategoryDateTimeXAxis and IGNumericYAxis. IGOHLCSeriesDataSourceHelper can be used as a data source for this series.
 
 IGFinancialPriceSeries supports two rendering modes: CangleStick and OHLC. In candlestick mode each data point is plotted as a vertical column with vertical lines on both the top and bottom. The vertical line indicates the span between high and low values of an investment. The top of the vertical line indicates the highest price during a session and the bottom of the vertical line indicates the lowest price during a session.
 
 The vertical columns indicate the span between the opening and closing values of an investment. The columns are filled using brush property when there is positive value and using negativeBrush property when there is negative value between the opening and closing values.
 
 In OHLC mode each data point is plotted as a vertical line with horizontal perpendicular lines on both the left and right side. The vertical line indicates the span between high and low values of an investment. The top of the vertical line indicates the highest price during a session and the bottom of the vertical line indicates the lowest price during a session. The horizontal lines indicate the span between the opening and closing values of an investment. The horizontal line on the left-hand side of the vertical line indicates the opening value of a session. The horizontal line on the right-hand side of the vertical line indicates the closing value of a session.
 
Mmultiple series of the IGFinancialPriceSeries type that share the same X axis are rendered in clusters where each cluster represents a data point. The first series in the series collection of the IGChartView control renders as a column on the left of the cluster. Each successive series gets rendered to the right of the previous series. However, if series do not share the X axis, they are rendered in layers with each successive series rendered in front of the previous one.
*/
@interface IGFinancialPriceSeries : IGFinancialSeries
{
    NSArray *_trendLineDashArray;
}

/** An enumeration value that determines the shapes of data points.
 IGFinancialPriceSeries can display data as a candle stick or OHLC lines.
 */
@property (nonatomic) IGPriceDisplayType displayType;

/** An enumeration property that determines which trend line to use.
 */
@property (nonatomic) IGTrendLineType trendLineType;

/** Returns the resolved brush for the series trend line. (read-only)
 */
@property (nonatomic, readonly) IGBrush *actualTrendLineBrush;

/** This property determines the brush of the series trend line.
 */ 
@property (nonatomic, retain) IGBrush *trendLineBrush;

/** An array of values used to create a dash pattern for the series trend line.
 This property uses an array of CGFloats to construct a repeating pattern of alternating dashes and spaces.
 */
@property (nonatomic, retain) NSArray *trendLineDashArray;

/** An enumeration value that determines the trend line's dash cap.
 This propery specifies the shape used to draw line caps when the trend line uses a dashed pattern.
 */
@property (nonatomic) CGLineCap trendLineDashCap;

/** A value that determines the moving average period.
 This property only applies to the follwing trend lines: ExponentialAverage, ModifiedAverage, SimpleAverage, WeightedAverage.
 */
@property (nonatomic) int trendLinePeriod;

/** A value that determines the thickness (in pixels) of the trend line.
 */
@property (nonatomic) float trendLineThickness;

/** A value that determines the Z-index of the trend line.
 Values greater than 1000 will result in the trend line being rendered in front of the series data. Values below 1000 will cause the trend line to render behind the series.

@property (nonatomic) int trendLineZIndex; */
@end



///***** Value Overlay *****/
///*!
// Value overlay is currently not supported
// */
//@interface IGValueOverlay : IGSeries
//{
//    IGAxis *_axis;
//    float _value;
//}
//
///** Specifies the axis used by the overlay.
// */
//@property (nonatomic, retain) IGAxis *axis;
//
///** A value that determines where the overlay is rendered. 
// */
//@property (nonatomic) float value;
//@end

 

/***** AbsoluteVolumeOscillatorIndicator *****/
/*!
 IGAbsoluteVolumeOscillatorIndicator is an indicator that is calculated by taking the difference between two average volume measures. It's scores range from -100% to +100%. The indicator is used to identify whether volume trends are increasing or decreasing. The user can select the time period for analysis.
 */
@interface IGAbsoluteVolumeOscillatorIndicator : IGFinancialIndicator

/** A value that determines the short moving average period.
 The default value is 10.
 */
@property (nonatomic) int shortPeriod;

/** A value that determines the long moving average period.
 The default value is 30. 
 */
@property (nonatomic) int longPeriod;
@end


/***** AccumulationDistributionIndicator *****/
/*!
  IGAccumulationDistributionIndicator evaluates the supply and demand of a stock, security, or index over time by looking at disparities in whether investors are selling or buying.
 */
@interface IGAccumulationDistributionIndicator : IGFinancialIndicator
@end


/***** AverageDirectionalIndexIndicator *****/
@interface IGAverageDirectionalIndexIndicator : IGFinancialIndicator

/** A value that determines the period of the indicator.
 */
@property (nonatomic) int period;
@end


/***** AverageTrueRangeIndicator *****/
/*!
IGAverageTrueRangeIndicator is a financial indicator that measures a security's degree of price movement or volatility within a given period of time. The indicator is not a measure of price direction or duration, but simply the amount of price movement or volatility. The Average True Range (ATR) is frequently calculated with a 14 day period using several bases, including: daily, weekly or monthly. The Average True Range is the exponential moving average of the TR values for the last 14 periods. The actual period used can vary depending on user preference.
 */
@interface IGAverageTrueRangeIndicator : IGFinancialIndicator

/** A value that determines the period of the indicator.
 */
@property (nonatomic) int period;
@end


///***** Moving Average Conversion Diversion Indicator *****/
///*!
//IGMovingAverageConversionDiversionIndicator (MACD) is used to identify changes in the strength, direction, momentum, or length of a trend for a stock price. MACD is computed by taking the difference between two exponential moving averages (EMAs) of closing prices. The difference is then charted over time with a moving average of the difference.
// */
//@interface IGMovingAverageConversionDiversionIndicator: IGFinancialIndicator
//{
//    int _shortPeriod, _longPeriod, _signalPeriod;
//}
//
///** A value that determines the short moving average period.
// The default value is 10.
// */
//@property (nonatomic) int shortPeriod;
//
///** A value that determines the long moving average period.
// The default value is 30. 
// */
//@property (nonatomic) int longPeriod;
//
///** A value that determines the signal period.
// */
//@property (nonatomic) int signalPeriod;
//@end


/***** TypicalPriceIndicator *****/
/*!
 IGTypicalPriceIndicator represents the arithmetic average of the High, Low and Closing prices of a security for a given period of time.
 */
@interface IGTypicalPriceIndicator : IGFinancialIndicator
@end
