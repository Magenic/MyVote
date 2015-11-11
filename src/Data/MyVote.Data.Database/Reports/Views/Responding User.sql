create view [Reports].[Responding User] 
with schemabinding 
as 
SELECT [UserID] as [User Id]
      ,case [Gender] when '' then 'Unspecified' when 'M' then 'Male' when 'F' then 'Female' else 'Unspecified' end as Gender
      ,[PostalCode] as [Zip Code]
      ,[BirthDate] as [Date of Birth]
	  ,datediff(year,birthdate,getdate()) as Age
  FROM [dbo].[MVUser]