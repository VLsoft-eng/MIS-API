using Domain;

namespace Application.Abstractions.Mapper;

public interface IConsultationMapper
{
    Consultation ToEntity(Speciality speciality, Inspection inspection);
}