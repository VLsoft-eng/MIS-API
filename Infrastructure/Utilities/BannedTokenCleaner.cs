using Application.Abstractions.Repository;
using Quartz;

namespace Infrastructure;

public class BannedTokenCleaner : IJob
{
    private ITokenRepository _tokenRepository;
    
    public async Task Execute(IJobExecutionContext context)
    {
        await Clear();
    }
    
    public BannedTokenCleaner(
        ITokenRepository inspectionRepository
    )
    {
        _tokenRepository = inspectionRepository;
    }

    private async Task Clear()
    {
       await _tokenRepository.DeleteExpired();
    }
}