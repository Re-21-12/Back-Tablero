namespace tablero_api.Models.DTOS
{
    public record JugadorDto(string Nombre, int Edad, int id_Equipo);
    public record CreateJugadorDto(string Nombre,string Apellido, int Edad, int id_Equipo);
    public record UpdateJugadorDto(string Nombre, int Edad, int id_Equipo);

}
