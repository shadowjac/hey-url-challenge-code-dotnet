using hey_url_challenge_code_dotnet.Models;
using HeyUrlChallengeCodeDotnet.Data;

namespace hey_url_challenge_code_dotnet.Repositories
{
    public class UrlRepository : BaseRepository<Url>, IUrlRepository
    {
        public UrlRepository(ApplicationContext context) : base(context)
        {
        }
    }
}