CREATE TABLE [dbo].[MVPollSubmission]
(
	PollSubmissionID INT identity(1,1) NOT NULL 
		constraint pk_MVPollSubmission PRIMARY KEY clustered
	,PollID int not null
		constraint fk_MVPoll_MVPollSubmission references dbo.MVPoll(PollID)
	,UserID int not null
		constraint fk_MVUser_MVPollSubmission references dbo.MVUser(UserID)
	,PollSubmissionDate datetime2 not null
	,PollSubmissionComment nvarchar(1000) null
)

