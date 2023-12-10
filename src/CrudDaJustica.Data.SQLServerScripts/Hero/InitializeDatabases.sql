-- Initializes the CrudDaJustica databases: one for development and another for production.
-- This script requires permission to create databases, views, procedures, logins and users.
-- The lack of error messages indicates that it was successfully executed.


-- Initialize the database to use in production 
IF DB_ID('CrudDaJustica') IS NULL
    CREATE DATABASE CrudDaJustica

USE CrudDaJustica
GO

-- CHAR(32) = space
-- CHAR(9) = tab
-- CHAR(10) = new line / line feed 
-- CHAR(13) = carriage return
IF OBJECT_ID('Hero') IS NULL
    CREATE TABLE Hero (
	    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
		Alias VARCHAR(30) NOT NULL 
			CONSTRAINT CHK_AliasIsntEmpty_Hero
            CHECK (TRIM(BOTH CHAR(32) + CHAR(9) + CHAR(10) + CHAR(13) FROM Alias) <> ''),
        Debut DATE NOT NULL,
        FirstName VARCHAR(15) NOT NULL 
			CONSTRAINT CHK_FirstNameIsntEmpty_Hero
            CHECK (TRIM(BOTH CHAR(32) + CHAR(9) + CHAR(10) + CHAR(13) FROM FirstName) <> ''),
        LastName VARCHAR(15) NOT NULL 
			CONSTRAINT CHK_LastNameIsntEmpty_Hero
			CHECK (TRIM(BOTH CHAR(32) + CHAR(9) + CHAR(10) + CHAR(13) FROM LastName) <> ''),
            )
GO

IF OBJECT_ID('EventLog') IS NULL
	CREATE TABLE EventLog (
		Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
		LogMsg VARCHAR(512) NOT NULL,
    )
GO

CREATE OR ALTER VIEW HeroInformation
AS 
	SELECT Id, Alias, Debut, FirstName, LastName
	FROM Hero
GO

CREATE OR ALTER VIEW DebutAge
WITH SCHEMABINDING
AS 
	SELECT
	Id,
	Alias,
	Debut,
	(CASE WHEN YEAR(Debut) <= 1938 THEN 'Platinum Age'
		  WHEN YEAR(Debut) <= 1956 THEN 'Golden Age'
		  WHEN YEAR(Debut) <= 1970 THEN 'Silver Age'
		  WHEN YEAR(Debut) <= 1985 THEN 'Bronze Age'
		  ELSE 'Modern Age'
		  END) AS Age
	FROM dbo.Hero
GO


CREATE OR ALTER PROCEDURE LogError
AS
	BEGIN

	SET NOCOUNT ON

	DECLARE @errorNum INT = ERROR_NUMBER(),
			@errorMsg NVARCHAR(1) = ERROR_MESSAGE(),
			@errorSeverity INT = ERROR_SEVERITY(),
			@errorState INT = ERROR_STATE(),
			@errorProc NVARCHAR(1) = ERROR_PROCEDURE(),
			@errorLine INT = ERROR_LINE(),
			@errorId UNIQUEIDENTIFIER = NEWID(),
			@logMsg VARCHAR(512)

	SET @logMsg = FORMATMESSAGE('MsgId %s. ' + @errorMsg + ' Severity %d. State %d. Procedure %s. Line %d',
		CAST(@errorId AS CHAR(36)),
		@errorSeverity,
		@errorState,
		@errorProc,
		@errorLine)

	INSERT INTO EventLog (Id, LogMsg)
	VALUES (@errorId, @logMsg)

	EXECUTE xp_logevent 100000, logMsg, @errorSeverity

END
GO

CREATE OR ALTER PROCEDURE InsertHero
@Id UNIQUEIDENTIFIER = NULL,
@Alias VARCHAR(30),
@Debut DATE,
@FirstName VARCHAR(15),
@LastName VARCHAR(15)
AS
BEGIN

    DECLARE @rowsAffected int = 0,
            @trimmedAlias VARCHAR(30) = TRIM(@Alias),
            @trimmedFirstName VARCHAR(15) = TRIM(@FirstName),
            @trimmedLastName VARCHAR(15) = TRIM(@LastName)

    BEGIN TRY
        BEGIN TRANSACTION

        INSERT 
            INTO Hero (Alias, Debut, FirstName, LastName)
            VALUES (@trimmedAlias, @Debut, @trimmedFirstName, @trimmedLastName)

        IF @@TRANCOUNT > 0
            SET @rowsAffected = @@ROWCOUNT
            COMMIT
    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK

        EXECUTE LogError
    END CATCH

    RETURN @rowsAffected

