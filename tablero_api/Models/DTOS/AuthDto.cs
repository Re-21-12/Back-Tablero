﻿namespace tablero_api.Models.DTOS
{
public record LoginRequestDto(string Nombre, string Contrasena);
public record LoginResponseDto(string? Token, int ExpiresIn, string? Nombre, RolDto? Rol, string RefresToken, List<PermisoDto>? Permisos);
public record RolDto(int Id_Rol, string Nombre);
public record CreateRolDto(string Nombre);
public record RegisterRequestDto(string Nombre, string Contrasena, RolDto Rol);
public record RefreshRequestDto(string RefreshToken);
public record RefreshResponseDto(string Token, string RefreshToken);
public record PermisoDto(string Nombre, int Id_Rol);
}
