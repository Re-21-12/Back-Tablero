namespace tablero_api.Models.DTOS
{
    public record CreateEquipoDto(string Nombre, int id_Localidad);
    public record UpdateEquipoDto(string Nombre, int id_Localidad);
    public record EquipoDto( string Nombre, string Localidad);

}
