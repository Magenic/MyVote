CREATE TABLE [dbo].[MVPollComment]
(
	PollCommentID INT identity(1,1) NOT NULL 
		constraint pk_MVPollComment PRIMARY KEY clustered
	,UserID int not null
		constraint fk_MVUser_MVPollComment references dbo.MVUser(UserID)
	,CommentText nvarchar(1000)
	,CommentDate datetime2, 
    [ParentCommentID] INT NULL, 
    [PollID] INT NOT NULL
		constraint fk_MVPoll_MVPollComment references dbo.MVPoll(PollID), 
    [PollCommentDeletedFlag] BIT NULL, 
    [PollCommentDeletedDate] DATETIME2 NULL 

)
