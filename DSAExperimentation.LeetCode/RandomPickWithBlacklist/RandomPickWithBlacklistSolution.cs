using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.RandomPickWithBlacklist;

// LeetCode 710. Random Pick with Blacklist: pick a uniformly random int in [0, n)
// that is never one of the given blacklisted numbers, across repeated Pick() calls.
//
// This is a design problem - a stateful object built once, then Pick() called many
// times - so the strategy choice is which representation the constructor-equivalent
// setup builds, the same CreateBy<Strategy> factory shape LRUCacheSolution/
// AllOneDataStructureSolution use for their own design problems.
//
// CreateBySetHashMapRemap is the standard O(B) remap solution: a Set<int> (blacklist
// membership, used only while scanning for remap targets) plus a HashMap<int,int>
// (every blacklisted number below the whitelist boundary M = N - blacklist.Length
// remapped, once, to a whitelisted number >= M) - built entirely up front so every
// Pick() afterward is a single random draw plus one O(1)-expected lookup, never a
// per-call rescan of the blacklist the way rejection sampling needs.
//
// CreateByRejectionSampling is the textbook baseline this composition has to justify
// itself against: redraw from [0, n) until landing outside the blacklist, using a BCL
// HashSet<int> for membership. Its expected retries per pick grow with how much of
// [0, n) is blacklisted, which the remap solution avoids entirely by construction.
internal static class RandomPickWithBlacklistSolution
{
    public static IRandomPick CreateByRejectionSampling(int n, int[] blacklist, int seed) =>
        new RejectionSamplingRandomPick(n, blacklist, seed);

    public static IRandomPick CreateBySetHashMapRemap(int n, int[] blacklist, int seed) =>
        new SetHashMapRemapRandomPick(n, blacklist, seed);

    internal interface IRandomPick
    {
        int Pick();
    }

    private sealed class RejectionSamplingRandomPick : IRandomPick
    {
        private readonly HashSet<int> _blacklisted;
        private readonly Random _random;
        private readonly int _n;

        public RejectionSamplingRandomPick(int n, int[] blacklist, int seed)
        {
            _n = n;
            _blacklisted = new HashSet<int>(blacklist);
            _random = new Random(seed);
        }

        public int Pick()
        {
            int candidate;
            do
            {
                candidate = _random.Next(_n);
            }
            while (_blacklisted.Contains(candidate));

            return candidate;
        }
    }

    private sealed class SetHashMapRemapRandomPick : IRandomPick
    {
        private readonly HashMap<int, int> _remap = new();
        private readonly Random _random;
        private readonly int _whitelistBound;

        public SetHashMapRemapRandomPick(int n, int[] blacklist, int seed)
        {
            _random = new Random(seed);
            _whitelistBound = n - blacklist.Length;

            var blacklistedSet = new Set<int>();
            foreach (var value in blacklist)
            {
                blacklistedSet.TryAdd(value);
            }

            var nextWhitelisted = _whitelistBound;
            foreach (var value in blacklist)
            {
                nextWhitelisted = MapIfNeeded(value, blacklistedSet, nextWhitelisted);
            }
        }

        private int MapIfNeeded(int value, Set<int> blacklistedSet, int nextWhitelisted)
        {
            if (value >= _whitelistBound)
            {
                return nextWhitelisted; // already outside the drawn range, needs no remap target
            }

            while (blacklistedSet.Has(nextWhitelisted))
            {
                nextWhitelisted++;
            }

            _remap.Set(value, nextWhitelisted);
            return nextWhitelisted + 1;
        }

        public int Pick()
        {
            var candidate = _random.Next(_whitelistBound);
            return _remap.TryGetValue(candidate, out var mapped) ? mapped : candidate;
        }
    }
}
