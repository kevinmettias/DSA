using CountedBitTrieStructure = DSAExperimentation.DataStructures.CountedBitTrie.CountedBitTrie;

namespace DSAExperimentation.Tests.DataStructures.CountedBitTrie;

// The tables pin hand-derived cases, each row's arithmetic in the comment above it; the seeded
// tests replay one long script of inserts and removals and check every answer against a
// brute-force scan of a plain List<int> holding the same multiset. A row's removals all name
// values its inserts put in, so each one is expected to succeed.
public sealed partial class CountedBitTrieTests
{
    public static TheoryData<int[], int[], int> CountCases =>
        new()
        {
            { [], [], 0 },

            // Duplicates count once per copy.
            { [7, 7, 1], [], 3 },
            { [7, 7, 1], [7], 2 },
            { [7, 7, 1], [7, 7, 1], 0 },
        };

    // Inserted, removed first, then the value TryRemove is asked for: whether it succeeds, and the
    // Count left behind.
    public static TheoryData<int[], int[], int, bool, int> TryRemoveCases =>
        new()
        {
            { [], [], 7, false, 0 },
            { [7], [], 7, true, 0 },

            // A multiset: one copy goes, the other stays live.
            { [7, 7], [], 7, true, 1 },

            // 7 = 0b111 shares all 31 upper bits with 6 = 0b110 and leaves the laid-down path only
            // at bit 0.
            { [6], [], 7, false, 1 },

            // -6 sets bit 31, so its path leaves 6's at the root.
            { [6], [], -6, false, 1 },

            // 5's leaf is still laid down but holds no live copy - beside a live sibling, 4.
            { [5], [5], 5, false, 0 },
            { [4, 5], [5], 5, false, 1 },
        };

    // Inserted, removed, the query: whether anything is live, and the largest query ^ y.
    public static TheoryData<int[], int[], int, bool, int> TryMaxXorCases =>
        new()
        {
            { [], [], 5, false, 0 },

            // Every value removed: the nodes remain, nothing is live.
            { [4, 6], [4, 6], 5, false, 0 },

            // LeetCode 421's example: 5 ^ 3 = 6, 5 ^ 10 = 15, 5 ^ 5 = 0, 5 ^ 25 = 28, 5 ^ 2 = 7,
            // 5 ^ 8 = 13, so 28.
            { [3, 10, 5, 25, 2, 8], [], 5, true, 28 },

            // 6 ^ 1 = 7, 6 ^ 8 = 14, 6 ^ 14 = 8. With 8 removed its path still branches opposite
            // 6's bit at bit 2, but holds nothing live, so the walk must take 14's: 8.
            { [1, 8, 14], [8], 6, true, 8 },

            // 0's whole branch is dead from bit 3 down; only 15 ^ 15 = 0 is left.
            { [0, 15], [0], 15, true, 0 },

            // One copy of 7 removed, the other still live: 0 ^ 7 = 7 beats 0 ^ 1 = 1.
            { [7, 7, 1], [7], 0, true, 7 },
            { [9], [], 9, true, 0 },

            // Unsigned reading: 0 ^ -1 is the all-ones pattern, the largest there is, read back as
            // the int -1.
            { [-1, 1], [], 0, true, -1 },
        };

    // Inserted, removed, value, limit: how many live y have value ^ y below limit, unsigned.
    public static TheoryData<int[], int[], int, int, int> CountXorBelowCases =>
        new()
        {
            { [], [], 3, 100, 0 },

            // 3 ^ 3 = 0, which is not below 0.
            { [3, 3], [], 3, 0, 0 },

            // 3 ^ 3 = 0 < 1 for every copy.
            { [3, 3, 3], [], 3, 1, 3 },

            // 3 ^ 5 = 6: not strictly below 6, below 7.
            { [5], [], 3, 6, 0 },
            { [5], [], 3, 7, 1 },

            // Removed values are gone: 0 ^ 1 = 1 and 0 ^ 3 = 3 are below 4, 2 was removed.
            { [1, 2, 3], [2], 0, 4, 2 },

            // LeetCode 1803's first example, 7 against the earlier [1, 4, 2]: 6, 3 and 5 are all
            // below high + 1 = 7 and none below low = 2.
            { [1, 4, 2], [], 7, 7, 3 },
            { [1, 4, 2], [], 7, 2, 0 },

            // int.MinValue is the unsigned 2^31: 0 ^ 0 = 0 and 0 ^ int.MaxValue = 2^31 - 1 are
            // below it, 0 ^ -1 = 2^32 - 1 is not.
            { [0, int.MaxValue, -1], [], 0, int.MinValue, 2 },

            // -1 is the unsigned 2^32 - 1: everything below it but the all-ones XOR.
            { [0, -1], [], 0, -1, 1 },
        };

