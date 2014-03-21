create view [Reports].[Geography] 
with schemabinding 
as
SELECT [GeographyKey] as [Geography Id]
      ,[zip]  as [Zip Code]
      ,[primary_city] as [City]
      ,[state] as [State]
      ,[county] as [County]
      ,[timezone] as [Time Zone]
      ,[area_codes] as [Area Codes]
      ,cast([latitude] as double precision) as [Latitude]
      ,cast([longitude] as double precision) as [Longitude]
      ,cast([estimated_population] as int) as [Population]
  FROM [dbo].[MVGeography]