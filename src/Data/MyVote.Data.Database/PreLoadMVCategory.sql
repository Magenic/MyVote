-- Update or load new categories

merge dbo.MVCategory as target
using (
		select 1, 'Fun'
		union
		select 2, 'Technology'
		union
		select 3, 'Entertainment'
		union
		select 4, 'News'
		union
		select 5, 'Sports'
		union
		select 6, 'Off-Topic'
	) as source (CategoryID, CategoryName)
	on target.CategoryID = source.CategoryID
when matched then
	update set target.CategoryName = source.CategoryName
when not matched then 
	insert (CategoryID, CategoryName) values (source.CategoryID, source.CategoryName)
;
go