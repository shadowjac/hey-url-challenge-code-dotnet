using hey_url_challenge_code_dotnet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hey_url_challenge_code_dotnet.Services
{
    public interface IShortUrlService
    {
        Task CreateHistorical(string urlId, string browser, string os);
        Task CreateShortUrl(Url url);
        Task<IEnumerable<Url>> GetAll();
        Task<Url> GetUrlByShortUrlAsync(string shortUrl);
    }
}