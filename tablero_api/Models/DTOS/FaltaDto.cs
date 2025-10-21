namespace tablero_api.Models.DTOS
{
    public record FaltaDto(int id_Jugador, int Id_Partido, int Id_Cuarto, int total_faltas);
    public record FaltaJugadorDto(int id_Partido, int id_Cuarto, int total_faltas);
}
