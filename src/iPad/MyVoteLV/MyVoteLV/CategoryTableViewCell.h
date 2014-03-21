//
//  CategoryTableViewCell.h
//  DuelScrollingCollectionView
//
//  Created by Kasey Schindler on 3/14/13.
//  Copyright (c) 2013 Kasey Schindler. All rights reserved.
//

#import "IndexedCollectionView.h"

@interface CategoryTableViewCell : UITableViewCell

@property (nonatomic, weak) IBOutlet IndexedCollectionView *collectionView;
@property (nonatomic, weak) IBOutlet UILabel *categoryTitleLabel;

- (void)setCollectionViewDataSourceAndDelegate:(id<UICollectionViewDataSource, UICollectionViewDelegate>)dataSourceDelegate index:(NSInteger)index;

@end
