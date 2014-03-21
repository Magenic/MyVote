create view [dbo].[dimDate]
	as select 
		  DateKey
		  ,DateID
		  ,CalendarYear
		  ,CalendarQuarter
		  ,CalendarQuarterShort
		  ,CalendarMonth
		  ,CalendarMonthShort
	   from dbo.mvdates