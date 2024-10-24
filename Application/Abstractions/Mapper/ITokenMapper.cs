using Domain;

namespace Application.Abstractions.Mapper;

public interface ITokenMapper
{
    Token toEntity(string token);
}