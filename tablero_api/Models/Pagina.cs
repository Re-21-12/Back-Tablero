namespace tablero_api.Models
{
    public class Pagina<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
