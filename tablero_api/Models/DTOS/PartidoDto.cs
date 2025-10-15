namespace tablero_api.Models.DTOS
{
    public record GetPartidoDto(int id);
    public record ResponsePartidoDto( DateTime FechaHora, string localidad, string local, string visitante);
    public record ReportePartidoDto(DateTime FechaHora, int localidad, int local, int visitante);
    public record PartidoDto( DateTime FechaHora, int id_Localidad, int id_Local, int id_Visitante, string? local, string? visitante);
    public record CreatePartidoDto(DateTime FechaHora,int id_Local, int id_Visitante);
    public record UpdatePartidoDto(DateTime FechaHora, int id_Local, int id_visitante);
    public record PartidoResultadoDto(int id_Partido, string local, string visitante, ResultadoDto Resultado, DateTime fecha);
    public record ResultadoDto(int id_Partido, int puntaje_local, int puntaje_visitante);
}