using Application.Dto;

namespace Application.Abstractions.Mapper;

public interface IReportMapper
{
    IcdRootsReportDto ToDto(
        DateTime start,
        DateTime end,
        List<Guid> icdRoots, List<IcdRootsReportRecordDto> records,
        Dictionary<Guid, int> summaryByRoot);
}