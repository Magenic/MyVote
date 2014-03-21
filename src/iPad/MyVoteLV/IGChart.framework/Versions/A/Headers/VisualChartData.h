//
//  VisualChartData.h
//
//  Copyright (c) 2012 Infragistcs. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

@class VisualDataPrimitive;
@class AppearanceDataPrimitive;
@class AppearanceDataLabel;
@class PointsSettings;

@interface VisualChartData : NSObject 
{
    @public
    NSMutableArray* axes;
    NSMutableArray* series;
    NSString* name;
    BOOL isViewportScaled;
}

- (id)initWithGeneratedObject:(id)obj;

- (void)scaleByViewport;

@end

@interface VisualDataAxis : NSObject 
{
@public
    NSString* name;
    NSString* type;
    CGRect viewport;
    NSMutableArray* labels; // VisualDataAxisLabel
    VisualDataPrimitive* axisLine;
    VisualDataPrimitive* majorLines;
    VisualDataPrimitive* minorLines;
}

- (id)initWithGeneratedObject:(id)obj;

@end


@interface VisualDataPrimitive : NSObject 
{
@public
    AppearanceDataPrimitive* appearance;
    NSMutableArray* tags; // Strings
    NSString* name;
    NSString* type;
}

- (id)initWithGeneratedObject:(id)obj;

@end

@interface AppearanceDataPrimitive : NSObject 
{
@public
    UIColor* stroke;
    UIColor* fill;
    double strokeThickness;
    NSInteger visibility;
    double opacity;
    double canvasLeft;
    double canvasTop;
    NSInteger canvaZIndex;
    NSMutableArray *dashArray;
    NSInteger dashCap;
}

- (id)initWithGeneratedObject:(id)obj;

@end

@interface VisualDataAxisLabel : NSObject 
{
@public
    NSObject* labelValue;
    double labelPosition;
    AppearanceDataLabel* appearance;
}

- (id)initWithGeneratedObject:(id)obj;

@end

@interface AppearanceDataLabel : NSObject 
{
@public
    NSString* text;
}

- (id)initWithGeneratedObject:(id)obj;

@end

@interface PointsSettings : NSObject {
@public
    BOOL ignoreFigureStartPoint;
}

@end

@interface VisualDataSeries : NSObject 
{
@public
    NSString* name;
    NSString* type;
    CGRect viewport;
    NSMutableArray* shapes;
    NSMutableArray* markerShapes;
}

- (id)initWithGeneratedObject:(id)obj;

@end


@interface VisualDataMarker : NSObject 
{
@public
    double x;
    double y;
    NSInteger index;
    NSInteger visibility;
}

- (id)initWithGeneratedObject:(id)obj;

@end
