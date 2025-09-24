namespace tablero_api.Models.DTOS
{
    public record CreateEquipoDto(string Nombre, int id_Localidad);
    public record UpdateEquipoDto(string Nombre, int id_Localidad);
    public record EquipoImageDto(int id_Equipo, string url);
    public record EquipoDto(int id_Equipo, string Nombre, string Localidad, int id_Localidad, string url);

}
