namespace Application.Dto;

public record IcdRootsReportDto(
    IcdRootsReportFiltersDto filters,
    List<IcdRootsReportRecordDto> records,
    Dictionary<string, int> summaryByRoot);