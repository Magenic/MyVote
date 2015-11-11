CREATE TABLE [dbo].[MVReportedPollStateLog]
(
	ReportedPollStateLogID int identity(1,1)
		constraint pk_MVReportedPollStateLog primary key clustered
	,ReportedPollID int not null
		constraint fk_MVReportedPoll_MVReportedPollStateLog references dbo.MVReportedPoll(ReportedPollID)
	,PollID int not null 
		constraint fk_MVPoll_MVReportedPollStateLog references dbo.MVPoll(PollID)
	,StateAdminUserID int not null
		constraint fk_StateAdminUser_MVReportedPollStateLog references dbo.MVUser(UserID)
	,DateStateChanged datetime2 not null
	,StateChangeComments nvarchar(500) null
)
