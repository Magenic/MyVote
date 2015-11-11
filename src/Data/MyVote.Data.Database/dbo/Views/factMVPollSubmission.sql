
create view [dbo].[factMVPollSubmission]
as
select 
	PollID
	,UserID
	,cast(convert(varchar, PollSubmissionDate, 112) as int) as DateKey
	,1 as SubmissionCount
from dbo.MVPollSubmission