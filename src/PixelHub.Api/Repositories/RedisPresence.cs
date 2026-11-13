using StackExchange.Redis;

namespace PixelHub.Api.Repositories;

public sealed class RedisPresence
{
    private const string Key = "presence:joueurs";
    private static readonly TimeSpan OnlineWindow = TimeSpan.FromSeconds(60);
    private readonly IDatabase _db;

    public RedisPresence(IConnectionMultiplexer redis) => _db = redis.GetDatabase();

    public async Task TouchAsync(string pseudo)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await _db.SortedSetAddAsync(Key, pseudo, now);
    }

    public async Task<long> CountOnlineAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Subtract(OnlineWindow).ToUnixTimeSeconds();
        await _db.SortedSetRemoveRangeByScoreAsync(Key, double.NegativeInfinity, threshold);
        return await _db.SortedSetLengthAsync(Key);
    }
}