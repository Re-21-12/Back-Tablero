-- Crear base de datos
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MiBase')
BEGIN
    CREATE DATABASE [MiBase];
END
GO

-- Crear login a nivel servidor
IF NOT EXISTS (SELECT name FROM sys.sql_logins WHERE name = 'appuser')
BEGIN
    CREATE LOGIN [appuser] WITH PASSWORD = 'AppUserPass!2025', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;
END
GO

-- Crear usuario dentro de la DB y asignar rol
USE [MiBase];
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'appuser')
BEGIN
    CREATE USER [appuser] FOR LOGIN [appuser];
    ALTER ROLE db_owner ADD MEMBER [appuser];
END
GO
