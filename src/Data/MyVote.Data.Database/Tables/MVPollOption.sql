CREATE TABLE [dbo].[MVPollOption]
(
	PollOptionID INT identity(1,1) NOT NULL 
		constraint pk_MVPollOption PRIMARY KEY clustered
	,PollID int not null
		constraint fk_MVPoll_MVPollOption references dbo.MVPoll(PollID)
	,OptionPosition smallint not null
	,OptionText nvarchar(200) not null
)
