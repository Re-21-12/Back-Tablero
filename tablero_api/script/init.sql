-- Usar la base de datos master como contexto inicial
USE master;
GO

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Tablero_DB')
BEGIN
    CREATE DATABASE Tablero_DB;
    PRINT 'Base de datos creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La base de datos ya existe.';
END
GO

-- Crear login a nivel servidor
IF NOT EXISTS (SELECT name FROM sys.sql_logins WHERE name = 'appuser')
BEGIN
    CREATE LOGIN [appuser] WITH PASSWORD = 'AppUserPass!2025', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;
END
GO

-- Crear usuario dentro de la DB y asignar rol
USE [Tablero_DB];
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'appuser')
BEGIN
    CREATE USER [appuser] FOR LOGIN [appuser];
    ALTER ROLE db_owner ADD MEMBER [appuser];
END
GO
