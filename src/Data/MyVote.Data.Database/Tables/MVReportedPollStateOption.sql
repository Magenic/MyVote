CREATE TABLE [dbo].[MVReportedPollStateOption]
(
	ReportedPollStateOptionID int
		constraint pk_MVReportedPollStateOption primary key clustered
	,ReportedPollStateName nvarchar(50) not null
	,ReportedPollStateComments nvarchar(500) null
)