END
GO

CREATE OR ALTER PROCEDURE UpdateHero
@Id UNIQUEIDENTIFIER,
@Alias VARCHAR(30),
@Debut DATE,
@FirstName VARCHAR(15),
@LastName VARCHAR(15)
AS
BEGIN

    DECLARE @rowsAffected int = 0,
            @trimmedAlias VARCHAR(30) = TRIM(@Alias),
            @trimmedFirstName VARCHAR(15) = TRIM(@FirstName),
            @trimmedLastName VARCHAR(15) = TRIM(@LastName)

    BEGIN TRY
        BEGIN TRANSACTION

        UPDATE HERO
        SET Alias = @trimmedAlias,
            Debut = @Debut,
            FirstName = @trimmedFirstName,
            LastName = @trimmedLastName
        WHERE Id = @Id

        IF @@TRANCOUNT > 0
            SET @rowsAffected = @@ROWCOUNT
            COMMIT
    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK

        EXECUTE LogError
    END CATCH

    RETURN @rowsAffected

END
GO

CREATE OR ALTER PROCEDURE DeleteHero
@Id UNIQUEIDENTIFIER
AS
BEGIN
    
    DECLARE @rowsAffected int = 0

    BEGIN TRY
        BEGIN TRANSACTION
        
        DELETE FROM Hero WHERE Id = @Id

        IF @@TRANCOUNT > 0
            SET @rowsAffected = @@ROWCOUNT
            COMMIT

    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK
        
        EXECUTE LogError
    END CATCH

    RETURN @rowsAffected

END
GO

-- Initialize the database to use during development 
IF DB_ID('CrudDaJusticaDev') IS NULL
    CREATE DATABASE CrudDaJusticaDev

USE CrudDaJusticaDev
GO


-- CHAR(32) = space
-- CHAR(9) = tab
-- CHAR(10) = new line / line feed 
-- CHAR(13) = carriage return
IF OBJECT_ID('Hero') IS NULL
    CREATE TABLE Hero (
	    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
		Alias VARCHAR(30) NOT NULL 
			CONSTRAINT CHK_AliasIsntEmpty_Hero
            CHECK (TRIM(BOTH CHAR(32) + CHAR(9) + CHAR(10) + CHAR(13) FROM Alias) <> ''),
        Debut DATE NOT NULL,
        FirstName VARCHAR(15) NOT NULL 
			CONSTRAINT CHK_FirstNameIsntEmpty_Hero
            CHECK (TRIM(BOTH CHAR(32) + CHAR(9) + CHAR(10) + CHAR(13) FROM FirstName) <> ''),
        LastName VARCHAR(15) NOT NULL 
			CONSTRAINT CHK_LastNameIsntEmpty_Hero
			CHECK (TRIM(BOTH CHAR(32) + CHAR(9) + CHAR(10) + CHAR(13) FROM LastName) <> ''),
            )
GO

IF OBJECT_ID('EventLog') IS NULL
	CREATE TABLE EventLog (
		Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
		LogMsg VARCHAR(512) NOT NULL,
    )
GO

CREATE OR ALTER VIEW HeroInformation
AS 
	SELECT Id, Alias, Debut, FirstName, LastName
	FROM Hero
GO

CREATE OR ALTER VIEW DebutAge
WITH SCHEMABINDING
AS 
	SELECT
	Id,
	Alias,
	Debut,
	(CASE WHEN YEAR(Debut) <= 1938 THEN 'Platinum Age'
		  WHEN YEAR(Debut) <= 1956 THEN 'Golden Age'
		  WHEN YEAR(Debut) <= 1970 THEN 'Silver Age'
		  WHEN YEAR(Debut) <= 1985 THEN 'Bronze Age'
		  ELSE 'Modern Age'
		  END) AS Age
	FROM dbo.Hero
GO


