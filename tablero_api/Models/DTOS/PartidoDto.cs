namespace tablero_api.Models.DTOS
{
    public record GetPartidoDto(int id);
    public record ResponsePartidoDto( DateTime FechaHora, string localidad, string local, string visitante);
    public record PartidoDto( DateTime FechaHora, int id_Localidad, int id_Local, int id_Visitante, string local, string visitante);

    public record CreatePartidoDto(DateTime FechaHora,int id_Local, int id_Visitante);
    public record UpdatePartidoDto(DateTime FechaHora, int id_Local, int id_visitante);
}