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
            previousInspection = previousInspection,
            createTime = DateTime.UtcNow
        };
    }

    public InspectionShortDto ToInspectionShortDto(Inspection inspection, DiagnosisDto diagnosis)
    {
        return new InspectionShortDto(
            inspection.date,
            diagnosis,
            inspection.id,
            inspection.createTime);
    }
}