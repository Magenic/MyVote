

create view [Reports].[Categories] 
with schemabinding 
as 
SELECT [CategoryID] as [Category Id]
      ,[CategoryName] as [Category Name]
  FROM [dbo].[MVCategory]