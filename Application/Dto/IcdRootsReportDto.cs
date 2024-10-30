namespace Application.Dto;

public record IcdRootsReportDto(
    IcdRootsReportFiltersDto filters,
    List<IcdRootsReportRecordDto> records,
    Dictionary<Guid, int> summaryByRoot);