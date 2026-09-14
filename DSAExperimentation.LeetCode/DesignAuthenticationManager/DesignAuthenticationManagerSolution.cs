using DSAExperimentation.DataStructures.HashMap;

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
// Every operation is keyed lookup, so the whole problem is a bet on how tokens are
// stored: a flat list the renew probe has to scan, or a hashed map.
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
    // them. Deliberately written without this repo's HashMap<TKey,TValue>.
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

    // The composed answer: a thin string-key/int-expiry wrapper directly over this
    // repo's own HashMap<TKey,TValue>. Generate and Renew map onto Set and
    // TryGetValue, and CountUnexpiredTokens reads HashMap's existing Values
    // snapshot rather than needing a second, purpose-built enumeration surface.
    internal sealed class AuthenticationManagerByHashMap(int timeToLive) : IAuthenticationManager
    {
        private readonly HashMap<string, int> _expiryByToken = new();

        public void Generate(string tokenId, int currentTime) =>
            _expiryByToken.Set(tokenId, currentTime + timeToLive);

        // An unknown token has nothing to renew, and an already-expired one is not
        // revived - LeetCode says both are ignored.
        public void Renew(string tokenId, int currentTime)
        {
            if (_expiryByToken.TryGetValue(tokenId, out var expiry) && expiry > currentTime)
            {
                _expiryByToken.Set(tokenId, currentTime + timeToLive);
            }
        }

        public int CountUnexpiredTokens(int currentTime)
        {
            var unexpired = 0;

            foreach (var expiry in _expiryByToken.Values)
            {
                if (expiry > currentTime)
                {
                    unexpired++;
                }
            }

            return unexpired;
        }
    }
}
