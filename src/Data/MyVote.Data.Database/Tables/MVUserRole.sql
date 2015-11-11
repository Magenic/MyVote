CREATE TABLE [dbo].[MVUserRole]
(
	[UserRoleID] INT identity(1,1) NOT NULL 
		constraint pk_MVRole PRIMARY KEY clustered
	,[UserRoleName] nvarchar(100) not null
)