    [Theory]
    [MemberData(nameof(CountCases))]
    public void Count_AfterInsertsAndRemovals_IsTheLiveMultisetSize(int[] inserted, int[] removed, int expected)
    {
        var trie = Build(inserted, removed);

        Assert.Equal(expected, trie.Count);
    }

    [Fact]
    public void Insert_AfterEveryCopyWasRemoved_MakesTheValueLiveAgain()
    {
        var trie = Build([Fixtures.ReinsertedValue], [Fixtures.ReinsertedValue]);

        trie.Insert(Fixtures.ReinsertedValue);
        var copies = trie.CountXorBelow(Fixtures.ReinsertedValue, Fixtures.OnlyAnEqualValue);

        Assert.Equal(Fixtures.OneCopy, copies);
        Assert.Equal(Fixtures.OneCopy, trie.Count);
    }

    // Enough distinct values to double the node storage several times: every value must still be
    // found on its own path, so no link was written into storage a doubling discarded.
    [Fact]
    public void Insert_ManyDistinctValues_KeepsEveryPathAcrossStorageGrowth()
    {
        var trie = new CountedBitTrieStructure();

        for (var value = 0; value < Fixtures.DistinctValueCount; value++)
        {
            trie.Insert(value);
        }

        for (var value = 0; value < Fixtures.DistinctValueCount; value++)
        {
            var copies = trie.CountXorBelow(value, Fixtures.OnlyAnEqualValue);
            Assert.Equal(Fixtures.OneCopy, copies);
        }
    }

    [Theory]
    [MemberData(nameof(TryRemoveCases))]
    public void TryRemove_Cases_ReportsWhetherACopyWasLiveAndLeavesTheRestAlone(
        int[] inserted, int[] removed, int target, bool expectedRemoved, int expectedCount)
    {
        var trie = Build(inserted, removed);

        var wasRemoved = trie.TryRemove(target);

        Assert.Equal(expectedRemoved, wasRemoved);
        Assert.Equal(expectedCount, trie.Count);
    }

    [Fact]
    public void TryRemove_SeededInsertsAndRemovals_AgreesWithAListModel()
    {
        var random = new Random(Fixtures.Seed);
        var trie = new CountedBitTrieStructure();
        var live = new List<int>();

        for (var step = 0; step < Fixtures.ScriptLength; step++)
        {
            ApplySeededStep(trie, live, random);
        }

        Assert.Equal(live.Count, trie.Count);
    }

