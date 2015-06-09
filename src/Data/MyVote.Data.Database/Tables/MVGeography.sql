CREATE TABLE [dbo].[MVGeography]
(
	GeographyKey int identity(1,1) 
		constraint pk_MVGeography primary key clustered,
    [Zip] varchar(50),
    [Primary_City] varchar(50),
    [State] varchar(50),
    [County] varchar(50),
    [TimeZone] varchar(50),
    [Area_Codes] varchar(50),
    [Latitude] varchar(50),
    [Longitude] varchar(50),
    [Estimated_Population] varchar(50),
)
