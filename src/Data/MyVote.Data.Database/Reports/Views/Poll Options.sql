
create view [Reports].[Poll Options] 
with schemabinding 
as
SELECT [PollOptionID] as [Poll Option Id]
      ,[PollID] as [Poll Id]
      ,[OptionPosition] as [Option Position]
      ,[OptionText] as [Option Text]
  FROM [dbo].[MVPollOption]