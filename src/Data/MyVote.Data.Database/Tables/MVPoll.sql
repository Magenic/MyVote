CREATE TABLE [dbo].[MVPoll]
(
	PollID INT identity(1,1) NOT NULL 
		constraint pk_MVPoll PRIMARY KEY clustered
	,UserID int not null
		constraint fk_MVPoll_MVUser references dbo.MVUser(UserID)
	,PollCategoryID int not null
	,PollQuestion nvarchar(1000) not null
	,PollDescription nvarchar(max) null
	--,PollImage nvarbinary() use link to table storage?
	,PollImageLink nvarchar(500) null
	,PollMaxAnswers smallint 
	,PollMinAnswers smallint
	,PollStartDate datetime2 NOT NULL
	,PollEndDate datetime2 NOT NULL
	,PollAdminRemovedFlag bit
	,PollDateRemoved datetime2
	,PollDeletedFlag bit
	,PollDeletedDate datetime2
	,AuditDateCreated datetime2
	,AuditDateModified datetime2, 
    CONSTRAINT [FK_MVPoll_MVCategory] FOREIGN KEY ([PollCategoryID]) REFERENCES [MVCategory]([CategoryID])
)
