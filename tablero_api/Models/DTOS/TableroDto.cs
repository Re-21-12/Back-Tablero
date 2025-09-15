namespace tablero_api.Models.DTOS
{
public record GetResultadoPartido(int id);
    public record ResultadoPartidoDto(string nombreLocal, string nombreVisitante, string localidad, int totalFaltasLocal, int totalFaltasVisitante, int totalPunteoLocal, int totalPunteoVisitante);
}
