using Application.Abstractions.Mapper;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class TokenMapper : ITokenMapper
{
    public Token toEntity(Guid id ,string token)
    {
        return new Token
        {
            id = id,
            tokenValue = token
        };
    } 
    
}