using Application.Abstractions.Repository;
using Quartz;

namespace Infrastructure;

public class BannedTokenCleaner(ITokenRepository inspectionRepository) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await Clear();
    }

    private async Task Clear()
    {
       await inspectionRepository.DeleteExpired();
    }
}