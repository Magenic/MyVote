//
//  IGSeriesDataSourceHelper.h
//
//  Copyright (c) 2012 Infragistcs. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Series.h"

/*!
 This is the base class for all series data source helpers. This class should not be allocated.
 */
@interface IGSeriesDataSourceHelper : NSObject<IGSeriesDataSource>

/** An array of custom data objects. The fields in the data object are accessed via memberPath properties.
 */
@property (nonatomic, retain) NSArray* data;

/** An array of string labels used to create data points.
 */
@property (nonatomic, retain) NSArray* labels;

/** Name of the property containing labels.
 */
@property (nonatomic, retain) NSString* labelPath;
@end


/*!
 This data source helper is used to create a data source usable by category series, or any series that uses single value data points. A typical example of such series is IGColumnSeries or IGLineSeries. This data source accepts an array of values and creates a sequence of IGCategoryPoints for the series.
 */
@interface IGCategorySeriesDataSourceHelper : IGSeriesDataSourceHelper

/** A numeric array of values used to create data points.
 */
@property (nonatomic, retain) NSArray* values;

/** A string value path that specifies the property in the data source used for values.
 */
@property (nonatomic, retain) NSString *valuePath;

///--------------------
///@name Initializing DataSource Helper
///--------------------

/** Initializes the data source with an array of numeric values.
 @param values Array of numeric values.
 @return Returns an initialized data source.
 */
-(id)initWithValues:(NSArray*)values;

/** Initializes the data source with an array of custom data objects.
 @param data Array of custom objects.
 @param valuePath The name of the property containing values.
 @return Returns an initialized data source.
 */
-(id)initWithData:(NSArray*)data andValuePath:(NSString*)valuePath;
@end


/*!
 This data source helper is used to create a data source usable by scatter series. The data points of this data source are numeric value pairs to be used in a cartesian coordinate system. This data source accepts an array of values and creates a sequence of IGScatterPoint objects for the series.
 */
@interface IGScatterSeriesDataSourceHelper : IGSeriesDataSourceHelper

/** An array of x coordinate values.
 */
@property (nonatomic, retain) NSArray* xValues;

/** An array of y coordinate values.
*/
@property (nonatomic, retain) NSArray* yValues;

/** A string value path that specifies the property in the data source used for x coordinates.
 */
@property (nonatomic, retain) NSString *xPath;

/** A string value path that specifies the property in the data source used for y coordinates.
 */
@property (nonatomic, retain) NSString *yPath;

///--------------------
///@name Initializing DataSource Helper
///--------------------

/** Initializes the data source with arrays of numeric values.
 @param xValues Array of numeric values used for the X Axis.
 @param yValues Array of numeric values used for the Y Axis.
 @return Returns an initialized data source.
 */
-(id)initWithXValues:(NSArray*)xValues andYValues:(NSArray*)yValues;

/** Initializes the data source with an array of custom data objects.
 @param data Array of custom objects.
 @param xPath The name of the property containing X values.
 @param yPath The name of the property containing Y values.
 @return Returns an initialized data source.
 */
-(id)initWithData:(NSArray*)data xPath:(NSString*)xPath andYPath:(NSString*)yPath;
@end



/*!
 This data source helper is used to create a data source usable by category series that uses IGCategoryDateTimeXAxis. The data points of this data source contain a value, a date and a label. This data source is used by charts displaying continuous data, such as IGLineSeries or IGAreaSeries. The data source generates a sequence of IGCategoryDatePoint objects.
 */
@interface IGCategoryDateSeriesDataSourceHelper : IGSeriesDataSourceHelper

/** An array of numeric values used to create data points.
 */
@property (nonatomic, retain) NSArray *values;

/** An array of NSDate objects used to create data points.
 */
@property (nonatomic, retain) NSArray *dates;

/** A string name of the property in the data source used for values.
 */
@property (nonatomic, retain) NSString* valuePath;

/** A string name of the property in the data source used for dates.
 */
@property (nonatomic, retain) NSString* datePath;

