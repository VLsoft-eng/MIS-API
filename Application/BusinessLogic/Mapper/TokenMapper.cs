using Application.Abstractions.Mapper;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class TokenMapper : ITokenMapper
{
    public Token toEntity(string token)
    {
        return new Token
        {
            id = Guid.NewGuid(),
            tokenValue = token
        };
    } 
    
}