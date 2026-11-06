using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.Solutions.RandomPickWithBlacklist;

// LeetCode 710. Random Pick with Blacklist: pick a uniformly random int in [0, rangeSize)
// that is never one of the given blacklisted numbers, across repeated Pick() calls.
//
// This is a design problem - a stateful object built once, then Pick() called many
// times - so the strategy choice is which representation the constructor-equivalent
// setup builds, the same CreateBy<Strategy> factory shape LRUCacheSolution/
// AllOneDataStructureSolution use for their own design problems.
//
// CreateBySetHashMapRemap is the standard O(B) remap solution: a Set<int> (blacklist
// membership, used only while scanning for remap targets) plus a HashMap<int,int>
// (every blacklisted number below the whitelist boundary rangeSize - blacklist.Length
// remapped, once, to a whitelisted number at or above it) - built entirely up front so
// every Pick() afterward is a single random draw plus one O(1)-expected lookup, never a
// per-call rescan of the blacklist the way rejection sampling needs.
//
// CreateByRejectionSampling is the textbook baseline this composition has to justify
// itself against: redraw from [0, rangeSize) until landing outside the blacklist, using
// a BCL HashSet<int> for membership. Its expected retries per pick grow with how much of
// [0, rangeSize) is blacklisted, which the remap solution avoids entirely by construction.
//
// LeetCode accepts any whitelisted number for each pick, so no answer can match a
// recording: RandomPickWithBlacklistAnswerCheck judges a whole script.
[LeetCodeQuestion("random-pick-with-blacklist", LeetCodeAnswerOrder.AsWritten, AnswerCheck = typeof(RandomPickWithBlacklistAnswerCheck))]
internal static class RandomPickWithBlacklistSolution
{
    // LeetCode's constructor takes the range and the blacklist alone, so the overloads
    // of LeetCode's shape fix the seed: a call script replayed twice, or once per
    // benchmark iteration, picks the same numbers.
    private const int Seed = 1;

    public static IRandomPick CreateByRejectionSampling(int rangeSize, int[] blacklist, int seed) =>
        new RejectionSamplingRandomPick(rangeSize, blacklist, seed);

    // Each strategy as LeetCode's constructor makes it, so the catalog harness can make
    // the object its call scripts run on (ARCHITECTURE 17.13).
    public static IRandomPick CreateByRejectionSampling(int rangeSize, int[] blacklist) =>
        CreateByRejectionSampling(rangeSize, blacklist, Seed);

    public static IRandomPick CreateBySetHashMapRemap(int rangeSize, int[] blacklist, int seed) =>
        new SetHashMapRemapRandomPick(rangeSize, blacklist, seed);

    public static IRandomPick CreateBySetHashMapRemap(int rangeSize, int[] blacklist) =>
        CreateBySetHashMapRemap(rangeSize, blacklist, Seed);

    internal interface IRandomPick
    {
        int Pick();
    }

    private sealed class RejectionSamplingRandomPick : IRandomPick
    {
        private readonly HashSet<int> _blacklisted;
        private readonly Random _random;
        private readonly int _rangeSize;

        public RejectionSamplingRandomPick(int rangeSize, int[] blacklist, int seed)
        {
            _rangeSize = rangeSize;
            _blacklisted = new HashSet<int>(blacklist);
            _random = new Random(seed);
        }

        public int Pick()
        {
            int candidate;
            do
            {
                candidate = _random.Next(_rangeSize);
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

        public SetHashMapRemapRandomPick(int rangeSize, int[] blacklist, int seed)
        {
            _random = new Random(seed);
            _whitelistBound = rangeSize - blacklist.Length;

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
