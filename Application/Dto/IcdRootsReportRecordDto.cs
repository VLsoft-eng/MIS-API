using Domain.Enums;

namespace Application.Dto;

public record IcdRootsReportRecordDto (
    string patientName,
    DateTime patientBirtday,
    Gender Gender,
    int visitsByRoot);