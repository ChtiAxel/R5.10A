using StackExchange.Redis;

namespace PixelHub.Api.Repositories;

public sealed class RedisMatchmakingQueue
{
    private const string Key = "file:matchmaking";
    private readonly IDatabase _db;

    public RedisMatchmakingQueue(IConnectionMultiplexer redis) => _db = redis.GetDatabase();

    public async Task EnqueueAsync(string pseudo) =>
        await _db.ListLeftPushAsync(Key, pseudo);

    public async Task<string?> DequeueAsync() =>
        await _db.ListRightPopAsync(Key);

    public async Task<long> LengthAsync() =>
        await _db.ListLengthAsync(Key);

    public async Task<(string Joueur1, string Joueur2)?> TryMatchAsync()
    {
        if (await LengthAsync() < 2)
            return null;

        var joueur1 = await DequeueAsync();
        var joueur2 = await DequeueAsync();
        return joueur1 is null || joueur2 is null ? null : (joueur1, joueur2);
    }
}