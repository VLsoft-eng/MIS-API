using Domain.Enums;

namespace Application.Dto;

public record InspectionDto(
    Guid id,
    DateTime createTime,
    DateTime date,
    string? anamnesis,
    string? complaints,
    string? treatment,
    Conclusion? conclusion,
    DateTime? nextVisitDate,
    DateTime? deathDate,
    Guid? baseInspectionId,
    Guid? previousInspectionId,
    PatientDto patient,
    DoctorDto doctor,
    List<DiagnosisDto> diagnoses,
    List<InspectionConsultationDto>? consultations);