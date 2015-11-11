create view [dbo].[dimPoll]
	as select
		mvp.PollID
		,mvp.PollQuestion
		,mvp.PollCategoryID
		,mvc.CategoryName as PollCategoryName
		from dbo.MVPoll mvp
			inner join dbo.MVCategory mvc on mvp.PollCategoryID = mvc.CategoryID