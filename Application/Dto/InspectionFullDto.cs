
using Domain.Enums;

namespace Application.Dto;

public record InspectionFullDto(
    Guid id,
    DateTime createTime,
    Guid? previousId,
    DateTime date,
    Conclusion conclusion,
    Guid doctorId,
    Guid patientId,
    DiagnosisDto diagnosis,
    bool hasChain,
    bool hasNested);