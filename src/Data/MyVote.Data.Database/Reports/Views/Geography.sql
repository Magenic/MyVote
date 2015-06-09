create view [Reports].[Geography] 
with schemabinding 
as
SELECT [GeographyKey] as [Geography Id]
      ,[Zip]  as [Zip Code]
      ,[Primary_City] as [City]
      ,[State] as [State]
      ,[County] as [County]
      ,[TimeZone] as [Time Zone]
      ,[Area_Codes] as [Area Codes]
      ,cast([Latitude] as double precision) as [Latitude]
      ,cast([Longitude] as double precision) as [Longitude]
      ,cast([Estimated_Population] as int) as [Population]
  FROM [dbo].[MVGeography]