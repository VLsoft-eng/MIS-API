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
}