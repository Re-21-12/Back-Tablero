-- Crear base de datos
CREATE DATABASE [$(MSSQL_DB)];
GO

-- Crear login para el usuario de aplicación
CREATE LOGIN [$(MSSQL_USER)] WITH PASSWORD = '$(MSSQL_PASSWORD)', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;
GO

-- Crear usuario dentro de la DB y asignarle rol
USE [$(MSSQL_DB)];
CREATE USER [$(MSSQL_USER)] FOR LOGIN [$(MSSQL_USER)];
ALTER ROLE db_owner ADD MEMBER [$(MSSQL_USER)];
GO
