using Application.Dto;
using Domain;

namespace Application.Abstractions.Mapper;

public interface IDiagnosisMapper
{
    Diagnosis ToEntity(
        DiagnosisCreateRequest diagnosisCreateRequest,
        Icd icd,
        Inspection inspection);
}