//
//  Enums.h
//
//  Copyright (c) 2012 Infragistcs. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef enum {
    IGGridModeNone = 0,
    IGGridModeBeforeSeries = 1,
    IGGridModeBehindSeries = 2
} IGGridMode;


typedef enum {
    IGMarkerTypeUnset = 0,
    IGMarkerTypeNone = 1,
    IGMarkerTypeAutomatic = 2,
    IGMarkerTypeCircle = 3,
    IGMarkerTypeTriangle = 4,
    IGMarkerTypePyramid = 5,
    IGMarkerTypeSquare = 6,
    IGMarkerTypeDiamond = 7,
    IGMarkerTypePentagon = 8,
    IGMarkerTypeHexagon = 9,
    IGMarkerTypeTetragram = 10,
    IGMarkerTypePentagram = 11,
    IGMarkerTypeHexagram = 12
} IGMarkerType;


typedef enum {
    IGAxisOrientationHorizontal = 0,
    IGAxisOrientationVertical = 1,
    IGAxisOrientationAngular = 2,
    IGAxisOrientationRadial = 3
} IGAxisOrientation;


typedef enum {
    IGAxisLabelsLocationOutsideTop = 0,
    IGAxisLabelsLocationOutsideBottom = 1,
    IGAxisLabelsLocationOutsideLeft = 2,
    IGAxisLabelsLocationOutsideRight = 3,
    IGAxisLabelsLocationInsideTop = 4,
    IGAxisLabelsLocationInsideBottom = 5,
    IGAxisLabelsLocationInsideLeft = 6,
    IGAxisLabelsLocationInsideRight = 7
} IGAxisLabelsLocation;


typedef enum {
    IGIndicatorDisplayTypeLine = 0,
    IGIndicatorDisplayTypeArea = 1,
    IGIndicatorDisplayTypeColumn = 2
} IGIndicatorDisplayType;


typedef enum {
    IGLabelsPositionNone = 0,
    IGLabelsPositionCenter = 1,
    IGLabelsPositionInsideEnd = 2,
    IGLabelsPositionOutsideEnd = 3,
    IGLabelsPositionBestFit = 4
} IGLabelsPosition;


typedef enum {
    IGOthersCategoryTypeNumber = 0,
    IGOthersCategoryTypePercent = 1
} IGOthersCategoryType;


typedef enum {
    IGPriceDisplayTypeCandlestick = 0,
    IGPriceDisplayTypeOHLC = 1
} IGPriceDisplayType;


typedef enum {
    IGSplineTypeNatural = 0,
    IGSplineTypeClamped = 1
} IGSplineType;


typedef enum {
    IGTimeAxisDisplayTypeContinuous = 0,
    IGTimeAxisDisplayTypeDiscrete = 1
} IGTimeAxisDisplayType;


typedef enum {
    IGTrendLineTypeNone = 0,
    IGTrendLineTypeLinearFit = 1,
    IGTrendLineTypeQuadraticFit = 2,
    IGTrendLineTypeCubicFit = 3,
    IGTrendLineTypeQuarticFit = 4,
    IGTrendLineTypeQuinticFit = 5,
    IGTrendLineTypeLogarithmicFit = 6,
    IGTrendLineTypeExponentialFit = 7,
    IGTrendLineTypePowerLawFit = 8,
    IGTrendLineTypeSimpleAverage = 9,
    IGTrendLineTypeExponentialAverage = 10,
    IGTrendLineTypeModifiedAverage = 11,
    IGTrendLineTypeCumulativeAverage = 12,
    IGTrendLineTypeWeightedAverage = 13
} IGTrendLineType;

typedef enum {
    IGUnknownValuePlottingLinearInterpolate = 0,
    IGUnknownValuePlottingDontPlot = 1
} IGUnknownValuePlotting;

typedef enum {
    IGHorizontalAlignLeft = 0,
    IGHorizontalAlignCenter = 1,
    IGHorizontalAlignRight = 2,
    IGHorizontalAlignStretch = 3
} IGHorizontalAlign;

typedef enum {
    IGVerticalAlignTop = 0,
    IGVerticalAlignCenter = 1,
    IGVerticalAlignBottom = 2,
    IGVerticalAlignStretch = 3
} IGVerticalAlign;

typedef enum{
    IGBrushSelectionModeSelect = 0,
    IGBrushSelectionModeInterpolate = 1
}IGBrushSelectionMode;

typedef enum{
    IGTooltipPinLocationFloating = 0,
    IGTooltipPinLocationTop = 1,
    IGTooltipPinLocationLeft = 2,
    IGTooltipPinLocationBottom = 3,
    IGTooltipPinLocationRight = 4
}IGTooltipPinLocation;

typedef enum{
    IGCrosshairsVisibilityBoth = 0,
    IGCrosshairsVisibilityNone = 1,
    IGCrosshairsVisibilityHorizontal = 2,
    IGCrosshairsVisibilityVertical = 3
}IGCrosshairsVisibility;

typedef enum{
    IGOrientationVertical = 0,
    IGOrientationHorizontal = 1
}IGOrientation;

typedef enum{
    IGChartAntiAliasingAlways,
    IGChartAntiAliasingNever,
    IGChartAntiAliasingAlwaysExceptWhilePanningAndZooming
    
}IGChartAntiAliasing;
