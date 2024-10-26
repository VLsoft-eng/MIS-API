using Domain;

namespace Application.Abstractions.Mapper;

public interface ITokenMapper
{
    Token ToEntity(Guid id, string token);
}