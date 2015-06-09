CREATE TABLE [dbo].[MVUser]
(
	 UserID INT identity(1,1) NOT NULL 
	 constraint pk_MVUser primary key nonclustered
	,UserName nvarchar(200) not null
	,ProfileID nvarchar(200) null
	,ProfileAuthToken nvarchar(2100) null
	,EmailAddress nvarchar(200) not null
	,FirstName nvarchar(100)
	,LastName nvarchar(100)
	,Gender nchar(1)
	,PostalCode nvarchar(20)
	,BirthDate date
	,UserRoleID int 
		constraint fk_MVUser_MVUserRole references dbo.MVUserRole(UserRoleID)
	,AuditCreateDate datetime2
		constraint df_MVUser_AuditCreateDate default(getutcdate())
	,AuditModifyDate datetime2
)



GO

CREATE UNIQUE CLUSTERED INDEX [cuidx_MVUser_UserName] ON [dbo].[MVUser] (UserName)
