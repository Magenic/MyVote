//
//  Core.h
//
//  Copyright (c) 2012 Infragistcs. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>
#import "Enums.h"

/*!
 IGBrush is a solid color brush used by the chart view. This brush can only have one color. 
 */
@interface IGBrush : NSObject
{
    float _r, _g, _b, _a;
}
@property (nonatomic,readonly) float red;
@property (nonatomic,readonly) float green;
@property (nonatomic,readonly) float blue;
@property (nonatomic,readonly) float alpha;

///--------------
///@name Initializing IGBrush
///--------------

/** Creates a new brush with red, green, blue and alpha components.
 @param r Red component of the brush. This value should be between 0 and 1.
 @param g Green component of the brush. This value should be between 0 and 1.
 @param b Blue component of the brush. This value should be between 0 and 1.
 @param a Alpha component of the brush. This value should be between 0 and 1.
 @return Returns an initialized instance of IGBrush.
 */
-(id)initWithR:(CGFloat)r andG:(CGFloat)g andB:(CGFloat)b andA:(CGFloat)a;

/** Creates a new brush from UIColor.
 @param color UIColor that will be used to create the brush.
 @return Returns an initialized instance of IGBrush.
 */
-(id)initWithColor:(UIColor*)color;

/** Compares two brushes and returns YES if they represent the same color; otherwise, returns NO.
 */
-(BOOL)isEqualToBrush:(IGBrush*)brush;
@end


/*!
 A size scale is used to relate sizes of multiple objects. Such scale is commonly used by IGBubbleSeries radiusScale property in order to scale bubble markers.
 */
@interface IGSizeScale: NSObject
{
    float _minimum, _maximum;
    BOOL _isLogarithmic;
    int _logBase;
}

/** A value that determines the minimum value of the scale.
 */
@property (nonatomic) float minimum;

/** A value that determines the maximum value of the scale.
 */
@property (nonatomic) float maximum;

/** A value that determines the logarithm base of the scale, when isLogarithmic property is set to YES.
 */
@property (nonatomic) int logarithmBase;

/** A Boolean value that determines whether this size scale uses a logarithmic scale.
 */
@property (nonatomic) BOOL isLogarithmic;
@end


/*!
 A brush scale is used to relate brushes of multiple objects. Such scale is commonly used by IGBubbleSeries brushScale property. 
 */
@interface IGBrushScale : IGSizeScale
{
    NSMutableArray *_brushes;
}

///----------
///@name Configuring IGBrushScale
///----------

/** Adds a brush to the brush scale.
 @param brush Brush to be added to the brush scale.
 */
-(void)addBrush:(IGBrush*)brush;

/** Removes a specified brush from the brush scale.
 @param brush Brush to be removed from the brush scale.
 */
-(void)removeBrush:(IGBrush*)brush;
@end


/*!
 A custom palette brush scale is as a collection of brushes that can be applied as a repeating or an interpolated pattern. The brushSelectionMode property determines how the scale applies brushes to shapes. This class is used by IGBubbleSeries brushScale property. IGCustomPaletteBrushScale uses the index of a bubble marker to select a brush from the brushes collection.
 */
@interface IGCustomPaletteBrushScale: IGBrushScale
{
    IGBrushSelectionMode _brushSelectionMode;
}

/** An enumeration value that determines the scaling mode.
 
 When set to IGBrushSelectionModeSelect, brushes are applied in a sequential repeating pattern. When set to IGBrushSelectionModeInterpolate, brushes are interpolated based on the bubble's index and the number of brushes in the collection..
 */
@property (nonatomic) IGBrushSelectionMode brushSelectionMode;
@end
