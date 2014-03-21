EXECUTE sp_addrolemember @rolename = N'db_datawriter', @membername = N'MyVote_AppUser';


GO
EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = N'MyVote_AppUser';


GO
EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = N'MyVote_Izenda';


GO
EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = N'MyVote_Reader';

