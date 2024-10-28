using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class ConsultationService : IConsultationService
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IInspectionRepository _inspectionRepository;
    private readonly ICommentMapper _commentMapper;
    private readonly IValidator<CommentEditRequest> _commentEditValidator;

    public ConsultationService(
        IConsultationRepository consultationRepository,
        IInspectionRepository inspectionRepository,
        ICommentRepository commentRepository,
        ICommentMapper commentMapper,
        IValidator<CommentEditRequest> commentEditValidator
    )
    {
        _consultationRepository = consultationRepository;
        _inspectionRepository = inspectionRepository;
        _commentRepository = commentRepository;
        _commentMapper = commentMapper;
        _commentEditValidator = commentEditValidator;
    }

    public async Task UpdateComment(Guid commentId, CommentEditRequest request)
    {
        var comment = await _commentRepository.GetById(commentId);
        if (comment == null)
        {
            throw new CommentNotFoundException();
        }

        var validation = await _commentEditValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }
        
        _commentMapper.UpdateCommentEntity(comment, request);
    }
}