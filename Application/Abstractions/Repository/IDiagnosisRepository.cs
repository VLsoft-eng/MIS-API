using Domain;

namespace Application.Abstractions.Repository;

public interface IDiagnosisRepository
{
    Task Create(Diagnosis diagnosis);
}