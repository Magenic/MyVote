-- used to load required user roles

if not exists(select 1 from dbo.MVUserRole where UserRoleName = 'User')
	insert dbo.MVUserRole (UserRoleName) values ('User');

go

if not exists(select 1 from dbo.MVUserRole where UserRoleName = 'Admin')
	insert dbo.MVUserRole (UserRoleName) values ('Admin');

go


