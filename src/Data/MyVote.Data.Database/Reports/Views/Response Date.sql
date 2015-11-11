
create view [Reports].[Response Date] with schemabinding 
as select 
[Date Id]
,[Date]
,[Day Name]
,[Short Day Name]
,[Year]
,[Quarter]
,[Quarter Name]
,[Short Quarter Name]
,[Month]
,[Month Name]
,[Short Month Name]
,[Week Number]
,[Week Name]
,[Short Week Name]
from Reports.Dates