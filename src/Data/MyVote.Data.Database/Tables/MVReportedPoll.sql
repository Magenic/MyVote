CREATE TABLE [dbo].[MVReportedPoll]
(
	ReportedPollID int identity(1,1)
		constraint pk_MVReportedPoll primary key clustered
	,PollID	int not null
		constraint fk_MVPoll_MVReportedPoll references dbo.MVPoll(PollID)
	,ReportedByUserID int not null
		constraint fk_ReportedByUser_MVReportedPoll references dbo.MVUser(UserID)
	,CurrentStateAdminUserID int null
		constraint fk_CurrentStateAdminUser_MVReportedPoll references dbo.MVUser(UserID)
	,DateReported datetime2 not null
		constraint df_DateReported default(getutcdate())
	,DateCurrentStateChanged datetime2 null
	,ReportedPollStateOptionID int not null
		constraint fk_MVReportedPollStateOption_MVReportedPoll references dbo.MVReportedPollStateOption(ReportedPollStateOptionID)
	,ReportComments nvarchar(500)
)
