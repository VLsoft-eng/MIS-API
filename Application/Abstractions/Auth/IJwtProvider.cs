using Domain;

namespace Application.Abstractions.Auth;

public interface IJwtProvider
{ 
    string GenerateToken(Doctor doctor);
}