using hey_url_challenge_code_dotnet.Models;
using hey_url_challenge_code_dotnet.Services;
using hey_url_challenge_code_dotnet.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shyjus.BrowserDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HeyUrlChallengeCodeDotnet.Controllers
{
    [Route("/")]
    public class UrlsController : Controller
    {
        private readonly ILogger<UrlsController> _logger;
        private static readonly Random getrandom = new Random();
        private readonly IBrowserDetector browserDetector;
        private readonly IShortUrlService shortUrlService;

        public UrlsController(ILogger<UrlsController> logger, IBrowserDetector browserDetector, IShortUrlService shortUrlService)
        {
            this.browserDetector = browserDetector;
            this.shortUrlService = shortUrlService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var urls = await shortUrlService.GetAll();
            return View(new HomeViewModel { Urls = urls });
        }

        [HttpPost]
        public async Task<IActionResult> Create(HomeViewModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.NewUrl.OriginalUrl))
            {
                throw new ArgumentException($"'{nameof(model.NewUrl.OriginalUrl)}' cannot be null or empty.", nameof(model.NewUrl.OriginalUrl));
            }
            var isValidUrl = Regex.IsMatch(model.NewUrl.OriginalUrl, @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
            if(!isValidUrl)
            {
                TempData["Notice"] = "Invalid URL";
                return RedirectToAction("Index");
            }
            await this.shortUrlService.CreateShortUrl(model.NewUrl);
            return RedirectToAction("Index");
        }

        [Route("/{url}")]
        public async Task<IActionResult> VisitAsync(string url)
        {
            try
            {
                await this.shortUrlService.CreateHistorical(url, browserDetector.Browser.Name, browserDetector.Browser.OS);
                return new OkObjectResult($"{url}, {this.browserDetector.Browser.OS}, {this.browserDetector.Browser.Name}");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Route("urls/{url}")]
        public async Task<IActionResult> ShowAsync(string url)
        {
            var urlInfo = await shortUrlService.GetUrlByShortUrlAsync(url);
            var browserClicks = urlInfo.Historical?
                .GroupBy(p => p.BrowserName)
                .Select(p => new { Browser = p.Key, Value = p.Count() })
                .ToDictionary(p => p.Browser, q => q.Value);
            var platformClicks = urlInfo.Historical?
                .GroupBy(p => p.OS)
                .Select(p => new { OS = p.Key, Value = p.Count() })
                .ToDictionary(p => p.OS, q => q.Value);

            var dailyClicks = urlInfo.Historical?
                .Where(p => p.CreatedAt.Month == DateTime.UtcNow.Month)
                .GroupBy(p => p.CreatedAt.Day)
                .Select(p => new { Day = p.Key, Value = p.Count() })
                .OrderBy(p => p.Day)
                .ToDictionary(p => p.Day.ToString(), q => q.Value);

            return View(new ShowViewModel
            {
                Url = urlInfo,
                DailyClicks = dailyClicks,
                BrowseClicks = browserClicks,
                PlatformClicks = platformClicks
            });
        }
    }
}