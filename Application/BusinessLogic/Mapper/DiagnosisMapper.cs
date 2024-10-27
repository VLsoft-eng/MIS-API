using Application.Abstractions.Mapper;
using Application.Dto;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class DiagnosisMapper : IDiagnosisMapper
{
    public Diagnosis ToEntity(
        DiagnosisCreateRequest diagnosisCreateRequest,
        Icd icd, 
        Inspection inspection)
    {
        return new Diagnosis
        {
            id = Guid.NewGuid(),
            description = diagnosisCreateRequest.description,
            diagnosisType = diagnosisCreateRequest.type,
            icd = icd,
            inspection = inspection,
            createTime = DateTime.UtcNow
        };
    }

    public DiagnosisDto ToDto(
        Diagnosis diagnosis)
    {
        return new DiagnosisDto(
            diagnosis.icd.сode,
            diagnosis.icd.name,
            diagnosis.description,
            diagnosis.diagnosisType,
            diagnosis.id,
            diagnosis.createTime);
    }
}