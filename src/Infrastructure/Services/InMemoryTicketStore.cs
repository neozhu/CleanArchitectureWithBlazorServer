using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services
{
    /// <summary>
    /// ITicketStore implementation backed by FusionCache (no DI).
    /// - Process-wide singleton FusionCache via Lazy<T>.
    /// - Per-entry TTL aligned with AuthenticationTicket.ExpiresUtc to avoid "stale-but-valid" drift.
    /// - Resilience enabled (fail-safe / jitter / lock) with conservative values for auth sensitivity.
    /// </summary>
    public sealed class MemoryCacheTicketStore : ITicketStore
    {
        private const string KeyPrefix = "AuthSessionStore-";

        // Default entry options used when the ticket has no ExpiresUtc.
        // For authentication scenarios we keep fail-safe short to reduce security risk.
        private static readonly FusionCacheEntryOptions DefaultEntryOptions = new()
        {
            // Fallback absolute TTL if the ticket doesn't carry an explicit expiry.
            Duration = TimeSpan.FromDays(7),

            // —— Resilience: fail-safe & timeouts ——
            // Keep fail-safe short: if dependencies are flaky, we can serve a recent value briefly,
            // but avoid long windows for security-sensitive data.
            IsFailSafeEnabled = true,
            FailSafeMaxDuration = TimeSpan.FromMinutes(20),
            FailSafeThrottleDuration = TimeSpan.FromSeconds(15),

            // Factory timeouts mostly affect GetOrSet (not used here), but are kept for consistency.
            FactorySoftTimeout = TimeSpan.FromMilliseconds(250),
            FactoryHardTimeout = TimeSpan.FromMilliseconds(1000),

            // —— Anti-stampede ——
            // Spread expirations to mitigate thundering herds; short lock to avoid long waits.
            JitterMaxDuration = TimeSpan.FromSeconds(30),
            LockTimeout = TimeSpan.FromSeconds(1)
        };

        // Process-wide FusionCache singleton. Configure() may adjust options prior to first access.
        private static readonly Lazy<IFusionCache> LazyCache = new(() =>
        {
            var options = new FusionCacheOptions
            {
                CacheName = "AuthSessionStore",
                DefaultEntryOptions = DefaultEntryOptions
            };

            _configureAction?.Invoke(options);

            var cache = new FusionCache(options);

            // OPTIONAL (no DI):
            // If you want 2nd-level cache + backplane, wire them here:
            // cache.SetSerializer(new ZiggyCreatures.FusionCache.Serialization.SystemTextJson
            //     .FusionCacheSystemTextJsonSerializer());
            // cache.SetDistributedCache(yourDistributedCacheInstance /* IDistributedCache */);
            // cache.SetBackplane(yourBackplaneInstance /* e.g., RedisBackplane */);

            return cache;
        }, isThreadSafe: true);

        private static Action<FusionCacheOptions>? _configureAction;

        /// <summary>
        /// Allows customizing FusionCacheOptions BEFORE the first use.
        /// Throws if the cache was already created.
        /// </summary>
        public static void Configure(Action<FusionCacheOptions> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            if (LazyCache.IsValueCreated)
                throw new InvalidOperationException("FusionCache already initialized. Configure must be called before first use.");
            _configureAction = configure;
        }

        private static IFusionCache Cache => LazyCache.Value;

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            if (ticket is null) throw new ArgumentNullException(nameof(ticket));

            var key = KeyPrefix + Guid.NewGuid().ToString("N");
            await RenewAsync(key, ticket).ConfigureAwait(false);
            return key;
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key is required.", nameof(key));
            if (ticket is null) throw new ArgumentNullException(nameof(ticket));

            // Align cache TTL with ticket expiry when available.
            var options = BuildPerEntryOptions(ticket);

            // If the ticket is already expired (or nearly), do not cache it.
            if (options is null)
            {
                // No ExpiresUtc → fall back to defaults.
                await Cache.SetAsync(key, ticket, DefaultEntryOptions).ConfigureAwait(false);
                return;
            }

            if (options.Duration <= TimeSpan.Zero)
            {
                // Ticket considered expired; ensure removal to avoid serving stale auth data.
                await Cache.RemoveAsync(key).ConfigureAwait(false);
                return;
            }

            await Cache.SetAsync(key, ticket, options).ConfigureAwait(false);
        }

        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;

            // No factory: return null when missing.
            // With fail-safe enabled, a recent prior value may be served on transient failures.
            return await Cache.GetOrDefaultAsync<AuthenticationTicket>(key).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            await Cache.RemoveAsync(key).ConfigureAwait(false);
        }

        /// <summary>
        /// Builds per-entry options from AuthenticationTicket so that cache TTL equals the ticket lifetime.
        /// Returns:
        /// - FusionCacheEntryOptions with Duration if ExpiresUtc is present and in the future,
        /// - null if the ticket has no ExpiresUtc (caller uses DefaultEntryOptions),
        /// - options with small positive Duration if extremely close to expiry (optional),
        /// - Duration <= 0 means "expired": caller should not cache and may remove the key.
        /// </summary>
        private static FusionCacheEntryOptions? BuildPerEntryOptions(AuthenticationTicket ticket)
        {
            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc is null)
                return null;

            var now = DateTimeOffset.UtcNow;
            var ttl = expiresUtc.Value - now;

            // If already expired, caller will remove and not cache.
            // If extremely small but positive, you may choose to set a tiny TTL to smooth boundary conditions.
            if (ttl > TimeSpan.Zero && ttl < TimeSpan.FromSeconds(5))
                ttl = TimeSpan.FromSeconds(5);

            return new FusionCacheEntryOptions
            {
                Duration = ttl,

                // Keep resilience but short for auth data.
                IsFailSafeEnabled = true,
                FailSafeMaxDuration = TimeSpan.FromMinutes(20),
                FailSafeThrottleDuration = TimeSpan.FromSeconds(15),

                // We do not typically need eager refresh for tickets; renew happens via app logic.
                // EagerRefreshThreshold = ...

                // Anti-stampede for concurrent renewals.
                JitterMaxDuration = TimeSpan.FromSeconds(30),
                LockTimeout = TimeSpan.FromSeconds(1)
            };
        }
    }
}
