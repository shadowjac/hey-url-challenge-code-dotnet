using System.Threading.Tasks;

namespace hey_url_challenge_code_dotnet.Repositories
{
    public interface IUnitOfWork
    {
        IUrlRepository UrlRepository { get; }
        IHistoricalRepository HistoricalRepository { get; }
        Task SaveAsync();
    }
}
