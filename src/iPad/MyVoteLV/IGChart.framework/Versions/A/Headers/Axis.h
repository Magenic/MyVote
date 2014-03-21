//
//  Axis.h
//
//  Copyright (c) 2012 Infragistcs. All rights reserved.
//


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "Enums.h"
#import "Core.h"

@class IGBrush;

/*!
 This is the base class for all axis types of the chart view. This class should not be allocated.
 */
@interface IGAxis : NSObject
{
    NSArray *_strokeDashArray, *_minorStrokeDashArray, *_majorStrokeDashArray;
    IGAxis *_crossingAxis;
    NSString *_key;
}

/** This property specifies the axis identifier key.
 
 A key must be specified on an axis when an axis is initialized.
 */
@property (nonatomic, readonly) NSString *key;

/** Amount of white space in pixels between the axis and the edge of the chart view.
 
 This property determines the amount of space used for axis labels.
 */
@property (nonatomic) float extent;

/** A Boolean value that determines whether the axis labels are visible.
 */
@property (nonatomic) BOOL labelsVisible;

/** A Boolean value that determines whether the axis is visible.
 
 When set to NO, this property hides the axis line, all axis gridlines and labels.
 */
@property (nonatomic) BOOL axisVisible;

/** A Boolean value that determines whether the axis is inverted.
 
 This property can be used to reverse the direction of the axis.
 */
@property (nonatomic) BOOL isInverted;

/** Specifies the axis to intersect with the current axis.
 
 This property is used in conjunction with crossingValue property. The pair of properties can be used to determine which two axes intersect each other at specified values. 
 */
@property (nonatomic, retain) IGAxis *crossingAxis;

/** Specifies the value, at which the current axis will cross its crossing axis. 
 
 This property is used in conjunction with crossingAxis property. When both properties are set, the current axis will use this property to determine the location, at which to cross the crossingAxis. For example, if an X axis has a crossing value of 5, it will intersect Y axis at Y=5. 
 */
@property (nonatomic) float crossingValue;

/** An enumeration value that determines where the labels are displayed.
 
 One of the values from IGAxisLabelsLocation enumeration can be used to set the axis labels placement. The enumeration contains values that can be used for both horizontal and vertical axes. However, only some values can be used with one particular axis type. For example, OutsideBottom, OutsideTop, InsideBottom and InsideTop can be used with a horizontal axis, but not a vertical one. 
 
 When labels are set to be outside, they reside within the axis extent. When the labels are set to be inside, they are positioned immediately above or below the horizontal axis line, or to the right or left of the vertical axis line. If there is not enough space for inside labels to display, they will be placed outside. IGAxisLabelsLocationOutsideBottom is the default for horizontal axes. IGAxisLabelsLocationOutsideLeft is the default for vertical axes.
 */
@property (nonatomic) IGAxisLabelsLocation labelsLocation;

/** The brush used for the axis labels.
 */
@property (nonatomic, retain) IGBrush *labelBrush;

/** The brush used for the axis line.
 */
@property (nonatomic, retain) IGBrush *stroke;

/** The brush used for major grid lines.
 */
@property (nonatomic, retain) IGBrush *majorStroke;

/** The brush used for minor grid lines.
 */
@property (nonatomic, retain) IGBrush *minorStroke;

/** The brush used for axis strips.
 
 Strips are alternating highlighted regions of the axis between major grid lines.
 */
@property (nonatomic, retain) IGBrush *strip;

/** Determines the pattern of the axis line.
 
 The array is a collection of alternating spaces and dashes represented by their length. For example, an array {2, 2} will create a repeating line pattern of 2 pixels of white space followed by a 2 pixel dash.
 */
@property (nonatomic, retain) NSArray *strokeDashArray;

/** Determines the pattern of the major grid lines.
 
 The array is a collection of alternating spaces and dashes represented by their length. For example, an array {2, 2} will create a repeating line pattern of 2 pixels of white space followed by a 2 pixel dash.
 */
@property (nonatomic, retain) NSArray *majorStrokeDashArray;

/** Determines the pattern of the minor grid lines.
 
 The array is a collection of alternating spaces and dashes represented by their length. For example, an array {2, 2} will create a repeating line pattern of 2 pixels of white space followed by a 2 pixel dash.
 */
@property (nonatomic, retain) NSArray *minorStrokeDashArray;

/** A value that determines the thickness (in pixels) of the axis line.
 */
@property (nonatomic) float strokeThickness;

/** A value that determines the thickness (in pixels) of the major grid lines.
 */
