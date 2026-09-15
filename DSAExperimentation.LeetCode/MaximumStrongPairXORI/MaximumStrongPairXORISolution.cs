using System.Numerics;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumStrongPairXORI;

// LeetCode 2932. Maximum Strong Pair XOR I: (x, y) is a strong pair when
// |x - y| <= min(x, y); return the maximum x XOR y over every strong pair.
// nums.Length <= 50 and every value <= 100 here, so the textbook O(n^2) pairwise
// scan is already fast enough on its own - but the BitTrie-bucket strategy LC
// 2935 actually needs to clear ITS bound is exactly as correct at this smaller
// one, so both strategies are proven here too, the same way
// DistributeCandiesAmongChildrenI still carries the closed form its own bound
// doesn't strictly require. See MaximumStrongPairXORIISolution for the bucket
// strategy's own reasoning - identical here, just restated at LC 2932's scale.
internal static class MaximumStrongPairXORISolution
{
    public static int MaximumStrongPairXorByBruteForce(int[] nums)
    {
        var best = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (IsStrongPair(nums[i], nums[j]))
                {
                    best = Math.Max(best, nums[i] ^ nums[j]);
                }
            }
        }

        return best;
    }

    private static bool IsStrongPair(int x, int y)
    {
        var (small, large) = x <= y ? SmallestFirst(x, y) : LargestFirst(x, y);
        return large - small <= small;
    }

    // The pair with the smaller value first, or with the larger value first.
    private static (int Small, int Large) SmallestFirst(int x, int y) => (x, y);

    private static (int Small, int Large) LargestFirst(int x, int y) => (y, x);

    public static int MaximumStrongPairXorByBitTrieBuckets(int[] nums)
    {
        var sorted = new ArrayIndexedSequence<int>((int[])nums.Clone());
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(sorted);

        return MaximumStrongPairXorByBitTrieBuckets(sorted);
    }

    // Prepared-input overload: `sortedNums` must already be sorted ascending.
    // Every strong pair sits inside one "highest set bit" bucket or spans two
    // ADJACENT buckets (see MaximumStrongPairXORIISolution's doc comment for the
    // two-lemma argument), so a single ascending pass over sorted buckets, each
    // fed through this repo's own BitTrie (DataStructures/Graph/Engines/Dags/
    // Trees/BitTrie.cs), finds the answer without ever needing to remove an
    // element from a trie.
    public static int MaximumStrongPairXorByBitTrieBuckets(ArrayIndexedSequence<int> sortedNums)
    {
        var best = 0;
        var previousBucketStart = -1;
        var bucketStart = 0;

        while (bucketStart < sortedNums.Length)
        {
            (best, previousBucketStart, bucketStart) =
                ScoreBucket(sortedNums, previousBucketStart, bucketStart, best);
        }

        return best;
    }

    // Measures the bucket starting at `bucketStart` - every pair inside it, plus
    // the pairs it makes with the previous (adjacent) bucket - and reports the
    // best XOR so far alongside where the next bucket begins.
    private static (int Best, int PreviousBucketStart, int BucketStart) ScoreBucket(
        ArrayIndexedSequence<int> nums, int previousBucketStart, int bucketStart, int best)
    {
        var bucketEnd = FindBucketEnd(nums, bucketStart);

        var withinBest = MaxXorWithinBucket(nums, bucketStart, bucketEnd);
        best = Math.Max(best, withinBest);

        if (previousBucketStart >= 0)
        {
            var acrossBest = MaxXorAcrossAdjacentBuckets(
                nums, previousBucketStart, bucketStart, bucketEnd);
            best = Math.Max(best, acrossBest);
        }

        return (best, bucketStart, bucketEnd);
    }

    // The first index past `bucketStart` whose highest set bit differs from it, so
    // the run [bucketStart, result) is exactly one bucket.
    private static int FindBucketEnd(ArrayIndexedSequence<int> nums, int bucketStart)
    {
        var bucketEnd = bucketStart + 1;

        while (bucketEnd < nums.Length &&
               HighestBit(nums.Get(bucketEnd)) == HighestBit(nums.Get(bucketStart)))
        {
            bucketEnd++;
        }

        return bucketEnd;
    }

    private static int HighestBit(int value) => 31 - BitOperations.LeadingZeroCount((uint)value);

    // Every pair within one bucket is automatically a strong pair, so this is
    // exactly LC 421's Maximum XOR of Two Numbers restricted to the bucket - the
    // same insert-then-query-every-element composition
    // MaximumXOROfTwoNumbersInAnArrayTests uses.
    private static int MaxXorWithinBucket(ArrayIndexedSequence<int> nums, int start, int end)
    {
        if (end - start < 2)
        {
            return 0;
        }

        var trie = new BitTrie();

        for (var i = start; i < end; i++)
        {
            trie.Insert(nums.Get(i));
        }

        return MaxXorAgainstTrie(trie, nums, start, end);
    }

    // The best XOR any element of [start, end) can make against the elements
    // already in `trie`.
    private static int MaxXorAgainstTrie(BitTrie trie, ArrayIndexedSequence<int> nums, int start, int end)
    {
        var best = 0;

        for (var i = start; i < end; i++)
        {
            if (trie.TryMaxXor(nums.Get(i), out var candidate))
            {
                best = Math.Max(best, candidate);
            }
        }

        return best;
    }

    // Lower bucket is [lowerStart, upperStart); upper bucket is [upperStart,
    // upperEnd). x only grows walking the lower bucket ascending, so the set of
    // upper-bucket y's with y <= 2x only grows too - the same insert-only offline
    // sweep MaximumXORWithAnElementFromArrayTests uses for LC 1707, just keyed by
    // "2x" instead of a query's own limit, so no BitTrie removal is ever needed.
    private static int MaxXorAcrossAdjacentBuckets(
        ArrayIndexedSequence<int> nums, int lowerStart, int upperStart, int upperEnd)
    {
        var trie = new BitTrie();
        var best = 0;
        var y = upperStart;

        for (var x = lowerStart; x < upperStart; x++)
        {
            var limit = 2L * nums.Get(x);

            while (y < upperEnd && nums.Get(y) <= limit)
            {
                trie.Insert(nums.Get(y));
                y++;
            }

            if (trie.TryMaxXor(nums.Get(x), out var candidate))
            {
                best = Math.Max(best, candidate);
            }
        }

        return best;
    }
}
