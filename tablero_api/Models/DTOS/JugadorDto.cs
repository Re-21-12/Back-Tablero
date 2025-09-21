namespace tablero_api.Models.DTOS
{
    public record JugadorDto(string Nombre, string Apellido, float estatura, string posicion, string Nacionalidad , int Edad, int id_Equipo);
    public record JugadorPaginaDto(string Nombre, string Apellido,  float estatura, string posicion, string Nacionalidad, int Edad, string nombre_equipo);

    public record CreateJugadorDto(string Nombre,string Apellido, float estatura, string posicion, string Nacionalidad, int Edad, int id_Equipo);
    public record UpdateJugadorDto(string Nombre, int Edad, int id_Equipo);

}