@property (nonatomic) float majorStrokeThickness;

/** A value that determines the thickness (in pixels) of the minor grid lines.
 */
@property (nonatomic) float minorStrokeThickness;

/** Initializes the axis with a string key.
 
 @param key The string identifier for this axis.
 */
-(id)initWithKey:(NSString*)key;
@end



/***** Category Axis Base *****/
/*!
 This is a base class for all category axes. This class should not be allocated.
 */
@interface IGCategoryAxisBase : IGAxis

/** A value that determines the amount of space in pixels between adjacent categories.
 
 This property is used with column-based series, such as IGColumnSeries. It can be used to specify how much white space is added between two adjacent columns.
 */
@property (nonatomic) float gap;

/** A value that determines the amount of overlap in pixels between adjacent categories.
 
 This property is used with column-based series, such as IGColumnSeries. It can be used to specify how much the two adjacent columns overlap one another.
 */
@property (nonatomic) float overlap;
@end



/***** Numeric Axis Base *****/
/*!
 This is a base class for all numeric axes. This class should not be allocated.
 */
@interface IGNumericAxisBase : IGAxis

/** A Boolean value that determines whether the axis is logarithmic. (read-only)
 
 This property returns YES if the axis uses a logarithmic scale. This property will return NO if the axis was set to use a linear scale, or when a logarithmic scale could not be set.
 */
@property (nonatomic, readonly) BOOL actualIsLogarithmic;

/** Returns the maximum visible value on the axis. (read-only)
 
 This property will return the final maximum visible value after the chart has made various range adjustments. Adjustments can be made as a result of rounding or auto-calculating the axis range.
 */
@property (nonatomic, readonly) float actualMaximumValue;

/** Returns the minimum visible value on the axis. (read-only)
 
 This property will return the final minimum visible value after the chart has made various range adjustments. Adjustments can be made as a result of rounding or auto-calculating the axis range.
 */
@property (nonatomic, readonly) float actualMinimumValue;

/** A value that determines the axis minimum.
 */
@property (nonatomic) float minimum; 

/** A value that determines the axis maximum.
 */
@property (nonatomic) float maximum;

/** A value that determines the axis interval.
 
 The axis interval determines the values and the frequency of axis labels.
 */
@property (nonatomic) float interval;

/** A Boolean value that determines whether the axis uses a logarithmic scale.
 */
@property (nonatomic) BOOL isLogarithmic;

/** A value that determines the logarithm base.
 
 This property is used to specify the logarithm base to use when the axis uses a logarithmic scale.
 */
@property (nonatomic) int logarithmBase;

/** A value that determines the starting rendering point for series polygons.
 
 This property is only used by certain column and area series. The value of this property is used by the series as the base starting point for drawing shapes. For example, a column of a column series typically starts drawing at Y=0 and finishes at some value. This property will make the column start drawing at reference value insted of 0.
 */
@property (nonatomic) float referenceValue;
@end



/***** Category X Axis *****/
/*!
 A IGCategoryXAxis is a horizontal axis that is used to display a sequence of category labels.
 
 IGCategoryXAxis is the most commonly used axis. It is used by almost all IGAnchoredCategorySeries and by all IGFinancialSeries. IGCategoryXAxis is represented by a horizontal line and a set of labels. 
 
 IGCategoryXAxis contains vertical major and minor grid lines. Their behavior will vary for different series types. For column-based series, major grid lines are placed between columns, while minor grid lines are placed in the middle of each column. For line and area series, major grid lines are placed in the middle of each category, while minor grid lines are not displayed.
 
 Axis labels can be placed in one of the four locations: outside below the plotting area, inside the plotting area below the axis line, inside the plotting area above the axis line, and outside above the plotting area. It's common to set crossingAxis and crossingValue when setting labelsLocation to an inside value.
 
 CrossingAxis and crossingValue allow the axis to intersect the NumericYAxis at a specific value.
 */
@interface IGCategoryXAxis : IGCategoryAxisBase 

/** An enumeration value that specifies the label alignment.
 
 This property is used to set the alignment of the axis labels. The labels can be top, center, or bottom aligned.
 */
@property (nonatomic) IGVerticalAlign labelAlignment;

/** Angle of rotation (in degrees) used by the axis labels.
 */
@property (nonatomic) float labelOrientationAngle;

/** A value that determines the frequency of displayed labels.
 
 This property is used to set the frequency of axis labels. A value of 1 will display every label. A value of 2 will display every other label, and so on. When unset, the axis will automatically determine the label frequency.
 */
