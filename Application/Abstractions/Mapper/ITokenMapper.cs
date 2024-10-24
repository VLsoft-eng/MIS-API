using Domain;

namespace Application.Abstractions.Mapper;

public interface ITokenMapper
{
    Token toEntity(Guid id, string token);
}