//
//  IGLegend.h
//
//  Copyright (c) 2012 Infragistcs. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "Enums.h"

@class LegendContainer;
@class DataChartContainer;

/*!
 This is the base class for the chart legend. This class should not be allocated.
 */
@interface IGLegendBase : UIView
{
    LegendContainer *_legendContainer;
}

@end


/*!
 IGLegend is a UIView that displays information about chart view's series. This type of legend is the most common and can be used by all series types. The legend displays rows of legend items, with each legend item consisting of an icon and a label. The icon represents the brush of the series associated with the legend item and the label is the title of the series. The legend can be used to display series from multiple charts. When multiple chart views set their legend property to the same legend, all series from those chart views will be displayed in that legend. The legend can also be set from the series. Doing so will take precedence over chart view's legend property. With this legend, one series will display only one legend item. When the series does not have a title set, "Series Title" will be used in the legend item.
 */
@interface IGLegend : IGLegendBase

/* Orientation of the legend view.
 This property determines whether the legend draws its items vertically or horizontally.
 */
@property (nonatomic) IGOrientation orientation;

/** Horizontal alignment of the legend items.
 */
@property(nonatomic) IGHorizontalAlign horizontalAlignment;

/** Vertical alignment of the legend items.
 */
@property(nonatomic) IGVerticalAlign verticalAlignment;
@end
