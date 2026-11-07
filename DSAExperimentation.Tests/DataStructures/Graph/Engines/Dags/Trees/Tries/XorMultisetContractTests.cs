using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// IXorMultiset's contract, written once and run over every multiset that implements it: the one
// trie's CountedBitTrie, through its view CountedBitTrieMultiset (CountedBitTrieMultisetTests), and
// PooledCountedBitTrie, the hand-specialized strategy for exactly this layer, through its view
// (PooledCountedBitTrieMultisetTests, ARCHITECTURE §13.9). A class that derives from this one names
// only its multiset, as a struct type argument, and inherits every fact.
//
// The tables pin hand-derived cases, each row's arithmetic in the comment above it; the seeded
// tests replay one long script of inserts and removals and check every answer against a
// brute-force scan of a plain List<int> holding the same multiset. A row's removals all name
// values its inserts put in, so each one is expected to succeed.
public abstract partial class XorMultisetContractTests<TSet>
    where TSet : struct, IXorMultiset<TSet>
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

    // Inserted, removed first, then a value with a live copy for TryRemove to take: the Count left
    // behind.
    public static TheoryData<int[], int[], int, int> TryRemoveHeldCases =>
        new()
        {
            { [7], [], 7, 0 },

            // A multiset: one copy goes, the other stays live.
            { [7, 7], [], 7, 1 },
        };

    // Inserted, removed first, then a value with no live copy, which TryRemove must refuse.
    public static TheoryData<int[], int[], int> TryRemoveNotHeldCases =>
        new()
        {
            { [], [], 7 },

            // 7 = 0b111 shares all 31 upper bits with 6 = 0b110 and leaves the laid-down path only
            // at bit 0.
            { [6], [], 7 },

            // -6 sets bit 31, so its path leaves 6's at the root.
            { [6], [], -6 },

            // 5's leaf is still laid down but holds no live copy - beside a live sibling, 4.
            { [5], [5], 5 },
            { [4, 5], [5], 5 },
        };

    // Inserted, removed, then a query when nothing is live.
    public static TheoryData<int[], int[], int> TryMaxXorEmptyCases =>
        new()
        {
            { [], [], 5 },

            // Every value removed: the nodes remain, nothing is live.
            { [4, 6], [4, 6], 5 },
        };

    // Inserted, removed, the query: the largest query ^ y over the live values.
    public static TheoryData<int[], int[], int, int> TryMaxXorCases =>
        new()
        {
            // LeetCode 421's example: 5 ^ 3 = 6, 5 ^ 10 = 15, 5 ^ 5 = 0, 5 ^ 25 = 28, 5 ^ 2 = 7,
            // 5 ^ 8 = 13, so 28.
            { [3, 10, 5, 25, 2, 8], [], 5, 28 },

            // 6 ^ 1 = 7, 6 ^ 8 = 14, 6 ^ 14 = 8. With 8 removed its path still branches opposite
            // 6's bit at bit 2, but holds nothing live, so the walk must take 14's: 8.
            { [1, 8, 14], [8], 6, 8 },

            // 0's whole branch is dead from bit 3 down; only 15 ^ 15 = 0 is left.
            { [0, 15], [0], 15, 0 },

            // One copy of 7 removed, the other still live: 0 ^ 7 = 7 beats 0 ^ 1 = 1.
            { [7, 7, 1], [7], 0, 7 },
            { [9], [], 9, 0 },

            // Unsigned reading: 0 ^ -1 is the all-ones pattern, the largest there is, read back as
            // the int -1.
            { [-1, 1], [], 0, -1 },
        };

    // Inserted, value, limit: how many held y have value ^ y below limit, unsigned. A removal's
    // effect on the count is its own fact below.
    public static TheoryData<int[], int, int, int> CountXorBelowCases =>
        new()
        {
            { [], 3, 100, 0 },

            // 3 ^ 3 = 0, which is not below 0.
            { [3, 3], 3, 0, 0 },

            // 3 ^ 3 = 0 < 1 for every copy.
            { [3, 3, 3], 3, 1, 3 },

            // 3 ^ 5 = 6: not strictly below 6, below 7.
            { [5], 3, 6, 0 },
            { [5], 3, 7, 1 },

            // LeetCode 1803's first example, 7 against the earlier [1, 4, 2]: 6, 3 and 5 are all
            // below high + 1 = 7 and none below low = 2.
            { [1, 4, 2], 7, 7, 3 },
            { [1, 4, 2], 7, 2, 0 },

            // int.MinValue is the unsigned 2^31: 0 ^ 0 = 0 and 0 ^ int.MaxValue = 2^31 - 1 are
            // below it, 0 ^ -1 = 2^32 - 1 is not.
            { [0, int.MaxValue, -1], 0, int.MinValue, 2 },

            // -1 is the unsigned 2^32 - 1: everything below it but the all-ones XOR.
            { [0, -1], 0, -1, 1 },
        };

    [Theory]
    [MemberData(nameof(CountCases))]
    public void Count_AfterInsertsAndRemovals_IsTheLiveMultisetSize(
        int[] inserted, int[] removed, int expected)
    {
        var set = Build(inserted, removed);

        Assert.Equal(expected, set.Count);
    }

    [Fact]
    public void Insert_AfterEveryCopyWasRemoved_MakesTheValueLiveAgain()
    {
        var set = Build([Fixtures.ReinsertedValue], [Fixtures.ReinsertedValue]);

        set.Insert(Fixtures.ReinsertedValue);
        var copies = set.CountXorBelow(Fixtures.ReinsertedValue, Fixtures.OnlyAnEqualValue);

        Assert.Equal((Fixtures.OneCopy, Fixtures.OneCopy), (copies, set.Count));
    }

    // Enough distinct values to lay down a few thousand nodes: every value must still be found on
    // its own path, so no link was lost as the storage grew.
    [Fact]
    public void Insert_ManyDistinctValues_KeepsEveryPathAcrossStorageGrowth()
    {
        var set = TSet.Empty();

        for (var value = 0; value < Fixtures.DistinctValueCount; value++)
        {
            set.Insert(value);
        }

        for (var value = 0; value < Fixtures.DistinctValueCount; value++)
        {
            var copies = set.CountXorBelow(value, Fixtures.OnlyAnEqualValue);
            Assert.Equal(Fixtures.OneCopy, copies);
        }
    }

    [Theory]
    [MemberData(nameof(TryRemoveHeldCases))]
    public void TryRemove_LiveCopy_TakesOneCopyOut(
        int[] inserted, int[] removed, int target, int expectedCount)
    {
        var set = Build(inserted, removed);

        var wasRemoved = set.TryRemove(target);

        Assert.Equal((true, expectedCount), (wasRemoved, set.Count));
    }

    [Theory]
    [MemberData(nameof(TryRemoveNotHeldCases))]
    public void TryRemove_NoLiveCopy_IsFalseAndChangesNothing(
        int[] inserted, int[] removed, int target)
    {
        var set = Build(inserted, removed);
        var countBefore = set.Count;

        var wasRemoved = set.TryRemove(target);

        Assert.Equal((false, countBefore), (wasRemoved, set.Count));
    }

    [Fact]
    public void TryRemove_SeededInsertsAndRemovals_AgreesWithAListModel()
    {
        var random = new Random(Fixtures.Seed);
        var set = TSet.Empty();
        var live = new List<int>();

        for (var step = 0; step < Fixtures.ScriptLength; step++)
        {
            ApplySeededStep(set, live, random);
        }

        Assert.Equal(live.Count, set.Count);
    }

    [Theory]
    [MemberData(nameof(TryMaxXorEmptyCases))]
    public void TryMaxXor_NothingLive_IsFalse(int[] inserted, int[] removed, int query)
    {
        var set = Build(inserted, removed);

        var found = set.TryMaxXor(query, out var result);

        Assert.Equal((false, 0), (found, result));
    }

    [Theory]
    [MemberData(nameof(TryMaxXorCases))]
    public void TryMaxXor_Cases_ReturnsTheLargestXorAgainstALiveValue(
        int[] inserted, int[] removed, int query, int expected)
    {
        var set = Build(inserted, removed);

        var found = set.TryMaxXor(query, out var result);

        Assert.Equal((true, expected), (found, result));
    }

    [Fact]
    public void TryMaxXor_SeededInsertsAndRemovals_MatchesABruteForceScanOfTheLiveValues()
    {
        var random = new Random(Fixtures.Seed);
        var set = TSet.Empty();
        var live = new List<int>();

        for (var step = 0; step < Fixtures.ScriptLength; step++)
        {
            ApplySeededStep(set, live, random);
            var query = random.Next(int.MinValue, int.MaxValue);

            var found = set.TryMaxXor(query, out var result);
            var expected = BruteForceMaxXor(live, query);

            Assert.Equal((live.Count > 0, expected), (found, result));
        }
    }

    [Theory]
    [MemberData(nameof(CountXorBelowCases))]
    public void CountXorBelow_Cases_CountsHeldValuesWhoseXorIsStrictlyBelowTheLimit(
        int[] inserted, int value, int limit, int expected)
    {
        var set = Build(inserted, []);

        var count = set.CountXorBelow(value, limit);

        Assert.Equal(expected, count);
    }

    // Removed values are gone: 0 ^ 1 = 1 and 0 ^ 3 = 3 are below 4, and 2 was removed.
    [Fact]
    public void CountXorBelow_AfterARemoval_CountsOnlyLiveValues()
    {
        var set = Build([1, 2, 3], [2]);

        Assert.Equal(2, set.CountXorBelow(0, 4));
    }

    // Each step asks twice: once against a limit drawn from the whole int range, so bit 31 of the
    // limit is set about half the time, and once against a limit inside the stored values' own
    // range, where the XORs mostly fall.
    [Fact]
    public void CountXorBelow_SeededInsertsAndRemovals_MatchesABruteForceScanOfTheLiveValues()
    {
        var random = new Random(Fixtures.Seed);
        var set = TSet.Empty();
        var live = new List<int>();

        for (var step = 0; step < Fixtures.ScriptLength; step++)
        {
            ApplySeededStep(set, live, random);
            var value = random.Next(int.MinValue, int.MaxValue);
            var wideLimit = random.Next(int.MinValue, int.MaxValue);
            var narrowLimit = random.Next(Fixtures.StoredValueLow, Fixtures.StoredValueHigh);

            var wide = set.CountXorBelow(value, wideLimit);
            var narrow = set.CountXorBelow(value, narrowLimit);
            var expectedWide = BruteForceCountXorBelow(live, value, wideLimit);
            var expectedNarrow = BruteForceCountXorBelow(live, value, narrowLimit);

            Assert.Equal((expectedWide, expectedNarrow), (wide, narrow));
        }
    }

    private static TSet Build(int[] inserted, int[] removed)
    {
        var set = TSet.Empty();

        foreach (var value in inserted)
        {
            set.Insert(value);
        }

        foreach (var value in removed)
        {
            var wasRemoved = set.TryRemove(value);
            Assert.True(wasRemoved);
        }

        return set;
    }

    // One step of the script, applied to the multiset and the model alike: an insert or a removal
    // with even odds, of a value from a range narrow enough that duplicates pile up and a removal
    // often names a value absent or already gone. The range straddles zero, so bit 31 differs
    // between stored values too.
    private static void ApplySeededStep(TSet set, List<int> live, Random random)
    {
        var value = random.Next(Fixtures.StoredValueLow, Fixtures.StoredValueHigh);
        var isRemoval = random.Next(Fixtures.StepKinds) == 0;

        if (isRemoval)
        {
            var wasLive = live.Remove(value);
            var wasRemoved = set.TryRemove(value);
            Assert.Equal(wasLive, wasRemoved);
        }
        else
        {
            live.Add(value);
            set.Insert(value);
        }

        Assert.Equal(live.Count, set.Count);
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

        // 2,000 distinct values lay down a few thousand nodes; the pooled trie starts with room for
        // 34, so its storage doubles several times.
        public const int DistinctValueCount = 2_000;

        public const int Seed = 1_803;
        public const int ScriptLength = 2_000;
        public const int StoredValueLow = -16;
        public const int StoredValueHigh = 48;
        public const int StepKinds = 2;
    }
}
