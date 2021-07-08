using hey_url_challenge_code_dotnet.Models;
using hey_url_challenge_code_dotnet.Services;
using hey_url_challenge_code_dotnet.ViewModels;
using HeyUrlChallengeCodeDotnet.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shyjus.BrowserDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tests
{
    public class UrlsControllerTest
    {
        private Mock<ILogger<UrlsController>> logMock;
        private Mock<IBrowserDetector> browserMock;
        private Mock<IShortUrlService> serviceMock;

        [SetUp]
        public void Setup()
        {
            logMock = new Mock<ILogger<UrlsController>>();
            browserMock = new Mock<IBrowserDetector>();
            serviceMock = new Mock<IShortUrlService>();
        }

        [Test]
        public async Task Index_WhenRequestView_ItsModelShouldBeAListOfUrls()
        {
            serviceMock.Setup(m => m.GetAll())
                .ReturnsAsync(new List<Url>
                {
                    new Url{OriginalUrl = "http://www.aa.com", ShortUrl = "ABCDE"}
                });

            var controller = new UrlsController(logMock.Object, browserMock.Object, serviceMock.Object);
            var result = await controller.Index() as ViewResult;
            var model = result.Model as HomeViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Urls.Count());
            Assert.AreEqual("http://www.aa.com", model.Urls.First().OriginalUrl);
            Assert.AreEqual("ABCDE", model.Urls.First().ShortUrl);
        }

        [Test]
        public async Task Create_WhenModelIsNull_ShouldThrowAnArgumentNullException()
        {
            var controller = new UrlsController(logMock.Object, browserMock.Object, serviceMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var result = await controller.Create(null);
            });
        }

        [Test]
        public async Task Create_WhenOriginalUrlIsEmptyOrNull_ShouldThrowAnArgumentException()
        {
            var controller = new UrlsController(logMock.Object, browserMock.Object, serviceMock.Object);
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                var result = await controller.Create(new HomeViewModel { NewUrl = new Url { } });
            });
        }

        [Test]
        public async Task Create_WhenOriginalUrlIsInvalid_ShouldReturnIndexActionAndErrorInTempData()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var controller = new UrlsController(logMock.Object, browserMock.Object, serviceMock.Object)
            {
                TempData = tempData
            };
            var result = await controller.Create(new HomeViewModel { NewUrl = new Url { OriginalUrl = "abc" } }) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid URL", controller.TempData["Notice"]);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async Task Create_WhenOriginalUrlIsValid_ShouldReturnIndexActionAndCreateUrl()
        {
            serviceMock.Setup(m => m.CreateShortUrl(It.IsAny<Url>()))
                .Verifiable();

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var controller = new UrlsController(logMock.Object, browserMock.Object, serviceMock.Object)
            {
                TempData = tempData
            };
            var result = await controller.Create(new HomeViewModel { NewUrl = new Url { OriginalUrl = "http://www.aa.com" } }) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(null, controller.TempData["Notice"]);
            Assert.AreEqual("Index", result.ActionName);
            serviceMock.Verify(x => x.CreateShortUrl(It.IsAny<Url>()), Times.Once);
        }

        [Test]
        public async Task VisitAsync_WhenCreateHistoricalFails_ShouldreturnNotFound()
        {
            browserMock.SetupGet(p => p.Browser.Name).Returns("Firefox");
            browserMock.SetupGet(p => p.Browser.OS).Returns("Windows");
            serviceMock.Setup(m => m.CreateHistorical(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Test error"));
            var controller = new UrlsController(logMock.Object, browserMock.Object, serviceMock.Object);

            var result = await controller.VisitAsync(It.IsAny<string>()) as NotFoundObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Test error", result.Value);
        }

        [Test]
        public async Task VisitAsync_WhenCreateHistoricalWorks_ShouldReturnOk()
        {
            browserMock.SetupGet(p => p.Browser.Name).Returns("Firefox");
            browserMock.SetupGet(p => p.Browser.OS).Returns("Windows");
            serviceMock.Setup(m => m.CreateHistorical(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            var controller = new UrlsController(logMock.Object, browserMock.Object, serviceMock.Object);

            var result = await controller.VisitAsync("www.aa.com") as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("www.aa.com, Windows, Firefox", result.Value);
        }

        [Test]
        public async Task ShowAsync_WhenGetHistorical_ShouldReturnReportInformation()
        {
            serviceMock.Setup(m => m.GetUrlByShortUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new Url
                {
                    Historical = new List<Historical>
                    {
                        new Historical
                        {
                            BrowserName = "Firefox",
                            CreatedAt = new DateTime(2021,DateTime.UtcNow.Month,1),
                            OS = "MacOS",
                        },
                        new Historical
                        {
                            BrowserName = "Chrome",
                            CreatedAt = new DateTime(2021,DateTime.UtcNow.Month,3),
                            OS = "Windows",
                        },
                        new Historical
                        {
                            BrowserName = "Firefox",
                            CreatedAt = new DateTime(2021,DateTime.UtcNow.Month,1),
                            OS = "Linux",
                        },
                        new Historical
                        {
                            BrowserName = "Edge",
                            CreatedAt = new DateTime(2021,DateTime.UtcNow.Month,1),
                            OS = "Windows",
                        },
                        new Historical
                        {
                            BrowserName = "Firefox",
                            CreatedAt = new DateTime(2021,DateTime.UtcNow.Month,1),
                            OS = "MacOS",
                        },
                    }
                });
            var controller = new UrlsController(logMock.Object, browserMock.Object, serviceMock.Object);

            var result = await controller.ShowAsync(It.IsAny<string>()) as ViewResult;
            Assert.IsNotNull(result);
            var model = result.Model as ShowViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.BrowseClicks.Count);
            Assert.AreEqual(3, model.PlatformClicks.Count);
            Assert.AreEqual(2, model.DailyClicks.Count);

            Assert.AreEqual(3, model.BrowseClicks["Firefox"]);
            Assert.AreEqual(1, model.BrowseClicks["Edge"]);
            Assert.AreEqual(1, model.BrowseClicks["Chrome"]);

            Assert.AreEqual(1, model.PlatformClicks["Linux"]);
            Assert.AreEqual(2, model.PlatformClicks["Windows"]);
            Assert.AreEqual(2, model.PlatformClicks["MacOS"]);

            Assert.AreEqual(4, model.DailyClicks["1"]);
            Assert.AreEqual(1, model.DailyClicks["3"]);
        }
    }
}