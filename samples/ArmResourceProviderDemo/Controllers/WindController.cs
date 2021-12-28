using ArmResourceProviderDemo.WebModels;
using ArmResourceProviderDemo.WebModels.Wind;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmResourceProviderDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WindController : ControllerBase
    {
        private static readonly Dictionary<string, WindResource> _db = new Dictionary<string, WindResource>();
        private readonly ILogger<WindController> _logger;

        public WindController(ILogger<WindController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WindResource> Get()
        {
            return _db.Values;
        }

        [HttpPut]
        public IActionResult Put(WindResource traffic)
        {
            _db[traffic.Id] = traffic;
            return Ok();
        }
    }
}
