using DSAExperimentation.Mechanisms.ExpiringSet;

namespace DSAExperimentation.LeetCode.DesignAuthenticationManager;

// LeetCode 1797. Design Authentication Manager: tokens that expire timeToLive
// seconds after they were generated or last renewed, with a count of the ones
// still alive at a given time.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and three operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md §17.3) takes the form of two full classes
// implementing the shared IAuthenticationManager surface below, the same shape
// DesignBrowserHistorySolution uses for its own instance-API problem (LC 1472).
//
// The problem is a time-to-live mechanism: a flat list the renew probe and the count
// both scan, or this repo's own ExpiringSet, which lets lapsed tokens go in expiry
// order (ARCHITECTURE.md §17.6b).
internal static class DesignAuthenticationManagerSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IAuthenticationManager
    {
        void Generate(string tokenId, int currentTime);

        void Renew(string tokenId, int currentTime);

        int CountUnexpiredTokens(int currentTime);
    }

    // The textbook baseline this composition has to justify itself against: a BCL
    // List of (token, expiry) pairs with no hashing at all, so every renew probe
    // walks the entries until it finds its token - or, on an unknown id, all of
    // them. Deliberately written without this repo's ExpiringSet<TKey>.
    //
    // Generate appends rather than replacing, which is safe because LeetCode
    // guarantees each generated tokenId is unique.
    internal sealed class AuthenticationManagerByLinearScanList(int timeToLive) : IAuthenticationManager
    {
        private readonly List<(string TokenId, int Expiry)> _entries = [];

        public void Generate(string tokenId, int currentTime) =>
            _entries.Add((tokenId, currentTime + timeToLive));

        public void Renew(string tokenId, int currentTime)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].TokenId != tokenId)
                {
                    continue;
                }

                if (_entries[i].Expiry > currentTime)
                {
                    _entries[i] = (tokenId, currentTime + timeToLive);
                }

                return;
            }
        }

        public int CountUnexpiredTokens(int currentTime)
        {
            var unexpired = 0;

            foreach (var entry in _entries)
            {
                if (entry.Expiry > currentTime)
                {
                    unexpired++;
                }
            }

            return unexpired;
        }
    }

    // The composed answer: this repo's own ExpiringSet<string>, the time-to-live
    // mechanism itself. Generate starts a token's life, Renew restarts a living one
    // - an unknown or lapsed token stays out, which LeetCode says is ignored - and
    // the count lets every lapsed token go before it answers.
    internal sealed class AuthenticationManagerByExpiringSet(int timeToLive) : IAuthenticationManager
    {
        private readonly ExpiringSet<string> _tokens = new(timeToLive);

        public void Generate(string tokenId, int currentTime) => _tokens.Set(tokenId, currentTime);

        public void Renew(string tokenId, int currentTime) => _ = _tokens.TryRenew(tokenId, currentTime);

        public int CountUnexpiredTokens(int currentTime) => _tokens.CountAlive(currentTime);
    }
}
