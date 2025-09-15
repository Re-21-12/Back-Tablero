namespace tablero_api.Models.DTOS
{
public record LoginRequestDto(string Nombre, string Contrasena);
public record LoginResponseDto(string? Token, string? Nombre, RolDto? Rol, string RefresToken);
public record RolDto(string Nombre);
public record RegisterRequestDto(string Nombre, string Contrasena, RolDto Rol);
    public record RefreshRequestDto(string RefreshToken);
    public record RefreshResponseDto(string Token, string RefreshToken);
    public record PermisoDto(string Nombre );

}
