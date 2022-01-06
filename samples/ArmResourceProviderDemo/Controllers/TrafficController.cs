using System.Collections.Generic;
using ArmResourceProviderDemo.WebModels.Traffic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ArmResourceProviderDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrafficController : ControllerBase
    {
        private static readonly Dictionary<string, TrafficResource> Db = new Dictionary<string, TrafficResource>();
        private readonly ILogger<TrafficController> _logger;

        public TrafficController(ILogger<TrafficController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<TrafficResource> Get()
        {
            return Db.Values;
        }

        [HttpPut]
        public TrafficResource Put(TrafficResource traffic)
        {
            Db[traffic.Id] = traffic;
            return traffic;
        }
    }
}
