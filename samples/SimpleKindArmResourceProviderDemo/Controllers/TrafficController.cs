using SimpleKindArmResourceProviderDemo.WebModels;
using SimpleKindArmResourceProviderDemo.WebModels.Traffic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleKindArmResourceProviderDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrafficController : ControllerBase
    {
        private static readonly Dictionary<string, TrafficResource> _db = new Dictionary<string, TrafficResource>();
        private readonly ILogger<TrafficController> _logger;

        public TrafficController(ILogger<TrafficController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<TrafficResource> Get()
        {
            return _db.Values;
        }

        [HttpPut]
        public IActionResult Put(TrafficResource traffic)
        {
            _db[traffic.Id] = traffic;
            return Ok();
        }
    }
}
