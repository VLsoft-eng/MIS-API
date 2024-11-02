using Application.Abstractions.Mapper;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class TokenMapper : ITokenMapper
{
    public Token ToEntity(Guid id ,string token, DateTime expiresAt)
    {
        return new Token
        {
            id = id,
            tokenValue = token,
            expiresAt = expiresAt
        };
    } 
    
}