///--------------------
///@name Initializing DataSource Helper
///--------------------

/** Initializes the data source with an array of numeric values and dates.
 @param values Array of numeric values.
 @param dates Array of dates.
 @return Returns an initialized data source.
 */
-(id)initWithValues:(NSArray*)values andDates:(NSArray*)dates;

/** Initializes the data source with an array of custom data objects.
 @param data Array of custom objects.
 @param valuePath The name of the property containing values.
 @param datePath The name of the property containing dates.
 @return Returns an initialized data source.
 */
-(id)initWithData:(NSArray*)data valuePath:(NSString*)valuePath andDatePath:(NSString*)datePath;
@end


/*!
 This data source helper is used to create a data source usable by bubble series. The data points of this data source are numeric value pairs to be used in a cartesian coordinate system and a radius value for the size of the bubble marker. This data source accepts an array of values and creates a sequence of IGBubblePoint objects for the series.
 */
@interface IGBubbleSeriesDataSourceHelper : IGSeriesDataSourceHelper

/** An array of x coordinate values.
 */
@property (nonatomic, retain) NSArray* xValues;

/** An array of y coordinate values.
 */
@property (nonatomic, retain) NSArray* yValues;

/** An array of radius values.
*/
@property (nonatomic, retain) NSArray* radiusValues;

/** An array of fill values.
 */
@property (nonatomic, retain) NSArray *fillValues;

/** A string value path that specifies the property in the data source used for x coordinates.
 */
@property (nonatomic, retain) NSString *xPath;

/** A string value path that specifies the property in the data source used for y coordinates.
 */
@property (nonatomic, retain) NSString *yPath;

/** A string value path that specifies the property in the data source used for radii.
 */
@property (nonatomic, retain) NSString *radiusPath;

/** A string value path that specifies the property in the data source used for fill values;
 */
@property (nonatomic, retain) NSString *fillPath;

///--------------------
///@name Initializing DataSource Helper
///--------------------

/** Initializes the data source with arrays of numeric values.
 @param xValues Array of numeric values used for the X Axis.
 @param yValues Array of numeric values used for the Y Axis.
 @param radiusValues Array of numeric values used as radius values.
 @return Returns an initialized data source.
 */
-(id)initWithXValues:(NSArray*)xValues yValues:(NSArray*)yValues andRadiusValues:(NSArray*)radiusValues;

/** Initializes the data source with an array of custom data objects.
 @param data Array of custom objects.
 @param xPath The name of the property containing X values.
 @param yPath The name of the property containing Y values.
 @param radiusPath The name of the property containing radius values.
 @return Returns an initialized data source.
 */
-(id)initWithData:(NSArray*)data xPath:(NSString*)xPath yPath:(NSString*)yPath andRadiusPath:(NSString*)radiusPath;
@end


/*!
 This data source helper is used to create a data source usable by range series, or series that require a high and a low value. The data points of this data source are numeric high and low values. This data source accepts an array of values and creates a sequence of IGHighLowPoint objects for the series.
 */
@interface IGHighLowSeriesDataSourceHelper : IGSeriesDataSourceHelper 

/** An array of high values.
 */
@property (nonatomic, retain) NSArray* highValues;

/** An array of low values.
 */
@property (nonatomic, retain) NSArray* lowValues;

/** A string value path that specifies the property in the data source used for high values.
 */
@property (nonatomic, retain) NSString *highPath;

/** A string value path that specifies the property in the data source used for low values.
 */
@property (nonatomic, retain) NSString *lowPath;

///--------------------
///@name Initializing DataSource Helper
///--------------------

/** Initializes the data source with arrays of numeric values.
 @param highValues Array of numeric values used for the high values.
 @param lowValues Array of numeric values used for the low values.
 @return Returns an initialized data source.
 */
-(id)initWithHighValues:(NSArray*)highValues andLowValues:(NSArray*)lowValues;

/** Initializes the data source with an array of custom data objects.
 @param data An array of custom objects.
 @param highPath The name of the property containing high values.
 @param lowPath The name of the property containing low values.
 @return Returns an initialized data source.
 */
