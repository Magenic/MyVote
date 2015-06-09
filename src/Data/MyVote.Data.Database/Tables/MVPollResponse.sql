CREATE TABLE [dbo].[MVPollResponse]
(
	PollResponseID INT identity(1,1) NOT NULL 
		constraint pk_MVPollResponse PRIMARY KEY clustered
	,PollID int not null
		constraint fk_MVPoll_MVPollResponse references dbo.MVPoll(PollID)
	,PollSubmissionID int not null
		constraint fk_MVPollSubmission_MVPollResponse references dbo.MVPollSubmission(PollSubmissionID)
	,UserID int not null
		constraint fk_MVUser_MVPollResponse references dbo.MVUser(UserID)
	,PollOptionID int not null
		constraint fk_MVPollOption_MVPollResponse references dbo.MVPollOption(PollOptionID)
	,OptionSelected bit not null
	,ResponseDate datetime2 not null
)
