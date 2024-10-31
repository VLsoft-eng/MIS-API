using Application.Dto;

namespace Application.Abstractions.Service;

public interface IInspectionService
{
    Task<InspectionDto> GetInspectionById(Guid id);
    Task EditInspection(Guid inspectionId, InspectionEditRequest request, Guid doctorId);
    Task<List<InspectionFullDto>> GetChainByRoot(Guid rootId);

    Task<InspectionPagedListDto> GetInspectionsWithDoctorSpeciality(
        Guid doctorId,
        bool grouped,
        List<Guid> icdRoots,
        int page,
        int size);
}