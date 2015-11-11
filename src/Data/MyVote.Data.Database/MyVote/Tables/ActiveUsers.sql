CREATE TABLE [MyVote].[ActiveUsers] (
    [id]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [AuthnToken]    NVARCHAR (MAX) NULL,
    [containerName] NVARCHAR (MAX) NULL,
    [resourceName]  NVARCHAR (MAX) NULL,
    [sas]           NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

