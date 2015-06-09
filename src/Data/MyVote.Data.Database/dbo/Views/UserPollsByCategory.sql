create view [dbo].[UserPollsByCategory]
as
select  UserName
  , [Fun]
  , [Sports]
  , [Technology]
  , [Off-Topic]
  , [Entertainment]
from
 (select p.PollID, c.CategoryName, u.UserName
     from dbo.MVPoll p
     inner join dbo.MVCategory c on p.PollCategoryID = c.CategoryID
     inner join dbo.MVUser u on p.UserID = u.UserID
  ) as sourcetable
pivot
 (count(PollID)
   for CategoryName in ([Fun]
  , [Sports]
  , [Technology]
  , [Off-Topic]
  , [Entertainment])
) as pivottable
;