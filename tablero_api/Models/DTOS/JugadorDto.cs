namespace tablero_api.Models.DTOS
{
<<<<<<< HEAD
    public record JugadorDto(string Nombre, string Apellido, int Edad, int id_Equipo);
    public record CreatedJugadorDto(string Nombre, string Apellido, int Edad, string Equipo);

=======
    public record JugadorDto(string Nombre, int Edad, int id_Equipo);
    public record CreateJugadorDto(string Nombre, int Edad, int id_Equipo);
    public record UpdateJugadorDto(string Nombre, int Edad, int id_Equipo);
>>>>>>> origin/stable
}
