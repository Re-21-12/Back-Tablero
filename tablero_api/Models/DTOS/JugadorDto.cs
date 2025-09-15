namespace tablero_api.Models.DTOS
{
    public record JugadorDto(int id_Jugador, string Nombre, int Edad, int id_Equipo);
    public record CreateJugadorDto(string Nombre, int Edad, int id_Equipo);
    public record UpdateJugadorDto(string Nombre, int Edad, int id_Equipo);
}
