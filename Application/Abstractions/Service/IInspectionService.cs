using Application.Dto;

namespace Application.Abstractions.Service;

public interface IInspectionService
{
    Task<InspectionDto> GetInspectionById(Guid id);
    Task EditInspection(Guid inspectionId, InspectionEditRequest request, Guid doctorId);
}