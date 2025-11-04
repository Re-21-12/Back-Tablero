using System.ComponentModel.Design;

namespace tablero_api.Models.DTOS
{
    public record JugadorDto(string Nombre, string Apellido, float estatura, string posicion, string Nacionalidad, int Edad, int id_Equipo);
    public record JugadorAsignarDto(int id_Jugador, int Id_Equipo);
    public record JugadorPaginaDto(int id_Jugador, string Nombre, string Apellido, float estatura, string posicion, string Nacionalidad, int Edad, int id_Equipo);

    public record CreateJugadorDto(string Nombre, string Apellido, float estatura, string posicion, string Nacionalidad, int Edad, int id_Equipo);
    public record UpdateJugadorDto(string Nombre, int Edad, int id_Equipo);

}


