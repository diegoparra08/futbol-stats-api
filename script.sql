IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Matches] (
    [Id] int NOT NULL IDENTITY,
    [MatchDate] datetime2 NOT NULL,
    [Location] nvarchar(max) NOT NULL,
    [TeamAScore] int NOT NULL,
    [TeamBScore] int NOT NULL,
    CONSTRAINT [PK_Matches] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Players] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Nickname] nvarchar(max) NOT NULL,
    [PhotoUrl] nvarchar(max) NULL,
    [PreferredFoot] nvarchar(max) NOT NULL,
    [OverallRating] float NOT NULL,
    [Age] int NOT NULL,
    [Height] float NOT NULL,
    CONSTRAINT [PK_Players] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Role] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Goals] (
    [Id] int NOT NULL IDENTITY,
    [Minute] int NULL,
    [MatchId] int NOT NULL,
    [PlayerId] int NOT NULL,
    CONSTRAINT [PK_Goals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Goals_Matches_MatchId] FOREIGN KEY ([MatchId]) REFERENCES [Matches] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Goals_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [MatchDetails] (
    [Id] int NOT NULL IDENTITY,
    [Team] int NOT NULL,
    [MatchId] int NOT NULL,
    [PlayerId] int NOT NULL,
    CONSTRAINT [PK_MatchDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MatchDetails_Matches_MatchId] FOREIGN KEY ([MatchId]) REFERENCES [Matches] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MatchDetails_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [PlayerPositions] (
    [Id] int NOT NULL IDENTITY,
    [PositionName] int NOT NULL,
    [PlayerId] int NOT NULL,
    CONSTRAINT [PK_PlayerPositions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlayerPositions_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Ratings] (
    [Id] int NOT NULL IDENTITY,
    [Speed] int NOT NULL,
    [Shooting] int NOT NULL,
    [Passing] int NOT NULL,
    [Dribbling] int NOT NULL,
    [Defending] int NOT NULL,
    [Physicality] int NOT NULL,
    [Strength] int NOT NULL,
    [Goalkeeping] int NOT NULL,
    [UserId] int NOT NULL,
    [PlayerId] int NOT NULL,
    CONSTRAINT [PK_Ratings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Ratings_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Ratings_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Goals_MatchId] ON [Goals] ([MatchId]);
GO

CREATE INDEX [IX_Goals_PlayerId] ON [Goals] ([PlayerId]);
GO

CREATE INDEX [IX_MatchDetails_MatchId] ON [MatchDetails] ([MatchId]);
GO

CREATE INDEX [IX_MatchDetails_PlayerId] ON [MatchDetails] ([PlayerId]);
GO

CREATE INDEX [IX_PlayerPositions_PlayerId] ON [PlayerPositions] ([PlayerId]);
GO

CREATE INDEX [IX_Ratings_PlayerId] ON [Ratings] ([PlayerId]);
GO

CREATE INDEX [IX_Ratings_UserId] ON [Ratings] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260529204235_InitialCreate', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Players]') AND [c].[name] = N'OverallRating');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Players] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Players] DROP COLUMN [OverallRating];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260601221839_RemoveOverallRatingFromPlayer', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Players] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260602144910_AddIsActiveToPlayer', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [MatchDetails] ADD [FoulsCommitted] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [MatchDetails] ADD [Recoveries] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [MatchDetails] ADD [Tackles] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Goals] ADD [AssistedByPlayerId] int NULL;
GO

ALTER TABLE [Goals] ADD [IsFreeKick] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Goals] ADD [IsPenalty] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

CREATE INDEX [IX_Goals_AssistedByPlayerId] ON [Goals] ([AssistedByPlayerId]);
GO

ALTER TABLE [Goals] ADD CONSTRAINT [FK_Goals_Players_AssistedByPlayerId] FOREIGN KEY ([AssistedByPlayerId]) REFERENCES [Players] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260603205559_AddPenaltyAndFreeKickToGoals', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Matches] ADD [Status] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260604163231_AddStatusToMatch', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Ratings] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '2026-06-10T00:00:00.0000000Z';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260710154948_AddCreatedAtToRating', N'8.0.11');
GO

COMMIT;
GO

