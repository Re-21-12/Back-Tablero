using System.Threading.Tasks;
using tablero_api.DTOS;

namespace tablero_api.Services.Interfaces
{
    public interface IImportService
    {
        Task<ImportResponse> ImportEquiposAsync(byte[] fileBytes, bool isCsv, string? fileName = null);
        Task<ImportResponse> ImportJugadoresAsync(byte[] fileBytes, bool isCsv, string? fileName = null);
        Task<ImportResponse> ImportLocalidadesAsync(byte[] fileBytes, bool isCsv, string? fileName = null);
        Task<ImportResponse> ImportPartidosAsync(byte[] fileBytes, bool isCsv, string? fileName = null);
    }
}
