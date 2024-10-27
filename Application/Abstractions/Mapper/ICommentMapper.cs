using Application.Dto;
using Domain;

namespace Application.Abstractions.Mapper;

public interface ICommentMapper
{
    Comment ToEntity(
        InspectionCommentCreateRequest request,
        Doctor doctor,
        Consultation consultation,
        Comment parent);
}