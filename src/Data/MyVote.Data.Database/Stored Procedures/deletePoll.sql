CREATE PROCEDURE [dbo].[deletePoll]
	@PollID int
AS
	
delete from dbo.MVReportedPollStateLog where PollID = @PollID;

delete from dbo.MVReportedPoll where PollID = @PollID;

delete from dbo.MVPollResponse where PollID = @PollID;

delete from dbo.MVPollSubmission where PollID = @PollID;

delete from dbo.MVPollOption where PollID = @PollID;

delete from dbo.MVPoll where PollID = @PollID;


RETURN 0
