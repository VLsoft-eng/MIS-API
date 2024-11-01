using Application.Abstractions.Mapper;
using Application.Dto;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class InspectionMapper : IInspectionMapper
{
    public Inspection ToEntity(
        InspectionCreateRequest inspectionCreateRequest, 
        Doctor doctor,
        Patient patient,
        Inspection previousInspection)
    {
        return new Inspection
        {
            id = Guid.NewGuid(),
            date = inspectionCreateRequest.date,
            anamnesis = inspectionCreateRequest.anamnesis,
            complaints = inspectionCreateRequest.complaints,
            treatment = inspectionCreateRequest.treatment,
            conclusion = inspectionCreateRequest.conclusion,
            nextVisitDate = inspectionCreateRequest.nextVisitDate,
            deathDate = inspectionCreateRequest.deathDate,
            doctor = doctor,
            patient = patient,
            isNotified = false,
            previousInspection = previousInspection,
            createTime = DateTime.UtcNow
        };
    }

    public InspectionShortDto ToInspectionShortDto(
        Inspection inspection, 
        DiagnosisDto diagnosis)
    {
        return new InspectionShortDto(
            inspection.date,
            diagnosis,
            inspection.id,
            inspection.createTime);
    }

    public InspectionFullDto ToInspectionFullDto(
        Inspection inspection, 
        DiagnosisDto diagnosis,
        bool hasChain,
        bool hasNested)
    {
        return new InspectionFullDto(
            inspection.id,
            inspection.createTime,
            inspection.previousInspection == null ? null : inspection.previousInspection.id,
            inspection.date,
            inspection.conclusion,
            inspection.doctor.id,
            inspection.patient.id,
            diagnosis,
            hasChain,
            hasNested
        );
    }

    public InspectionDto ToDto(
        Inspection inspection,
        Guid? baseInspectionId,
        PatientDto patient,
        DoctorDto doctor,
        List<DiagnosisDto> diagnoses,
        List<InspectionConsultationDto>? inspectionConsultation)
    {
        return new InspectionDto(
            inspection.id,
            inspection.createTime,
            inspection.date,
            inspection.anamnesis,
            inspection.complaints,
            inspection.treatment,
            inspection.conclusion,
            inspection.nextVisitDate,
            inspection.deathDate,
            baseInspectionId,
            inspection.previousInspection == null ? null : inspection.previousInspection.id,
            patient,
            doctor,
            diagnoses,
            inspectionConsultation
        );
    }

    public void UpdateInspectionEntity(Inspection inspection, InspectionEditRequest request)
    {
        inspection.anamnesis = request.anamnesis;
        inspection.complaints = request.complaints;
        inspection.treatment = request.treatment;
        inspection.conclusion = request.conclusion;
        inspection.nextVisitDate = request.nextVisitDate;
        inspection.deathDate = request.deathDate;
    }
}