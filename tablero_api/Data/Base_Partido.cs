using System;
using System.Collections.Generic;

namespace tablero_api.Data
{
    public class Partido
    {
        public int id_Partido { get; set; } // PK
        public DateTime FechaHora { get; set; }

        // Relaciones
        public int id_Localidad { get; set; }
        public Localidad Localidad { get; set; } = null!;

        public int id_Local { get; set; }
        public Equipo local { get; set; }
        public int id_Visitante { get; set; }
        public Equipo Visitante { get; set; }
    }

    public class Equipo
    {
        public int id_Equipo { get; set; }
        public string Nombre { get; set; }
        public int id_Localidad {get;set;}
        public Localidad Localidad { get; set; }
    }

    public class Cuarto
    {
        public int id_Cuarto { get; set; }
        public int No_Cuarto { get; set; }
        public int Total_Punteo { get; set; }
        public int Total_Faltas { get; set; }


        // FK a Partido
        public int id_Partido { get; set; }
        public Partido Partido { get; set; } = null!;
        
        public int id_Equipo { get; set; }
        public Equipo Equipo { get; set; }
    }

    public class Localidad
    {
        public int id_Localidad { get; set; }
        public string Nombre { get; set; } = null!;

        
    }
    public class imagen
    {
        public int id_Imagen { get; set; }
        public string url { get; set; }
    }

}