-(id)initWithData:(NSArray*)data highPath:(NSString*)highPath andLowPath:(NSString*)lowPath;
@end


/*!
 This data source helper is used to create a data source usable by range series, or series that require open, high, low and close values. This data source accepts an array of values and creates a sequence of IGOHLCPoint objects for the series.
 */
@interface IGOHLCSeriesDataSourceHelper : IGSeriesDataSourceHelper

/** An array of open values.
 */
@property (nonatomic, retain) NSArray* openValues;

/** An array of high values.
 */
@property (nonatomic, retain) NSArray* highValues;

/** An array of low values.
 */
@property (nonatomic, retain) NSArray* lowValues;

/** An array of close values.
 */
@property (nonatomic, retain) NSArray* closeValues;

/** An array of volume values.
 */
@property (nonatomic, retain) NSArray* volumeValues;

/** A string value path that specifies the property in the data source used for close values.
 */
@property (nonatomic, retain) NSString *closePath;

/** A string value path that specifies the property in the data source used for open values.
 */
@property (nonatomic, retain) NSString *openPath;

/** A string value path that specifies the property in the data source used for high values.
 */
@property (nonatomic, retain) NSString *highPath;

/** A string value path that specifies the property in the data source used for low values.
 */
@property (nonatomic, retain) NSString *lowPath;

/** A string value path that specifies the property in the data source used for volume values.
 */
@property (nonatomic, retain) NSString *volumePath;

///--------------------
///@name Initializing DataSource Helper
///--------------------

/** Initializes the data source with arrays of numeric values.
 @param openValues Array of numeric values used for the open values.
 @param highValues Array of numeric values used for the high values.
 @param lowValues Array of numeric values used for the low values.
 @param closeValues Array of numeric values used for the close values.
 @return Returns an initialized data source.
 */
-(id)initWithOpenValues:(NSArray*)openValues highValues:(NSArray*)highValues lowValues:(NSArray*)lowValues andCloseValues:(NSArray*)closeValues;

/** Initializes the data source with an array of custom data objects.
 @param data Array of custom objects.
 @param openPath The name of the proeprty containing open values.
 @param highPath The name of the property containing high values.
 @param lowPath The name of the property containing low values.
 @param closePath The name of the property containing close values.
 @return Returns an initialized data source.
 */
-(id)initWithData:(NSArray*)data openPath:(NSString*)openPath highPath:(NSString*)highPath lowPath:(NSString*)lowPath andClosePath:(NSString*)closePath;

@end


/*!
  This data source helper is used to create a data source usable by polar series. The data points of this data source are numeric value pairs to be used in a polar coordinate system. This data source accepts an array of values and creates a sequence of IGPolarPoint objects for the series.
 */
@interface IGPolarSeriesDataSourceHelper : IGSeriesDataSourceHelper

/** An array of angle values.
 */
@property (nonatomic, retain) NSArray* angleValues;

/** An array of radius values.
 */
@property (nonatomic, retain) NSArray* radiusValues;

/** A string value path that specifies the property in the data source used for angle values.
 */
@property (nonatomic, retain) NSString *anglePath;

/** A string value path that specifies the property in the data source used for radius values.
 */
@property (nonatomic, retain) NSString *radiusPath;

///--------------------
///@name Initializing DataSource Helper
///--------------------

/** Initializes the data source with arrays of numeric values.
 @param angleValues Array of numeric values used for the angle axis.
 @param radiusValues Array of numeric values used for the radius axis.
 @return Returns an initialized data source.
 */
-(id)initWithAngleValues:(NSArray*)angleValues andRadiusValues:(NSArray*)radiusValues;

/** Initializes the data source with an array of custom data objects.
 @param data An array of custom objects.
 @param anglePath The name of the proeprty containing angle values.
 @param radiusPath The name of the property containing radius values.
 @return Returns an initialized data source.
 */
-(id)initWithData:(NSArray*)data anglePath:(NSString*)anglePath andRadiusPath:(NSString*)radiusPath;
@end