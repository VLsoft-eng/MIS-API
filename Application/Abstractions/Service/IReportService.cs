using Application.Dto;

namespace Application.Abstractions.Service;

public interface IReportService
{
    Task<IcdRootsReportDto> GetReport(DateTime start, DateTime end, List<Guid> icdRoots);
}