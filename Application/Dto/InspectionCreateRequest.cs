using Domain.Enums;

namespace Application.Dto;

public record InspectionCreateRequest(
        DateTime date,
        string anamnesis,
        string complaints,
        Conclusion conclusion,
        DateTime? nextVisitDate,
        DateTime? deathDate,
        Guid? previousInspectionId,
        List<DiagnosisCreateRequest> diagnoses,
        List<ConsultationCreateRequest>? consultations);