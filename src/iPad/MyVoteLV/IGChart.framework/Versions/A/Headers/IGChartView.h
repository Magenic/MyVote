//
//  IGChartView.h
//
//  Copyright (c) 2012 Infragistcs. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>
#import "Enums.h"
#import "Axis.h"
#import "Core.h"
#import "Series.h"
#import "IGLegend.h"
#import "IGSeriesDataSourceHelper.h"
#import "VisualChartData.h"
#import "Themes.h"

@class DataChartContainer;
@class LegendContainer;
@class IGChartView;

@protocol IGChartViewDelegate <NSObject>
@optional
-(UIView*)chartView:(IGChartView*)chartView viewForTooltipWithItemlist:(NSDictionary*)itemlist;
-(void)chartView:(IGChartView*)chartView tapForSeries:(IGSeries*)series withItem:(NSObject*)item atPoint:(CGPoint)point;
-(NSString*)chartView:(IGChartView*)chartView labelForAxis:(IGAxis*)axis withItem:(NSObject*)item;
@end

/*!
 A chart view is a data visualization control that creates a graphical representation of the user's data. This control is designed to display high amounts of data and can handle constant data updates.
 
 A chart view consists of two or more IGAxis objects and one or more IGSeries objects. The type of axes will depend on which series are used in the chart view. Each IGSeries object has a detailed description about axes requirements. The chart view supports an unlimited number of axes and an unlimited number of series. A typical chart view may include an IGCategoryXAxis, an IGNumericYAxis and a series such as IGColumnSeries.
 */
@interface IGChartView : UIView <UIScrollViewDelegate>
{
    UIScrollView *_scrollView;
    UIView *_scrollingContainer;
    DataChartContainer *_chartContainer;
    NSMutableArray *_brushes, *_outlines, *_markerBrushes, *_markerOutlines;
    NSMutableDictionary *_axisDictionary, *_seriesDictionary;
    IGChartThemeDefinition* _theme;
    IGCrosshairsVisibility _crosshairsVisibility;
}

/** Sets the IGChartViewDelegate for the chart view
 */
@property (nonatomic, weak) id<IGChartViewDelegate> delegate;

/** Returns an array of IGAxis objects used by the chart view. (read-only)
 
 This property will return a read-only list of axes. To add or remove an axis, use addAxis and removeAxis methods.
 */
@property (nonatomic, readonly) NSArray* axes;

/** Returns an array of IGSeries objects used by the chart view. (read-only)
 
 This property will return a read-only list of series. To add or remove a series, use addSeries and removeSeries methods.
 */
@property (nonatomic, readonly) NSArray* series;

/** Returns an array of IGBrush objects used by the chart's series.
 
 This property will return a list of brushes used by the series. Each of the chart's series will use a brush from the brushes array in the same order. Setting a brush on a series directly takes precedence over this array, in which case, the brush from the array will be skipped over.
 */
@property (nonatomic, retain) NSArray *brushes;

/** Returns an array of IGBrush objects used by the chart's series markers.
 
 This property will return a list of brushes used by the series markers. Each of the chart's series will use a marker brush from the markerBrushes array in the same order. Setting a marker brush on a series directly takes precedence over this array, in which case, the brush from the array will be skipped over.
 */
@property (nonatomic, retain) NSArray *markerBrushes;

/** Returns an array of IGBrush objects used to draw a border around the series shapes.
 
 This property will return a list of outlines used by the series. Each of the chart's series will use an outline from the outlines array in the same order. Setting an outline on a series directly takes precedence over this array, in which case, the outline from the array will be skipped over.
 */
@property (nonatomic, retain) NSArray *outlines;

/** Returns an array of IGBrush objects used to draw a border around the series markers.
 
 This property will return a list of outlines used by the series markers. Each of the chart's series will use a marker outline from the markerOutlines array in the same order. Setting a marker outline on a series directly takes precedence over this array, in which case, the outline from the array will be skipped over. To add or remove an outline, use addMarkerOutline and removeMarkerOutline methods.
 */
