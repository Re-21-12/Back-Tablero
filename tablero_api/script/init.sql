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

-- Crear tablas y relaciones
IF OBJECT_ID('Cuartos', 'U') IS NOT NULL DROP TABLE Cuartos;
IF OBJECT_ID('Partidos', 'U') IS NOT NULL DROP TABLE Partidos;
IF OBJECT_ID('Equipos', 'U') IS NOT NULL DROP TABLE Equipos;
IF OBJECT_ID('Localidades', 'U') IS NOT NULL DROP TABLE Localidades;
IF OBJECT_ID('Imagenes', 'U') IS NOT NULL DROP TABLE Imagenes;
GO

CREATE TABLE Localidades (
    id_Localidad INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(MAX) NOT NULL
);

CREATE TABLE Equipos (
    id_Equipo INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(MAX) NOT NULL,
    id_Localidad INT NOT NULL,
    CONSTRAINT FK_Equipos_Localidades_id_Localidad FOREIGN KEY (id_Localidad)
        REFERENCES Localidades(id_Localidad) ON DELETE NO ACTION
);

CREATE TABLE Imagenes (
    id_Imagen INT IDENTITY(1,1) PRIMARY KEY,
    url NVARCHAR(MAX) NOT NULL
);

CREATE TABLE Partidos (
    id_Partido INT IDENTITY(1,1) PRIMARY KEY,
    FechaHora DATETIME2 NOT NULL,
    id_Localidad INT NOT NULL,
    id_Local INT NOT NULL,
    id_Visitante INT NOT NULL,
    CONSTRAINT FK_Partidos_Localidades_id_Localidad FOREIGN KEY (id_Localidad)
        REFERENCES Localidades(id_Localidad) ON DELETE NO ACTION,
    CONSTRAINT FK_Partidos_Equipos_id_Local FOREIGN KEY (id_Local)
        REFERENCES Equipos(id_Equipo) ON DELETE NO ACTION,
    CONSTRAINT FK_Partidos_Equipos_id_Visitante FOREIGN KEY (id_Visitante)
        REFERENCES Equipos(id_Equipo) ON DELETE NO ACTION
);

CREATE TABLE Cuartos (
    id_Cuarto INT IDENTITY(1,1) PRIMARY KEY,
    No_Cuarto INT NOT NULL,
    duenio NVARCHAR(MAX) NULL,
    Total_Punteo INT NOT NULL,
    Total_Faltas INT NOT NULL,
    id_Partido INT NOT NULL,
    id_Equipo INT NOT NULL,
    CONSTRAINT FK_Cuartos_Partidos_id_Partido FOREIGN KEY (id_Partido)
        REFERENCES Partidos(id_Partido) ON DELETE NO ACTION,
    CONSTRAINT FK_Cuartos_Equipos_id_Equipo FOREIGN KEY (id_Equipo)
        REFERENCES Equipos(id_Equipo) ON DELETE NO ACTION
);

-- Índices para claves foráneas
CREATE INDEX IX_Equipos_id_Localidad ON Equipos(id_Localidad);
CREATE INDEX IX_Partidos_id_Localidad ON Partidos(id_Localidad);
CREATE INDEX IX_Partidos_id_Local ON Partidos(id_Local);
CREATE INDEX IX_Partidos_id_Visitante ON Partidos(id_Visitante);
CREATE INDEX IX_Cuartos_id_Partido ON Cuartos(id_Partido);
CREATE INDEX IX_Cuartos_id_Equipo ON Cuartos(id_Equipo);
GO