@property (nonatomic) float interval;
@end



/***** Category Y Axis *****/
/*!
 A IGCategoryYAxis is a vertical axis that is used to display a sequence of category labels.
 
 IGCategoryYAxis is used by IGBarSeries. IGCategoryYAxis is represented by a vertical line and a set of labels. Labels are typically extracted from series datapoints.
 
 IGCategoryYAxis contains horizontal major and minor grid lines. Major grid lines are placed between columns, while minor grid lines are placed in the middle of each column.
 
 Axis labels can be placed in one of the four locations: outside left of the plotting area, inside the plotting area left of the axis line, inside the plotting area right of the axis line, and outside right of the plotting area. It's common to set crossingAxis and crossingValue when setting labelsLocation to an inside value.
 
 CrossingAxis and crossingValue allow the axis to intersect the NumericXAxis at a specific value.
 */
@interface IGCategoryYAxis : IGCategoryAxisBase

/** An enumeration value that specifies the label alignment.
 
 This property is used to set the alignment of the axis labels. The labels can be left, center, or right aligned.
 */
@property (nonatomic) IGHorizontalAlign labelAlignment;

/** Angle of rotation (in degrees) used by the axis labels.
 */
@property (nonatomic) float labelOrientationAngle;

/** A value that determines the frequency of displayed labels.
 
 This property is used to set the frequency of axis labels. A value of 1 will display every label. A value of 2 will display every other label, and so on. When unset, the axis will automatically determine the label frequency.
 */
@property (nonatomic) float interval;
@end



/***** Numeric X Axis *****/
/*!
 A IGNumericXAxis is a horizontal axis that is used to display a sequence of numeric labels. This axis type is most commonly used by scatter and bubble series. It is also used by the bar series. The range of the axis is determined by the data used in the series. The range can also be set excplicitly via minimum and maximum properties. The axis can use a linear or a logarithmic scale.
 
 IGNumericXAxis contains vertical major and minor grid lines. Major grid lines are placed for each label. The number of minor grid lines depends on the size of the axis. The range between two major grid lines is typically split up into 2, 5 or 10 sections. Minor grid lines are used to denote these sections.
 
 Axis labels can be placed in one of the four locations: outside below the plotting area, inside the plotting area below the axis line, inside the plotting area above the axis line, and outside above the plotting area. It's common to set crossingAxis and crossingValue when setting labelsLocation to an inside value.
 
 CrossingAxis and crossingValue allow the axis to intersect the Y axis at a specific value.
 */
@interface IGNumericXAxis : IGNumericAxisBase 

/** An enumeration value that specifies the label alignment.
 
 This property is used to set the alignment of the axis labels. The labels can be top, center, or bottom aligned.
 */
@property (nonatomic) IGVerticalAlign labelAlignment;

/** Angle of rotation (in degrees) used by the axis labels.
 */
@property (nonatomic) float labelOrientationAngle;
@end



/***** Numeric Y Axis *****/
/*!
 A IGNumericYAxis is a vertical axis that is used to display a sequence of numeric labels. This axis type is used by nearly all series. The range of the axis is determined by the data used in the series. The range can also be set excplicitly via minimum and maximum properties. The axis can use a linear or a logarithmic scale.
 
 IGNumericYAxis contains horizontal major and minor grid lines. Major grid lines are placed for each label. The number of minor grid lines depends on the size of the axis. The range between two major grid lines is typically split up into 2, 5 or 10 sections. Minor grid lines are used to denote these sections.
 
 Axis labels can be placed in one of the four locations: outside left of the plotting area, inside the plotting area left of the axis line, inside the plotting area right of the axis line, and outside right of the plotting area. It's common to set crossingAxis and crossingValue when setting labelsLocation to an inside value.
 
 CrossingAxis and crossingValue allow the axis to intersect the X axis at a specific value.
 */
@interface IGNumericYAxis : IGNumericAxisBase 

/** An enumeration value that specifies the label alignment.
 
 This property is used to set the alignment of the axis labels. The labels can be left, center, or right aligned.
 */
@property (nonatomic) IGHorizontalAlign labelAlignment;

/** Angle of rotation (in degrees) used by the axis labels.
 */
@property (nonatomic) float labelOrientationAngle;
@end