@property (nonatomic, retain) NSArray *markerOutlines;

/** Represents the brush used to color the chart view's plot area.
 
 The plot area is the portion of the chart view that contains axis lines, axis gridlines and all the series. It excludes the region marked by the axis extent.
 */
@property (nonatomic, retain) IGBrush *plotAreaBrush;

/** Determines how the grid lines are displayed.
 
 This enumeration property determines whether the grid lines are displayed on top of all series or underneath them. The enumeration is of type IGGridMode
 */
@property (nonatomic) IGGridMode gridMode;

/** A Boolean value indicating whether a square aspect ratio should be used for the chart view.
 
 A square aspect ratio is locked to YES for polar and radial charts.
 */
@property (nonatomic) BOOL isSquare;

/** An object that defines a set of brushes and fonts that will be used to style the chart. 
 */
@property (nonatomic, retain) IGChartThemeDefinition* theme;

/** Specifies the legend control used by the chart.
 
 This property lets the user set a reference to an existing legend control. When this property is set, all series in the chart will use the specified legend, unless a series has a legend property set directly on it.
 */
@property (nonatomic, weak) IGLegendBase *legend;

/** Specifies the tooltip location.
 
 This property determines where the tooltip will be positioned. The default setting uses a floating tooltip, which follows the location of the long press. Tooltip can also be pinned to top, bottom, left, or right.
 */
@property (nonatomic) IGTooltipPinLocation tooltipPinLocation;

/** Specifies the zoom scale.

 This property determines what the chart view's zoom scale should be. The default value is 1, which means fully zoomed out.
 */
@property (nonatomic) float zoomScale;

/** Specifies the minimum zoom scale.
 
 This property determines how far the chart view should be allowed to zoom out. The default value is 1. Set minimumZoomScale to be greater than maximumZoomScale to disable zooming.
 */
@property (nonatomic) float minimumZoomScale;

/** Specifies the maximum zoom scale.
 
 This property determines how far the chart view should be allowed to zoom in. The default value is 200. Set minimumZoomScale to be greater than maximumZoomScale to disable zooming.
 */
@property (nonatomic) float maximumZoomScale;

/** The point at which the origin of the content view is offset from the origin of the chart view.
 */
@property (nonatomic) CGPoint zoomContentOffset;

/** The thickness of the crosshair lines. 
 
 This property determines the thickness of both crosshairs. The default value is 1.5.
 */
@property (nonatomic) float crosshairsThickness;

/** The color of the crosshairs.
 */
@property (nonatomic, retain) UIColor *crosshairsBackground;

/** Determines the visibility of the crosshairs.
 
 The chart view can display both crosshairs, vertical or horizontal, or none.
 */
@property (nonatomic) IGCrosshairsVisibility crosshairsVisibility;

/** Gets or sets when anti-aliasing should occur while rendering the chart.
 
 Setting this property to IGChartAntiAliasingAlways will result in the best looking chart, however it will have slower performance. Setting it to IGChartAntiAliasingNever will result in the best performance, however the chart will not be rendered smoothly. The last option, which is also the default, IGChartAntiAliasingAlwaysExceptWhilePanningAndZooming, will result in the chart looking smooth, except while zooming and panning, however the zooming and panning operations will be much faster. 
 */
@property(nonatomic, assign)IGChartAntiAliasing antiAliasing;

/** Specifies whether series animation will proceed when a change in the axis range has been detected.
 */
@property (nonatomic) BOOL animateSeriesWhenAxisRangeChanges;

///----------------------
///@name Initializing the chart view
///----------------------

/** Initializes the chart view with a given frame
 @param frame A rectangular frame that specifies the location and size of the chart view.
 @return Returns an initialized IGChartView object.
 */
-(id)initWithFrame:(CGRect)frame;

///----------------------
///@name Configuring the chart view
///----------------------

/** Adds an axis to the chart view.
 @param axis Axis object to be added to the chart view.
 */
