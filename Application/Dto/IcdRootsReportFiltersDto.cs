namespace Application.Dto;

public record IcdRootsReportFiltersDto(
    DateTime start,
    DateTime end,
    List<Guid>? icdRoots);