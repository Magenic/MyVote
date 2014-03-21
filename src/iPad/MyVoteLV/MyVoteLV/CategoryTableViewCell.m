//
//  CategoryTableViewCell.m
//  DuelScrollingCollectionView
//
//  Created by Kasey Schindler on 3/14/13.
//  Copyright (c) 2013 Kasey Schindler. All rights reserved.
//

#import "CategoryTableViewCell.h"
#import "PollCollectionViewCell.h"

@implementation CategoryTableViewCell

- (void)layoutSubviews
{
    [super layoutSubviews];
    
    // custom
    self.collectionView.frame = self.contentView.bounds;
}

- (void)setCollectionViewDataSourceAndDelegate:(id<UICollectionViewDataSource, UICollectionViewDelegate>)dataSourceDelegate index:(NSInteger)index
{
    self.collectionView.dataSource = dataSourceDelegate;
    self.collectionView.delegate = dataSourceDelegate;
    self.collectionView.index = index;
    
    [self.collectionView reloadData];
}

@end
