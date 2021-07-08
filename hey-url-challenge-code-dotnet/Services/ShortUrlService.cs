using hey_url_challenge_code_dotnet.Models;
using hey_url_challenge_code_dotnet.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hey_url_challenge_code_dotnet.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly IUnitOfWork _unitOfWork;
        private static Random random = new Random();

        public ShortUrlService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateShortUrl(Url url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            url.ShortUrl = GenerateShortUrlId();
            url.CreatedAt = DateTime.UtcNow;
            
            await _unitOfWork.UrlRepository.AddAsync(url);
            await _unitOfWork.SaveAsync();
        }

        public async Task CreateHistorical(string urlId, string browser, string os)
        {
            var url = await GetUrlByShortUrlAsync(urlId);
            if (url is null) throw new Exception($"URL with id {urlId} doesnot exist");
            var historical = new Historical
            {
                BrowserName = browser,
                OS = os,
                CreatedAt = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Url = url
            };

            url.Count++;
            await _unitOfWork.HistoricalRepository.AddAsync(historical);
            await _unitOfWork.UrlRepository.UpdateAsync(url);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Url> GetUrlByShortUrlAsync(string shortUrl)
        {
            var url = await _unitOfWork.UrlRepository.GetSingleOrDefaultAsync(p => p.ShortUrl == shortUrl);
            if (url is null) return null;
            return url;
        }

        public async Task<IEnumerable<Url>> GetAll()
        {
            return await _unitOfWork.UrlRepository.GetAllAsync();
        }

        private static string GenerateShortUrlId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, 5)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
