
create view [Reports].[Poll Responses] 
with schemabinding 
as
SELECT [PollResponseID] as [Poll Response Id]
      ,[PollID] as [Poll Id]
      ,[PollSubmissionID] as [Poll Submission Id]
      ,[UserID] as [User Id]
      ,[PollOptionID] as [Poll Option Id]
      ,[OptionSelected] as [Option Selected]
      ,[ResponseDate] as [Response Date]
	  ,convert(int,convert(varchar,ResponseDate,112)) as [Date Id]
  FROM [dbo].[MVPollResponse]