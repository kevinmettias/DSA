using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomPickWithBlacklist;

// LeetCode 710. Random Pick with Blacklist: the standard O(B) remap solution - a
// Set<int> (blacklist membership, used only while scanning for remap targets) plus a
// HashMap<int,int> (every blacklisted number below the whitelist boundary
// M = N - blacklist.Length remapped, once, to a whitelisted number >= M) - built entirely
// in the constructor so every Pick() afterward is a single random draw plus one
// O(1)-expected lookup, never a per-call rescan of the blacklist the way naive rejection
// sampling needs.
public sealed partial class RandomPickWithBlacklistTests
{
    [Fact]
    public void Pick_NeverReturnsABlacklistedNumber()
    {
        var solution = new Solution(7, [2, 3, 5], seed: 1);
        var blacklisted = new HashSet<int> { 2, 3, 5 };

        for (var i = 0; i < 200; i++)
        {
            var picked = solution.Pick();
            Assert.InRange(picked, 0, 6);
            Assert.DoesNotContain(picked, blacklisted);
        }
    }

    [Fact]
    public void Pick_ManyCalls_EventuallyReturnsEveryWhitelistedNumber()
    {
        var solution = new Solution(5, [1, 3], seed: 1);
        var seen = new HashSet<int>();

        for (var i = 0; i < 200; i++)
        {
            seen.Add(solution.Pick());
        }

        Assert.Equal(new[] { 0, 2, 4 }, seen.OrderBy(x => x));
    }

    [Fact]
    public void Pick_EmptyBlacklist_AlwaysReturnsWithinRange()
    {
        var solution = new Solution(3, [], seed: 1);

        for (var i = 0; i < 20; i++)
        {
            Assert.InRange(solution.Pick(), 0, 2);
        }
    }

    private sealed class Solution
    {
        private readonly HashMap<int, int> _remap = new();
        private readonly Random _random;
        private readonly int _whitelistBound;

        public Solution(int n, int[] blacklist, int seed)
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
                if (value >= _whitelistBound)
                {
                    continue; // already outside the drawn range, needs no remap target
                }

                while (blacklistedSet.Has(nextWhitelisted))
                {
                    nextWhitelisted++;
                }

                _remap.Set(value, nextWhitelisted);
                nextWhitelisted++;
            }
        }

        public int Pick()
        {
            var candidate = _random.Next(_whitelistBound);
            return _remap.TryGetValue(candidate, out var mapped) ? mapped : candidate;
        }
    }
}
