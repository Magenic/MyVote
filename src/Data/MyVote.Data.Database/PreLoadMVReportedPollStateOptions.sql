-- used to load ReportedPollStateOptions

MERGE dbo.MVReportedPollStateOption as target
USING (select 1, 'New Report', 'New Report'
		union
		select 2, 'Confirmed', 'Admin confirms has confirmed there is an issue with the Poll'
		union
		select 3, 'Dismissed', 'Admin has dismissed the report'
		union 
		select 4, 'Cleared', 'Report has been cleared, poll is reactivated'
		) as source(ReportedPollStateOptionID, ReportedPollStateOptionName, ReportedPollStateOptionComments)
	ON target.ReportedPollStateOptionID = source.ReportedPollStateOptionID
	WHEN matched then
		update set target.ReportedPollStateName = source.ReportedPollStateOptionName
					,target.ReportedPollStateComments = source.ReportedPollStateOptionComments
	WHEN not matched then
		insert (ReportedPollStateOptionID, ReportedPollStateName, ReportedPollStateComments) values (source.ReportedPollStateOptionID, source.ReportedPollStateOptionName, source.ReportedPollStateOptionComments)
;
go
