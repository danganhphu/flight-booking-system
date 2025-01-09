﻿namespace FoodExpress.ApiGateway;


internal sealed class RedisSessionStore(IServiceCollection services) : ITicketStore
{
    private const string KeyPrefix = "_oauth2_proxy-";

    private readonly DistributedCacheEntryOptions _cacheEntryOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
    };

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        
        // Get services
        var distributedCache = scope.ServiceProvider.GetService<IDistributedCache>()!;
        var logger = scope.ServiceProvider.GetService<ILogger<RedisSessionStore>>()!;
        
        var key = KeyPrefix + Guid.NewGuid();

        var serializedTicket = TicketSerializer.Default.Serialize(ticket);
        await distributedCache.SetAsync(key, serializedTicket, _cacheEntryOptions);
        
        logger.LogInformation("Storing auth ticket with key {authTicketKey}", key);
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        
        // Get services
        var distributedCache = scope.ServiceProvider.GetService<IDistributedCache>()!;
        var logger = scope.ServiceProvider.GetService<ILogger<RedisSessionStore>>()!;
        
        logger.LogInformation("Renew ticket with key {authTicketKey} and schema {authSchema}", key,
            ticket.AuthenticationScheme);
        
        await distributedCache.SetAsync(key, TicketSerializer.Default.Serialize(ticket), _cacheEntryOptions);
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        // Get services
        var distributedCache = scope.ServiceProvider.GetService<IDistributedCache>()!;
        var logger = scope.ServiceProvider.GetService<ILogger<RedisSessionStore>>()!;
        
        logger.LogInformation("Getting ticket with {authTicketKey}", key);
        
        var cachedMember = await distributedCache.GetAsync(key);
        if (cachedMember == null)
        {
            return null;
        }

        var ticket = TicketSerializer.Default.Deserialize(cachedMember);
        return ticket;
    }

    public async Task RemoveAsync(string key)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        
        // Get services
        var distributedCache = scope.ServiceProvider.GetService<IDistributedCache>()!;
        var logger = scope.ServiceProvider.GetService<ILogger<RedisSessionStore>>()!;
        
        var ticket = await distributedCache.GetAsync(key);
        if (ticket != null)
        {
            await distributedCache.RemoveAsync(key);
            logger.LogInformation("Removing ticket with key {authTicketKey}", key);
        }
    }
}