-(void)addAxis:(IGAxis*)axis;

/** Removes a specified axis from the chart view.
 @param axis Axis object to be removed from the chart view.
 */
-(void)removeAxis:(IGAxis*)axis;

/** Gets an axis from the chart for a specified key.
 
 This method returns a reference to the axis in the chart's axes array for a given key. If the axis cannot be found the method returns nil.
 @param key Unique axis identifier key.
 */
-(IGAxis*)findAxisByKey:(NSString*)key;

/** Adds a series to the chart view.
 @param series Series to be added to the chart view.
 */
-(void)addSeries:(IGSeries*)series;

/** Removes a specified series from the chart view.
 @param series Series to be removed from the chart view.
 */
-(void)removeSeries:(IGSeries*)series;

/** Gets a series form the chart for a specified key.
 
 This method returns a reference to the series in the chart's series array for a given key. If the series cannot be found the method returns nil.
 @param key Unique series identifier key.
 */
-(IGSeries*)findSeriesByKey:(NSString*)key;

/** Creates a series from a series type and adds it to the chart.
 
 This method creates an IGSeries based on the specified series type. It also creates appropriate axes based on the specified axis keys. If any of the keys already exist, the chart will attempt to use that axis, provided that the axis type can be applied to the series. The first axis key is typically the X axis, while the second key is the Y axis. For polar and radial series, the first axis key is the angle axis, while the second key is the radius axis.
 @param seriesType Series type, such as IGColumnSeries or IGFinancialSeries.
 @param seriesKey A unique series string identifier.
 @param dataSource Series data source.
 @param axisKey1 First axis key.
 @param axisKey2 Second axis key.
 @return Returns a new instance of a series that was added to the chart.
 */
-(IGSeries*)addSeriesForType:(Class)seriesType usingKey:(NSString*)seriesKey withDataSource:(id<IGSeriesDataSource>)dataSource firstAxisKey:(NSString*)axisKey1 secondAxisKey:(NSString*)axisKey2;

/** Causes the chart view to refresh itself.
 */
-(void)refresh;

/** Returns a data representation of the visuals of the chart. 
 
 This method is available to provide a way to do validation for testing of the visuals of the chart.
 */
- (VisualChartData*)exportVisualData;

///-------------------------
///@name Notifying the chart view of data source changes
///-------------------------

/** Notifies the chart view that all items in the data source were cleared.
 
 This method is used to tell the chart view that all of the items in a given data source have been removed.
 @param source Data source that had its items removed.
 */
-(void) clearItemsForDataSource:(id<IGSeriesDataSource>)source;

/** Notifies the chart view that an item has been inserted in the data source.
 
 This method is used to tell the chart view that an item has been inserted into a given data source at a given index.
 @param index Index of an item that has been inserted. 
 @param source Data source that had an item inserted.
 */
-(void) insertItemAtIndex:(int)index withSource:(id<IGSeriesDataSource>)source;

/** Notifies the chart view that an item has been removed from the data source.
 
 This method is used to tell the chart view that an item has been removed from a given data source at a given index.
 @param index Index of an item that has been removed.
 @param source Data source that had an item removed.
 */
-(void) removeItemAtIndex:(int)index withSource:(id<IGSeriesDataSource>)source;

/** Notifies the chart view that an item has been replaced in the data source.
 
 This method is used to tell the chart view that an item has been replaced in a given data source at a given index. 
 @param index Index of an item that has been replaced.
 @param source Data source that had an item replaced.

 */
-(void) replaceItemAtIndex:(int)index withSource:(id<IGSeriesDataSource>)source;

/** Notifies the chart view that an item has been updated in the data source.
 
 This method is used to tell the chart view that an item has been updated in a given data source at a given index.
 @param index Index of an item that has been replaced.
 @param source Data source that had an item replaced.
 
 */
-(void)updateItemAtIndex:(int)index withSource:(id<IGSeriesDataSource>)source;

@end
