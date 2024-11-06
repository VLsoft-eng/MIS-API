using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using Domain.Enums;
using FluentValidation;


namespace Application.BusinessLogic.Service;

public class InspectionService(
    IInspectionRepository inspectionRepository,
    IConsultationRepository consultationRepository,
    IDoctorMapper doctorMapper,
    IPatientMapper patientMapper,
    IConsultationMapper consultationMapper,
    ISpecialityMapper specialityMapper,
    ICommentService commentService,
    IInspectionMapper inspectionMapper,
    IValidator<InspectionEditRequest> inspectionEditValidator,
    IIcdRepository icdRepository,
    IDiagnosisService diagnosisService,
    IDoctorRepository doctorRepository,
    IDiagnosisRepository diagnosisRepository)
    : IInspectionService
{
    public async Task<InspectionDto> GetInspectionById(Guid id)
    {
        var inspection = await inspectionRepository.GetById(id);
        if (inspection == null)
        {
            throw new InspectionNotFoundException();
        }
        
        var consultations = await consultationRepository.GetByInspectionId(id);
        var baseInspection = await inspectionRepository.GetBaseInspection(id);
        var patient = inspection.patient;
        var doctor = inspection.doctor;

        var doctorDto = doctorMapper.ToDto(doctor);
        var patientDto = patientMapper.ToDto(patient);
        var diagnosisDtos = await diagnosisService.GetDiagnosesByInspectionId(inspection.id);
        var consultationDtos = new List<InspectionConsultationDto>();
        foreach (var consultation in consultations)
        {
            var rootCommentDto = await commentService.GetRootCommentByConsultation(consultation.id);
            var consultationSpecialityDto = specialityMapper.ToDto(consultation.speciality);
            var commentsCount =  await commentService.GetCommentsCountByConsultation(consultation.id);
            
            var consultationDto = consultationMapper.ToInspectionConsultationDto(
                consultation,
                consultationSpecialityDto,
                rootCommentDto,
                consultation.inspection,
                commentsCount);
            consultationDtos.Add(consultationDto);
        }
        
        return inspectionMapper.ToDto(inspection, baseInspection == null ? null : baseInspection.id, patientDto, doctorDto, diagnosisDtos,
            consultationDtos);
    }

    public async Task EditInspection(Guid inspectionId, InspectionEditRequest request, Guid doctorId)
    {
        var inspection = await inspectionRepository.GetById(inspectionId);
        if (inspection == null)
        {
            throw new InspectionNotFoundException();
        }

        if (inspection.doctor.id != doctorId)
        {
            throw new DoesntHaveRightsException();
        }

        var validation = await inspectionEditValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        if (request.conclusion == Conclusion.Death)
        {
            if (await inspectionRepository.IsHasChild(inspectionId))
            {
                throw new ValidationException("You can set Death conclusion only on terminal inspections.");
            }
        }

        await diagnosisService.DeleteDiagnosesByInspectionId(inspection.id);
        var newDiagnoses = request.diagnoses;
        foreach (var diagnosis in newDiagnoses)
        {
            var icd = await icdRepository.GetById(diagnosis.icdDiagnosisId);
            if (icd == null)
            {
                throw new IcdNotFoundException();
            }

            await diagnosisService.CreateDiagnosis(diagnosis, inspection, icd);
        }
        
        inspectionMapper.UpdateInspectionEntity(inspection, request);
        await inspectionRepository.Update(inspection);
    }

    public async Task<List<InspectionFullDto>> GetChainByRoot(Guid rootId)
    {
        var rootInspection = await inspectionRepository.GetById(rootId);
        if (rootInspection == null)
        {
            throw new InspectionNotFoundException();
        }

        if (rootInspection.previousInspection != null)
        {
            throw new InspectionNotRootException();
        }

        var chain = await inspectionRepository.GetChainByRoot(rootId);

        var inspectionFullDtos = new List<InspectionFullDto>();
        
        foreach (var inspection in chain)
        {
            bool hasNested = await inspectionRepository.IsHasChild(inspection.id);
            bool hasChain = hasNested || inspection.previousInspection != null;

            var mainDiagnosisDto = await diagnosisService.GetMainDIagnosisByInspectionId(inspection.id);
            
            var inspectionFullDto = inspectionMapper.ToInspectionFullDto(inspection, mainDiagnosisDto, hasChain, hasNested);
            inspectionFullDtos.Add(inspectionFullDto);
        }

        return inspectionFullDtos;
    }
    
    public async Task<InspectionPagedListDto> GetInspectionsWithDoctorSpeciality(
        Guid doctorId,
        bool grouped,
        List<Guid> icdRoots,
        int page,
        int size)
    {
        if (page <= 0 || size <= 0)
        {
            throw new InvalidPaginationParamsException();
        }
        
        var diagnoses = await diagnosisRepository.GetAllMainDiagnoses();
        if (icdRoots.Any())
        {
            foreach (var root in icdRoots)
            {
                var icd = await icdRepository.GetById(root);
                if (icd == null)
                {
                    throw new IcdNotFoundException();
                }
                if (icd.parent != null)
                {
                    throw new IcdNotRootException();
                }
            }
            diagnoses = await diagnosisService.FilterDiagnosisByIcdRoots(diagnoses, icdRoots);
        }
        
        var inspections = diagnoses.Select(d => d.inspection).Distinct().ToList();

        if (grouped)
        {
            var filteredInspections = new List<Inspection>();
            foreach (var inspection in inspections)
            {
                var inspectionEntity = await inspectionRepository.GetById(inspection.id);
                if (inspectionEntity.previousInspection == null)
                {
                    filteredInspections.Add(inspectionEntity);
                }
            }

            inspections = filteredInspections;
        }

        var doctor = await doctorRepository.GetById(doctorId);
        var doctorSpecialtyId = doctor!.speciality.id;
        
        var consultations = await consultationRepository.GetBySpecialityId(doctorSpecialtyId);
        var inspectionIds = consultations.Select(c => c.inspection.id).Distinct().ToList();
        
        inspections = inspections.Where(i => inspectionIds.Contains(i.id)).ToList();
        
        var pagedInspections = inspections
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
        
        List<InspectionFullDto> inspectionFullDtos = new List<InspectionFullDto>();
        foreach (var inspection in pagedInspections)
        {
            bool hasNested = await inspectionRepository.IsHasChild(inspection.id);
            bool hasChain = (hasNested || inspection.previousInspection != null);

            var mainDiagnosesDto = await diagnosisService.GetMainDIagnosisByInspectionId(inspection.id);
            var inspectionFullDto =
                inspectionMapper.ToInspectionFullDto(inspection, mainDiagnosesDto, hasChain, hasNested);
            inspectionFullDtos.Add(inspectionFullDto);
        }

        var overAllInspectionsCount = inspections.Count;
        var totalPages = (int)Math.Ceiling((double)overAllInspectionsCount / size);

        if (page > totalPages)
        {
            throw new InvalidPaginationParamsException("Page must be smaller or equal page count.");
        }
        
        var pageInfo = new PageInfoDto(size, totalPages, page);

        return new InspectionPagedListDto(inspectionFullDtos, pageInfo);
    }
}