using Videotheque.ReportingService.Api.Models.DTO;

namespace Videotheque.ReportingService.Api.Application.Services.Interfaces
{
    public interface IReportingService
    {
        Task<ReportDTO> GetReportAsync(DateTime from, DateTime to); 
    }
}