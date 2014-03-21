//
//  Themes.h
//
//  Copyright (c) 2012 Infragistcs. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Core.h"

@interface IGChartPaletteItem : NSObject {    
    IGBrush* _color;
    IGBrush* _outlineColor;
}

@property (nonatomic, retain) IGBrush* color;

@property (nonatomic, retain) IGBrush* outlineColor;

@end

@interface IGChartThemeDefinition : NSObject {
    UIFont* _font;
    IGBrush* _fontColor;
    UIColor *_backgroundColor;
    UIFont* _legendFont;
    IGBrush* _legendFontColor;
    NSMutableArray* _seriesPalette;
    IGChartPaletteItem* _axisPalette;
    IGChartPaletteItem* _legendPalette;
    float _legendBorderThickness;
}

@property (nonatomic, retain) UIFont* font;

@property (nonatomic, retain) IGBrush* fontColor;

@property (nonatomic, retain) UIColor *backgroundColor;

@property (nonatomic, retain) UIFont* legendFont;

@property (nonatomic, retain) IGBrush* legendFontColor;

@property (readonly, nonatomic) NSMutableArray* seriesPalettes;

@property (nonatomic, retain) IGChartPaletteItem* axisPalette;

@property (nonatomic, retain) IGChartPaletteItem* legendPalette;

@property (nonatomic) float legendBorderThickness;

@end


@interface IGChartDefaultThemes : NSObject

+(IGChartThemeDefinition*)DefaultTheme;
+(IGChartThemeDefinition*)IGTheme;
+(IGChartThemeDefinition*)IGThemeDark;
+(IGChartThemeDefinition*)DarkTheme1;
+(IGChartThemeDefinition*)DarkTheme2;
+(IGChartThemeDefinition*)DarkTheme3;
+(IGChartThemeDefinition*)DarkTheme4;
@end