/***** Category DateTime X Axis *****/
/*!
 IGCategoryDateTimeXAxis is a horizontal axis used to display date labels. It is commonly used with line and area series, where the data contains dates. This axis is similar to IGCategoryXAxis. The main difference is that categories do not have to be evenly spaced.
 
 IGCategoryDateTimeXAxis contains vertical major and minor grid lines. Major grid lines are placed in the middle of each category, while minor grid lines are not displayed.
 
 Axis labels can be placed in one of the four locations: outside below the plotting area, inside the plotting area below the axis line, inside the plotting area above the axis line, and outside above the plotting area. It's common to set crossingAxis and crossingValue when setting labelsLocation to an inside value.
 
 CrossingAxis and crossingValue allow the axis to intersect the NumericYAxis at a specific value.
 */
@interface IGCategoryDateTimeXAxis : IGCategoryAxisBase

/** A date value indicating the minimum visible date on the axis. (read-only)
 
 This property returns the smallest date value on the axis.
 */
@property (nonatomic, readonly) NSDate *actualMinimumValue;

/** A date value indicating the maximum visible date on the axis. (read-only)
 
 This property returns the largest date value on the axis.
 */
@property (nonatomic, readonly) NSDate *actualMaximumValue;

/** An enumeration value that specifies the label alignment.
 
 This property is used to set the alignment of the axis labels. The labels can be top, center, or bottom aligned.
 */
@property (nonatomic) IGVerticalAlign labelAlignment;

/** Angle of rotation (in degrees) used by the axis labels.
 */
@property (nonatomic) float labelOrientationAngle;

/** A date value that determines the minimum value on the axis.
 */
@property (nonatomic, retain) NSDate* minimum;

/** A date value that determines the maximum value on the axis.
 */
@property (nonatomic, retain) NSDate* maximum;

/** A numeric value that determines the axis interval.
 
 The axis interval determines the values and the frequency of axis labels.
 */
@property (nonatomic) NSTimeInterval interval;

/** An enumeration value that determines the display type of the axis.
 
 When set to IGTimeAxisDisplayTypeContinuous (default), the axis will be evenly divided for axis labels and the interval between a pair of adjacent labels will be the same through out the axis.
 
 When set to IGTimeAxisDisplayTypeDiscreet, the labels will be placed at the locations of data points. The interval between pairs of points will likely vary.
 */
@property (nonatomic) IGTimeAxisDisplayType displayType;
@end



/***** Numeric Angle Axis *****/
/*!
 IGNumericAngleAxis is a polar axis used by polar series, such as IGPolarScatterSeries. IGNumericAngleAxis is used in conjunction with IGNumericRadiusAxis as part of the requirement for polar series. This axis displays numeric labels in a clockwise fashion. The labels can be displayed counter clockwise by setting isInverted to YES. 
 
 IGNumericAngleAxis supports linear and logarithmic scales. 
 */
@interface IGNumericAngleAxis : IGNumericAxisBase

/** A value (in degrees) that determines the placement of the first label.
 
 This property can be used to offset the 0th angle. This doesn't affect the total number of labels, only the starting point of the labels.
 */
@property (nonatomic) float startAngleOffset;
@end



/***** Numeric Radius Axis *****/
/*!
 IGNumericRadiusAxis is a numeric axis used by polar and radial series. It is displayed along the radius of the series. It supports inversion, logarithmic and linear scales and extent scales.
 */
@interface IGNumericRadiusAxis : IGNumericAxisBase

/** A value that determines the maximum radius.
 
 This property determines the percentage of the maximum radius extent to use as the maximum radius. The value should be between 0 and 1.
 */
@property (nonatomic) float radiusExtentScale;

/** A value that determines the how far off-center the radius axis begins.
 
 This property determines the percentage of the maximum radius extent to leave blank at the center of the chart. The value should be between 0 and 1.
 */
@property (nonatomic) float innerRadiusExtentScale;
@end



/***** Category Angle Axis *****/
/*!
 IGCategoryAngleAxis is a category axis used by radial series, such as IGRadialColumnSeries. IGCategoryAngleAxis is used in conjunction with IGNumericRadiusAxis as part of the requirement for radial series. This axis displays category labels in a clockwise fashion. The labels can be displayed counter clockwise by setting isInverted to YES. 
 */
@interface IGCategoryAngleAxis : IGCategoryAxisBase

/** A value that determines the frequency of displayed labels.
 
 This property is used to set the frequency of axis labels. A value of 1 will display every label. A value of 2 will display every other label, and so on. When unset, the axis will automatically determine the label frequency.
 */
@property (nonatomic) float interval;

/** A value (in degrees) that determines the placement of the first label.
 
 This property can be used to offset the 0th angle. This doesn't affect the total number of labels, only the starting point of the labels.
 */
@property (nonatomic) float startAngleOffset;
@end