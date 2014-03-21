CREATE TABLE [dbo].[MVGeography]
(
	GeographyKey int identity(1,1) 
		constraint pk_MVGeography primary key clustered,
    [zip] varchar(50),
    [primary_city] varchar(50),
    [state] varchar(50),
    [county] varchar(50),
    [timezone] varchar(50),
    [area_codes] varchar(50),
    [latitude] varchar(50),
    [longitude] varchar(50),
    [estimated_population] varchar(50),
)
