using System.Text.Json;
using PixelHub.Api.Models;
using StackExchange.Redis;

namespace PixelHub.Api.Repositories;

public sealed class CachedGameCatalog : IGameCatalog
{
    private const string KeyAll = "cache:catalogue:tous";
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);
    private readonly IGameCatalog _source;
    private readonly IDatabase _cache;

    public CachedGameCatalog(IGameCatalog source, IConnectionMultiplexer redis)
    {
        _source = source;
        _cache = redis.GetDatabase();
    }

    public async Task<List<Game>> GetAllAsync()
    {
        try
        {
            var cached = await _cache.StringGetAsync(KeyAll);
            if (cached.HasValue)
                return JsonSerializer.Deserialize<List<Game>>((string)cached!)!;
        }
        catch (RedisException) { return await _source.GetAllAsync(); }
        var games = await _source.GetAllAsync();
        try { await _cache.StringSetAsync(KeyAll, JsonSerializer.Serialize(games), Ttl); }
        catch (RedisException) { /* Le catalogue MongoDB reste disponible même si l'écriture du cache échoue. */ }
        return games;
    }

    public Task<Game?> GetByTitleAsync(string titre) => _source.GetByTitleAsync(titre);
    public Task<List<Game>> GetByTitlesAsync(IEnumerable<string> titres) => _source.GetByTitlesAsync(titres);
    public Task<List<Game>> GetByGenreAsync(string genre) => _source.GetByGenreAsync(genre);

    public Task<List<Game>> GetTopRatedAsync(int count) => _source.GetTopRatedAsync(count);

    public Task<List<GenreStats>> GetStatsByGenreAsync() => _source.GetStatsByGenreAsync();
}