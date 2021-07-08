using HeyUrlChallengeCodeDotnet.Data;
using System.Threading.Tasks;

namespace hey_url_challenge_code_dotnet.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUrlRepository UrlRepository { get; private set; }
        public IHistoricalRepository HistoricalRepository { get; private set; }
        private readonly ApplicationContext _context;

        public UnitOfWork(IUrlRepository urlRepository, IHistoricalRepository historicalRepository, ApplicationContext context)
        {
            UrlRepository = urlRepository;
            HistoricalRepository = historicalRepository;
            _context = context;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
