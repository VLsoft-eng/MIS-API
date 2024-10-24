using Domain;

namespace Application.Abstractions.Repository;

public interface ITokenRepository
{ 
    Task<Token?> GetById(Guid id);
    Task Add(Token token);
}