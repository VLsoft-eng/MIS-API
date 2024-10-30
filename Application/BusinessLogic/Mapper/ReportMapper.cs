using Application.Abstractions.Mapper;
using Application.Dto;

namespace Application.BusinessLogic.Mapper;

public class ReportMapper : IReportMapper
{
    public IcdRootsReportDto ToDto(
        DateTime start,
        DateTime end,
        List<Guid> icdRoots, List<IcdRootsReportRecordDto> records,
        Dictionary<Guid, int> summaryByRoot)
    {
        var filters = new IcdRootsReportFiltersDto(start, end, icdRoots);
        return new IcdRootsReportDto(
            filters,
            records,
            summaryByRoot);
    }
}