namespace tablero_api.Models.DTOS
{
    public record CreateImagenDto(string url);
    public record ImagenDto(int id_Imagen, string url);

}
