using Application.Dto;
using Domain;

namespace Application.Abstractions.Mapper;

public interface IInspectionMapper
{
    public Inspection ToEntity(
        InspectionCreateRequest inspectionCreateRequest,
        Doctor doctor,
        Patient patient,
        Inspection previousInspection);

    public InspectionShortDto ToInspectionShortDto(Inspection inspection, DiagnosisDto diagnosis);

    InspectionFullDto ToInspectionFullDto(
        Inspection inspection,
        DiagnosisDto diagnosis,
        bool hasChain,
        bool hasNested);

    InspectionDto ToDto(
        Inspection inspection,
        Guid? baseInspectionId,
        PatientDto patient,
        DoctorDto doctor,
        List<DiagnosisDto> diagnoses,
        List<InspectionConsultationDto>? inspectionConsultation);
}