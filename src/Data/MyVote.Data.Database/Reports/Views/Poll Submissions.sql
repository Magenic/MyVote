

create view [Reports].[Poll Submissions] 
with schemabinding 
as
SELECT [PollSubmissionID] as [Poll Submission Id] 
      ,[PollID] as [Poll Id]
      ,[UserID] as [User Id]
      ,[PollSubmissionDate] as [Date]
	  ,convert(int,convert(varchar,PollSubmissionDate,112)) as [Date Id]
      ,[PollSubmissionComment] as [Comment]
  FROM [dbo].[MVPollSubmission]