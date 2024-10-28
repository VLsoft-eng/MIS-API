using Application.Dto;

namespace Application.Abstractions.Service;

public interface IConsultationService
{

    Task UpdateComment(Guid commentId, CommentEditRequest request);
    Task<Guid> CreateComment(Guid consultationId, Guid doctorId, ConsultationCommentCreateRequest request);
    Task<ConsultationDto> GetConsultation(Guid consultationId);
    Task<InspectionPagedListDto> GetInspectionsWithDoctorSpeciality(
        Guid doctorId,
        bool grouped,
        List<Guid> icdRoots,
        int page,
        int size);
}