using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class DiagnosisService(
    IDiagnosisRepository diagnosisRepository,
    IDiagnosisMapper diagnosisMapper,
    IValidator<DiagnosisCreateRequest> diagnosisValidator,
    IIcdRepository icdRepository)
    : IDiagnosisService
{
    public async Task CreateDiagnosis(DiagnosisCreateRequest request, Inspection inspection, Icd icd)
    {
        var diagnosisValidation = await diagnosisValidator.ValidateAsync(request);
        if (!diagnosisValidation.IsValid)
        {
            throw new ValidationException(diagnosisValidation.Errors[0].ErrorMessage);
        }
        
        var diagnosis = diagnosisMapper.ToEntity(request, icd, inspection);
        await diagnosisRepository.Create(diagnosis);
    }

    public async Task<DiagnosisDto> GetMainDIagnosisByInspectionId(Guid inspectionId)
    {
        var diagnosis = await diagnosisRepository.GetMainDiagnosesByInspectionId(inspectionId);
        return diagnosisMapper.ToDto(diagnosis);
    }

    public async Task DeleteDiagnosesByInspectionId(Guid inspectionId)
    {
        await diagnosisRepository.DeleteAllDiagnosesByInspection(inspectionId);
    }

    public async Task<List<DiagnosisDto>> GetDiagnosesByInspectionId(Guid inspectionId)
    {
        var diagnoses = await diagnosisRepository.GetDiagnosesByInspectionId(inspectionId);
        return diagnoses!.Select(d => diagnosisMapper.ToDto(d)).ToList();
    }
    
    public async Task<List<Diagnosis>> FilterDiagnosisByIcdRoots(List<Diagnosis> diagnoses, List<Guid> icdRoots)
    {
        var filteredDiagnoses = new List<Diagnosis>();
        foreach (var diagnosis in diagnoses)
        {
            bool hasRoot = false;

            foreach (var root in icdRoots)
            {
                var diagnosisIcdRoot = await icdRepository.GetRootByIcdId(diagnosis.icd.id);
                if (diagnosisIcdRoot.id == root)
                {
                    hasRoot = true;
                    break; 
                }
            }

            if (hasRoot)
            {
                filteredDiagnoses.Add(diagnosis);
            }
        }

        return filteredDiagnoses;
    }
}