

namespace tablero_api.Models.DTOS
{
    public record AnotacionDto (int id_jugador, int id_partido, int id_cuarto, int total_anotaciones);
    public record AnotacionJugadorDto(int id_partido, int id_cuarto, int total_anotaciones);
}
