using hey_url_challenge_code_dotnet.Models;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace hey_url_challenge_code_dotnet.Controllers
{
    [Route("api/stats")]
    public class StatsController : JsonApiController<Url, Guid>
    {
        public StatsController(IJsonApiOptions options, ILoggerFactory loggerFactory, IResourceService<Url, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}