CREATE OR ALTER PROCEDURE LogError
AS
	BEGIN

	SET NOCOUNT ON

	DECLARE @errorNum INT = ERROR_NUMBER(),
			@errorMsg NVARCHAR(1) = ERROR_MESSAGE(),
			@errorSeverity INT = ERROR_SEVERITY(),
			@errorState INT = ERROR_STATE(),
			@errorProc NVARCHAR(1) = ERROR_PROCEDURE(),
			@errorLine INT = ERROR_LINE(),
			@errorId UNIQUEIDENTIFIER = NEWID(),
			@logMsg VARCHAR(512)

	SET @logMsg = FORMATMESSAGE('MsgId %s. ' + @errorMsg + ' Severity %d. State %d. Procedure %s. Line %d',
		CAST(@errorId AS CHAR(36)),
		@errorSeverity,
		@errorState,
		@errorProc,
		@errorLine)

	INSERT INTO EventLog (Id, LogMsg)
	VALUES (@errorId, @logMsg)

	EXECUTE xp_logevent 100000, logMsg, @errorSeverity

END
GO

CREATE OR ALTER PROCEDURE InsertHero
@Id UNIQUEIDENTIFIER = NULL,
@Alias VARCHAR(30),
@Debut DATE,
@FirstName VARCHAR(15),
@LastName VARCHAR(15)
AS
BEGIN

    DECLARE @rowsAffected int = 0,
            @trimmedAlias VARCHAR(30) = TRIM(@Alias),
            @trimmedFirstName VARCHAR(15) = TRIM(@FirstName),
            @trimmedLastName VARCHAR(15) = TRIM(@LastName)

    BEGIN TRY
        BEGIN TRANSACTION

        INSERT 
            INTO Hero (Alias, Debut, FirstName, LastName)
            VALUES (@trimmedAlias, @Debut, @trimmedFirstName, @trimmedLastName)

        IF @@TRANCOUNT > 0
            SET @rowsAffected = @@ROWCOUNT
            COMMIT
    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK

        EXECUTE LogError
    END CATCH

    RETURN @rowsAffected

END
GO

CREATE OR ALTER PROCEDURE UpdateHero
@Id UNIQUEIDENTIFIER,
@Alias VARCHAR(30),
@Debut DATE,
@FirstName VARCHAR(15),
@LastName VARCHAR(15)
AS
BEGIN

    DECLARE @rowsAffected int = 0,
            @trimmedAlias VARCHAR(30) = TRIM(@Alias),
            @trimmedFirstName VARCHAR(15) = TRIM(@FirstName),
            @trimmedLastName VARCHAR(15) = TRIM(@LastName)

    BEGIN TRY
        BEGIN TRANSACTION

        UPDATE HERO
        SET Alias = @trimmedAlias,
            Debut = @Debut,
            FirstName = @trimmedFirstName,
            LastName = @trimmedLastName
        WHERE Id = @Id

        IF @@TRANCOUNT > 0
            SET @rowsAffected = @@ROWCOUNT
            COMMIT
    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK

        EXECUTE LogError
    END CATCH

    RETURN @rowsAffected

END
GO

CREATE OR ALTER PROCEDURE DeleteHero
@Id UNIQUEIDENTIFIER
AS
BEGIN
    
    DECLARE @rowsAffected int = 0

    BEGIN TRY
        BEGIN TRANSACTION
        
        DELETE FROM Hero WHERE Id = @Id

        IF @@TRANCOUNT > 0
            SET @rowsAffected = @@ROWCOUNT
            COMMIT

    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK
        
        EXECUTE LogError
    END CATCH

    RETURN @rowsAffected

END
GO

-- Create the users that will access the databases
IF (SELECT principal_id FROM sys.sql_logins WHERE name = 'dev1Login') IS NULL
	CREATE LOGIN dev1Login
    WITH PASSWORD = '@dev1Login123',
    DEFAULT_DATABASE = CrudDaJusticaDev,
    CHECK_EXPIRATION = OFF,					-- Shouldn't expire
    CHECK_POLICY = OFF						-- Disable windows password policy
GO

CREATE OR ALTER PROCEDURE #CreateDev1User
AS
BEGIN
    IF DATABASE_PRINCIPAL_ID('dev1User') IS NULL
        CREATE USER dev1User
        FOR LOGIN dev1Login
        WITH DEFAULT_SCHEMA = dbo

    GRANT EXECUTE ON SCHEMA::dbo TO dev1User
    GRANT SELECT ON DebutAge TO dev1User
    GRANT SELECT ON EventLog TO dev1User
    GRANT SELECT ON HeroInformation TO dev1User
END
GO

USE CrudDaJustica
EXECUTE #CreateDev1User
GO

USE CrudDaJusticaDev
EXECUTE #CreateDev1User
GO