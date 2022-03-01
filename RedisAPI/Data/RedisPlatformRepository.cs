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
        database.HashSet("hashPlatform", new HashEntry[]
        {
            new HashEntry(platform.Id, jsonPlatform)
        });
    }

    public IEnumerable<Platform?>? GetAllPlatforms()
    {
        var database = _redis.GetDatabase();
        var completeHash = database.HashGetAll("hashPlatform");
        if (completeHash.Length > 0)
        {
            return Array.ConvertAll(completeHash, val => JsonSerializer.Deserialize<Platform>(val.Value)).ToList();
        }

        return null;
    }

    public Platform? GetPlatformById(string id)
    {
        var database = _redis.GetDatabase();
        var jsonPlatform = database.HashGet("hashPlatform", id);
        if (!string.IsNullOrEmpty(jsonPlatform))
        {
            return JsonSerializer.Deserialize<Platform>(jsonPlatform);
        }

        return null;
    }
}