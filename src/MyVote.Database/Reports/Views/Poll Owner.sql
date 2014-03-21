create view [Reports].[Poll Owner] 
with schemabinding 
as 
SELECT [UserID] as [User Id]
      ,[UserName] as [User Name]
      ,[ProfileID] as [Profile Id]
      ,[EmailAddress] as [Email Address]
      ,[FirstName] as [First Name]
      ,[LastName] as [Last Name]
      ,[Gender]
      ,[PostalCode] as [Zip Code]
      ,[BirthDate] as [Date of Birth]
  FROM [dbo].[MVUser]