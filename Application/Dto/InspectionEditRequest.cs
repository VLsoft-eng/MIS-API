using Domain.Enums;

namespace Application.Dto;

public record InspectionEditRequest(
    string anamnesis,
    string complaints,
    string treatment,
    Conclusion conclusion,
    DateTime nextVisitDate,
    DateTime deathDate,
    List<DiagnosisCreateRequest> diagnoses);