    [Theory]
    [MemberData(nameof(TryMaxXorCases))]
    public void TryMaxXor_Cases_ReturnsTheLargestXorAgainstALiveValue(
        int[] inserted, int[] removed, int query, bool expectedFound, int expectedResult)
    {
        var trie = Build(inserted, removed);

        var found = trie.TryMaxXor(query, out var result);

        Assert.Equal(expectedFound, found);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void TryMaxXor_SeededInsertsAndRemovals_MatchesABruteForceScanOfTheLiveValues()
    {
        var random = new Random(Fixtures.Seed);
        var trie = new CountedBitTrieStructure();
        var live = new List<int>();

        for (var step = 0; step < Fixtures.ScriptLength; step++)
        {
            ApplySeededStep(trie, live, random);
            var query = random.Next(int.MinValue, int.MaxValue);

            var found = trie.TryMaxXor(query, out var result);
            var expected = BruteForceMaxXor(live, query);

            Assert.Equal(live.Count > 0, found);
            Assert.Equal(expected, result);
        }
    }

    [Theory]
    [MemberData(nameof(CountXorBelowCases))]
    public void CountXorBelow_Cases_CountsLiveValuesWhoseXorIsStrictlyBelowTheLimit(
        int[] inserted, int[] removed, int value, int limit, int expected)
    {
        var trie = Build(inserted, removed);

        var count = trie.CountXorBelow(value, limit);

        Assert.Equal(expected, count);
    }

    // Each step asks twice: once against a limit drawn from the whole int range, so bit 31 of the
    // limit is set about half the time, and once against a limit inside the stored values' own
    // range, where the XORs mostly fall.
    [Fact]
    public void CountXorBelow_SeededInsertsAndRemovals_MatchesABruteForceScanOfTheLiveValues()
    {
        var random = new Random(Fixtures.Seed);
        var trie = new CountedBitTrieStructure();
        var live = new List<int>();

        for (var step = 0; step < Fixtures.ScriptLength; step++)
        {
            ApplySeededStep(trie, live, random);
            var value = random.Next(int.MinValue, int.MaxValue);
            var wideLimit = random.Next(int.MinValue, int.MaxValue);
            var narrowLimit = random.Next(Fixtures.StoredValueLow, Fixtures.StoredValueHigh);

            var wide = trie.CountXorBelow(value, wideLimit);
            var narrow = trie.CountXorBelow(value, narrowLimit);
            var expectedWide = BruteForceCountXorBelow(live, value, wideLimit);
            var expectedNarrow = BruteForceCountXorBelow(live, value, narrowLimit);

            Assert.Equal(expectedWide, wide);
            Assert.Equal(expectedNarrow, narrow);
        }
    }

    private static CountedBitTrieStructure Build(int[] inserted, int[] removed)
    {
        var trie = new CountedBitTrieStructure();

        foreach (var value in inserted)
        {
            trie.Insert(value);
        }

        foreach (var value in removed)
        {
            var wasRemoved = trie.TryRemove(value);
            Assert.True(wasRemoved);
        }

        return trie;
    }

    // One step of the script, applied to the trie and the model alike: an insert or a removal with
    // even odds, of a value from a range narrow enough that duplicates pile up and a removal often
    // names a value absent or already gone. The range straddles zero, so bit 31 differs between
    // stored values too.
    private static void ApplySeededStep(CountedBitTrieStructure trie, List<int> live, Random random)
    {
        var value = random.Next(Fixtures.StoredValueLow, Fixtures.StoredValueHigh);
        var isRemoval = random.Next(Fixtures.StepKinds) == 0;

        if (isRemoval)
        {
            var wasLive = live.Remove(value);
            var wasRemoved = trie.TryRemove(value);
            Assert.Equal(wasLive, wasRemoved);
        }
        else
        {
            live.Add(value);
            trie.Insert(value);
        }

        Assert.Equal(live.Count, trie.Count);
    }

    // The largest query ^ y read unsigned, cast back to int; 0 for an empty list, TryMaxXor's own
    // out value when it finds nothing.
    private static int BruteForceMaxXor(List<int> live, int query)
    {
        var best = 0u;

        foreach (var value in live)
        {
            best = Math.Max(best, unchecked((uint)(query ^ value)));
        }

        return unchecked((int)best);
    }

    private static int BruteForceCountXorBelow(List<int> live, int value, int limit)
    {
        var count = 0;

        foreach (var stored in live)
        {
            if (unchecked((uint)(value ^ stored)) < unchecked((uint)limit))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// The values these tests assert against, named once so a second reader does not have to reach
    /// into a test body for the number the first one used.
    /// </summary>
    private static class Fixtures
    {
        public const int ReinsertedValue = 9;

        // v ^ y < 1 holds only for y == v, so this limit counts the copies of v.
        public const int OnlyAnEqualValue = 1;
        public const int OneCopy = 1;

        // 2,000 distinct values lay down a few thousand nodes against an initial room for 34.
        public const int DistinctValueCount = 2_000;

        public const int Seed = 1_803;
        public const int ScriptLength = 2_000;
        public const int StoredValueLow = -16;
        public const int StoredValueHigh = 48;
        public const int StepKinds = 2;
    }
}
