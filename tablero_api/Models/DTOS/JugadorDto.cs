namespace tablero_api.Models.DTOS
{
    public record JugadorDto(string Nombre, string Apellido, int Edad, int id_Equipo);
    public record CreatedJugadorDto(string Nombre, string Apellido, int Edad, string Equipo);

}
