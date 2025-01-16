﻿namespace BuildingBlocks.SharedKernel.Caching;

public interface IHybridCacheService
{
    public Task<string> GetCachedDataAsync(string key, CancellationToken cancellationToken = default);
}
