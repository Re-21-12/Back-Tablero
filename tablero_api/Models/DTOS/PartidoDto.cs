namespace tablero_api.Models.DTOS
{
    public record PartidoDto(int id_Partido, DateTime FechaHora, int id_Localidad, int id_Local, int id_Visitante);
    public record CreatePartidoDto(DateTime FechaHora, int id_Localidad, int id_Local, int id_Visitante);
}