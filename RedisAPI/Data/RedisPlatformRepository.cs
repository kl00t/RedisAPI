using System.Text.Json;

namespace RedisAPI.Data;

public class RedisPlatformRepository : IPlatformRepository
{
    private readonly IConnectionMultiplexer _redis;

    public RedisPlatformRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public void CreatePlatform(Platform platform)
    {
        if(platform == null)
        {
            throw new ArgumentNullException(nameof(platform));
        }

        var database = _redis.GetDatabase();
        var jsonPlatform = JsonSerializer.Serialize(platform);
        database.StringSet(platform.Id, jsonPlatform);
        database.SetAdd("PlatformsSet", jsonPlatform);
    }

    public IEnumerable<Platform?>? GetAllPlatforms()
    {
        var database = _redis.GetDatabase();
        var platformsSet = database.SetMembers("PlatformsSet");
        if (platformsSet.Length > 0)
        {
            var platformsList = Array.ConvertAll(platformsSet, val => JsonSerializer.Deserialize<Platform>(val)).ToList();
            return platformsList;
        }

        return null;
    }

    public Platform? GetPlatformById(string id)
    {
        var database = _redis.GetDatabase();
        var jsonPlatform = database.StringGet(id);
        if (!string.IsNullOrEmpty(jsonPlatform))
        {
            return JsonSerializer.Deserialize<Platform>(jsonPlatform);
        }

        return null;
    }
}