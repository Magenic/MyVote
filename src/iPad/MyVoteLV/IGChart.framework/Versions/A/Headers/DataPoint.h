//
//  DataPoint.h
//
//  Copyright (c) 2012 Infragistcs. All rights reserved.
//

#import <Foundation/Foundation.h>

/*!
 IGDataPoint is the base class for all data point types. This class should not be allocated.
 */
@interface IGDataPoint : NSObject

/** A string value representing the point's label.
 */
@property (nonatomic, retain) NSString* label;

@end



/***** Category Point *****/

/*!
 IGCategoryPoint is a single value, single label point used by all category series and radial series.
 */
@interface IGCategoryPoint : IGDataPoint 

/** A value that determines the data point's numeric value.
 */
@property (nonatomic) double value;

///-------------------
///@name Initializing IGCategoryPoint
///-------------------

/** Initializes IGCategoryPoint with a value and a label.
 @param value A numeric value of the data point.
 @param label A string label of the data point.
 @return Returns an initialized IGCategoryPoint.
 */
-(id)initWithValue:(double) value andLabel:(NSString*)label;
@end



/***** Category Date Point *****/

/*!
 IGCategoryDatePoint is a point used by series displayed on a time scale. IGCategoryDatePoint contains one value, one label and one date.
 */
@interface IGCategoryDatePoint : IGDataPoint 

/** A value that determines the data point's numeric value.
 */
@property (nonatomic) double value;

/** A date that determines the date component of the data point.
 */
@property (nonatomic, retain) NSDate* date;

///---------------------
///@name Initializing IGCategoryDatePoint
///---------------------

/** Initializes IGCategoryDatePoint with a value, a date and a label.
 @param value A numeric value of the data point.
 @param date A date value of the data point.
 @param label A tring label of the data point.
 @return Returns an initialized IGCategoryDatePoint.
 */
-(id)initWithValue:(double)value andDate:(NSDate*)date andLabel:(NSString*)label;
@end



/***** Scatter Point *****/
/*!
 IGScatterPoint is a point used by series whose X and Y axes are numeric. Such series include IGScatterSeries, IGScatterLineSeries, IGScatterSplineSeries. Each point has a pair of numeric values and a label.
 */
@interface IGScatterPoint : IGDataPoint

/** X value of the data point.
 */
@property (nonatomic) double xValue;

/** Y value of the data point.
 */
@property (nonatomic) double yValue;

///--------------------
///@name Initializing IGScatterPoint
///--------------------

/** Initializes IGScatterPoint with a pair of numeric coordinates and a label.
 @param x X coordinate of the data point.
 @param y Y coordinate of the data point.
 @param label A string label of the data point.
 @return Returns an initialized IGScatterPoint.
 */
-(id)initWithX:(double)x andY:(double)y andLabel:(NSString*)label;

@end



/***** Bubble Point *****/
/*!
 IGBubblePoint is a point used by IGBubbleSeries. It is similar to IGScatterPoint with an additional radius property.
 */
@interface IGBubblePoint : IGDataPoint

/** X value of the data point.
 */
@property (nonatomic) double xValue;

/** Y value of the data point.
 */
@property (nonatomic) double yValue;

/** A value that determines the radius of the bubble marker.
 */
@property (nonatomic) double radius;

///--------------------
///@name Initializing IGBubblePoint
///--------------------

/** Initializes IGBubblePoint with a pair of numeric coordinates, a radius and a label.
 @param x X coordinate of the data point.
 @param y Y coordinate of the data point.
 @param radius Radius of the bubble marker.
 @param label A string label of the data point.
 @return Returns an initialized IGScatterPoint.
 */
-(id)initWithX:(double)x andY:(double)y andRadius:(double)radius andLabel:(NSString*)label;

@end



/***** High Low Point *****/
/*!
 IGHighLowPoint is a point used to describe a range. It is commonly used in range series, like IGRangeColumnSeries.
 */
@interface IGHighLowPoint : IGDataPoint

/** A value that determines the high value of the data point.
 */
@property (nonatomic) double highValue;

/** A value that determines the low value of the data point.
 */
@property (nonatomic) double lowValue;

///--------------------
///@name Initializing IGHighLowPoint
///--------------------

/** Initializes IGHighLowPoint with a high value, a low value and a label.
 @param high High value of the data point.
 @param low Low value of the data point.
 @param label A string label of the data point.
 @return Returns an initialized IGHighLowPoint.
 */
-(id)initWithHigh:(double)high andLow:(double)low andLabel:(NSString*)label;
@end



/***** OHLC Point *****/
/*!
 IGOHLCPoint is a data point with open, low, high and close values that are commonly used in financial charts.
 */
@interface IGOHLCPoint : IGDataPoint 

/** A value that determines the open value of the data point.
 */
@property (nonatomic) double openValue;

/** A value that determines the low value of the data point.
 */
@property (nonatomic) double lowValue;

/** A value that determines the high value of the data point.
 */
@property (nonatomic) double highValue;

/** A value that determines the close value of the data point.
 */
@property (nonatomic) double closeValue;

/** A value that determines the volume of the data point.
 */
@property (nonatomic) double volumeValue;

///--------------------
///@name Initializing IGOHLCPoint
///--------------------

/** Initializes IGOHLCPoint with a open, low, high, close values and a label.
 @param open Open value of the data point.
 @param low Low value of the data point.
 @param high High value of the data point.
 @param close Close value of the data point.
 @param label A string label of the data point.
 @return Returns an initialized IGOHLCPoint.
 */
-(id)initWithOpen:(double)open andLow:(double)low andHigh:(double)high andClose:(double)close andLabel:(NSString*)label;

/** Initializes IGOHLCPoint with a open, low, high, close values and a label.
 @param open Open value of the data point.
 @param low Low value of the data point.
 @param high High value of the data point.
 @param close Close value of the data point.
 @param volume Volume value of the data point.
 @param label A string label of the data point.
 @return Returns an initialized IGOHLCPoint.
 */
-(id)initWithOpen:(double)open andLow:(double)low andHigh:(double)high andClose:(double)close andVolume:(double)volume andLabel:(NSString*)label;
@end



/***** Polar Point *****/
/*!
 IGPolarPoint is a data point that has a pair of numeric coordinates. It is used with polar series, such as IGPolarLineSeries.
 */
@interface IGPolarPoint : IGDataPoint

/** A value that determines the value on the IGNumericAngleAxis.
 */
@property (nonatomic) double angleValue;

/** A value that determines the value on the IGNumericRadiusAxis.
 */
@property (nonatomic) double radiusValue;

///--------------------
///@name Initializing IGPolarPoint
///--------------------

/** Initializes IGPolarPoint with a pair of polar coordinates and a label.
 @param angle Angle value of the data point.
 @param radius Radius value of the data point.
 @param label A string label of the data point.
 @return Returns an initialized IGPolarPoint.
 */
-(id)initWithAngle:(double)angle andRadius:(double)radius andLabel:(NSString*)label;
@end