CREATE TABLE [dbo].[MVDates]
(
	[DateKey] [int] NOT NULL
		constraint pk_MVDates primary key clustered,
	[DateID] [date] NOT NULL,
	[DayNme] [varchar](50) NOT NULL,
	[DayShort] [char](10) NOT NULL,
	[DayNOD] [tinyint] NOT NULL,
	[CalendarYear] [smallint] NOT NULL,
	[CalendarYearNOD] [smallint] NOT NULL,
	[CalendarQuarter] [int] NOT NULL,
	[CalendarQuarterName] [char](15) NOT NULL,
	[CalendarQuarterShort] [char](7) NOT NULL,
	[CalendarQuarterNOD] [tinyint] NOT NULL,
	[CalendarMonth] [int] NOT NULL,
	[CalendarMonthName] [varchar](20) NOT NULL,
	[CalendarMonthShort] [char](8) NOT NULL,
	[CalendarMonthNOD] [tinyint] NOT NULL,
	[CalendarWeek] [int] NOT NULL,
	[CalendarWeekName] [varchar](20) NOT NULL,
	[CalendarWeekShort] [char](9) NOT NULL,
	[CalendarWeekNOD] [tinyint] NOT NULL,

)
