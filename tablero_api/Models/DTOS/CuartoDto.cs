namespace tablero_api.Models.DTOS
{
    public record CreateCuartoDto(int No_Cuarto, int Total_Punteo, int Total_Faltas, int id_Partido, int id_Equipo);
    public record CuartoDto(int id_Cuarto, int No_Cuarto, int Total_Punteo, int Total_Faltas, int id_Partido, int id_Equipo);

}
