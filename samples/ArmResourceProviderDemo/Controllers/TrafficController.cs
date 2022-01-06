using ArmResourceProviderDemo.WebModels;
using ArmResourceProviderDemo.WebModels.Traffic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ArmResourceProviderDemo.Controllers
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
        public TrafficResource Put(TrafficResource traffic)
        {
            _db[traffic.Id] = traffic;
            return traffic;
        }
    }
}
