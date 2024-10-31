using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class DiagnosisService : IDiagnosisService
{
    private readonly IDiagnosisRepository _diagnosisRepository;
    private readonly IDiagnosisMapper _diagnosisMapper;
    private readonly IValidator<DiagnosisCreateRequest> _diagnosisValidator;
    private readonly IIcdRepository _icdRepository;

    public DiagnosisService(
        IDiagnosisRepository diagnosisRepository,
        IDiagnosisMapper diagnosisMapper,
        IValidator<DiagnosisCreateRequest> diagnosisValidator,
        IIcdRepository icdRepository)
    {
        _diagnosisRepository = diagnosisRepository;
        _diagnosisMapper = diagnosisMapper;
        _diagnosisValidator = diagnosisValidator;
        _icdRepository = icdRepository;
    }

    public async Task CreateDiagnosis(DiagnosisCreateRequest request, Inspection inspection, Icd icd)
    {
        var diagnosisValidation = await _diagnosisValidator.ValidateAsync(request);
        if (!diagnosisValidation.IsValid)
        {
            throw new ValidationException(diagnosisValidation.Errors[0].ErrorMessage);
        }
        
        var diagnosis = _diagnosisMapper.ToEntity(request, icd, inspection);
        await _diagnosisRepository.Create(diagnosis);
    }

    public async Task<DiagnosisDto> GetMainDIagnosisByInspectionId(Guid inspectionId)
    {
        var diagnosis = await _diagnosisRepository.GetMainDiagnosesByInspectionId(inspectionId);
        return _diagnosisMapper.ToDto(diagnosis);
    }

    public async Task DeleteDiagnosesByInspectionId(Guid inspectionId)
    {
        await _diagnosisRepository.DeleteAllDiagnosesByInspection(inspectionId);
    }

    public async Task<List<DiagnosisDto>> GetDiagnosesByInspectionId(Guid inspectionId)
    {
        var diagnoses = await _diagnosisRepository.GetDiagnosesByInspectionId(inspectionId);
        return diagnoses.Select(d => _diagnosisMapper.ToDto(d)).ToList();
    }
    
    public async Task<List<Diagnosis>> FilterDiagnosisByIcdRoots(List<Diagnosis> diagnoses, List<Guid> icdRoots)
    {
        var filteredDiagnoses = new List<Diagnosis>();
        foreach (var diagnosis in diagnoses)
        {
            bool hasRoot = false;

            foreach (var root in icdRoots)
            {
                var diagnosisIcdRoot = await _icdRepository.GetRootByIcdId(diagnosis.icd.id);
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