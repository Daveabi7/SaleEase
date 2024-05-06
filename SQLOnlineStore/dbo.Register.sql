CREATE TABLE [dbo].[Register] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [login_user]    NVARCHAR (50) NOT NULL,
    [password_user] NVARCHAR (50) NOT NULL,
    [is_admin] BIT NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

