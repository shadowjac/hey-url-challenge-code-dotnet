using hey_url_challenge_code_dotnet.Models;
using HeyUrlChallengeCodeDotnet.Data;

namespace hey_url_challenge_code_dotnet.Repositories
{
    public class HistoricalRepository : BaseRepository<Historical>, IHistoricalRepository
    {
        public HistoricalRepository(ApplicationContext context) : base(context)
        {
        }
    }
}