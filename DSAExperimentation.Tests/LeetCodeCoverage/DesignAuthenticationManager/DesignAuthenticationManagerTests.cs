using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAuthenticationManager;

// LeetCode 1797. Design Authentication Manager: a thin string-key/int-expiry
// wrapper directly over this repo's own HashMap<TKey,TValue> - Generate/Renew map
// onto Set/TryGetValue, and CountUnexpiredTokens reads HashMap's existing Values
// snapshot rather than needing a second, purpose-built enumeration surface.
public sealed partial class DesignAuthenticationManagerTests
{
    [Fact]
    public void GenerateRenewCountUnexpiredTokens_TracksExpiryAcrossCalls()
    {
        var manager = new AuthenticationManager(timeToLive: 5);

        manager.Generate("1", currentTime: 1); // expires at 6
        manager.Generate("2", currentTime: 2); // expires at 7
        Assert.Equal(2, manager.CountUnexpiredTokens(currentTime: 4));

        manager.Renew("1", currentTime: 6); // token 1 already expired (6 <= 6) - ignored
        Assert.Equal(1, manager.CountUnexpiredTokens(currentTime: 6));

        manager.Renew("2", currentTime: 6); // token 2 still unexpired (7 > 6) - renewed to 11
        Assert.Equal(1, manager.CountUnexpiredTokens(currentTime: 10));
        Assert.Equal(0, manager.CountUnexpiredTokens(currentTime: 11));
    }

    [Fact]
    public void Renew_UnknownOrExpiredToken_IsNoOp()
    {
        var manager = new AuthenticationManager(timeToLive: 3);

        manager.Generate("1", currentTime: 1); // expires at 4
        manager.Renew("missing", currentTime: 2); // no such token - ignored
        Assert.Equal(1, manager.CountUnexpiredTokens(currentTime: 2));

        manager.Renew("1", currentTime: 5); // already expired (4 <= 5) - ignored
        Assert.Equal(0, manager.CountUnexpiredTokens(currentTime: 5));
    }

    private sealed class AuthenticationManager(int timeToLive)
    {
        private readonly HashMap<string, int> _expiryByToken = new();

        public void Generate(string tokenId, int currentTime) => _expiryByToken.Set(tokenId, currentTime + timeToLive);

        public void Renew(string tokenId, int currentTime)
        {
            if (_expiryByToken.TryGetValue(tokenId, out var expiry) && expiry > currentTime)
            {
                _expiryByToken.Set(tokenId, currentTime + timeToLive);
            }
        }

        public int CountUnexpiredTokens(int currentTime)
        {
            var count = 0;

            foreach (var expiry in _expiryByToken.Values)
            {
                if (expiry > currentTime)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
