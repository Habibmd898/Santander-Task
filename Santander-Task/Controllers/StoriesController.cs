using Microsoft.AspNetCore.Mvc;
using Santander_Task.APIManager;
using Santander_Task.DataModel;

namespace Santander_Task.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoriesController : ControllerBase
    {

        private readonly IBestStoriesAPI _api;

        public StoriesController(IBestStoriesAPI api)
        {
            _api = api;
        }

        [HttpGet(Name = "bestStories")]
        public async Task<IActionResult> Best([FromQuery] int numberOfStories = 10)
        {
            return Ok(await _api.GetBestStories(numberOfStories));
        }
    }
}