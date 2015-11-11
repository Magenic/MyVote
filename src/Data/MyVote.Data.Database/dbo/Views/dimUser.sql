create view [dbo].[dimUser]
	as select 
		mvu.UserID
		,mvu.UserName
		,mvu.Gender
		,isnull(mvg.State, 'XX') as State
		,isnull(mvg.Primary_City, 'Unknown') as City
		from dbo.MVUser mvu
			left join dbo.MVGeography mvg on mvu.PostalCode = mvg.Zip