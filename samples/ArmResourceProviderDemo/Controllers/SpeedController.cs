using ArmResourceProviderDemo.WebModels;
using ArmResourceProviderDemo.WebModels.Speed;
using Microsoft.AspNetCore.Mvc;

namespace ArmResourceProviderDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpeedController : ControllerBase
    {
        [HttpGet]
        public ResourceProxy<SpeedProperties> Get()
        {
            return new ResourceProxy<SpeedProperties> { };
        }
    }
}
