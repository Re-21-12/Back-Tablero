namespace tablero_api.Models.DTOS
{
public record UsuarioDto(
    string username,
    string email,
    string password,
    string firstName,
    string lastName,
    bool enabled
);
}
