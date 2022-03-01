using Microsoft.AspNetCore.Mvc;

namespace RedisAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly IPlatformRepository _platformRepository;

    public PlatformController(IPlatformRepository platformRepository)
    {
        _platformRepository = platformRepository;
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<Platform> GetPlatformById(string id)
    {
        var platform = _platformRepository.GetPlatformById(id);
        if (platform != null)
        {
            return Ok(platform);
        }

        return NotFound();
    }

    [HttpPost]
    public ActionResult<Platform> CreatePlatform(Platform platform)
    {
        _platformRepository.CreatePlatform(platform);
        return CreatedAtRoute(nameof(GetPlatformById), new { platform.Id }, platform);
    }
}