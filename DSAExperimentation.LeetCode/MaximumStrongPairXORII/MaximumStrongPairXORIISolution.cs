using System.Numerics;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumStrongPairXORII;

// LeetCode 2935. Maximum Strong Pair XOR II: the same question as LC 2932
// ((x, y) is a strong pair when |x - y| <= min(x, y); return the maximum x XOR y
// over every strong pair), at a scale (n up to 5*10^4, values up to 2^20-1)
// where the O(n^2) pairwise scan is too slow to be the intended solution - it
// stays here as the baseline the trie strategy has to beat.
//
// The trie strategy rests on two facts about a strong pair (x, y), x <= y, once
// nums is sorted and grouped by highest set bit ("bucket" k = values v with
// 2^k <= v < 2^(k+1)):
//   1. If x and y share a bucket, they are ALWAYS a strong pair:
//      y < 2^(k+1) <= 2 * 2^k <= 2x.
//   2. If x and y's buckets differ by >= 2 (x < 2^(k+1) <= 2^(m-1), y in bucket
//      m >= k + 2), they can NEVER be a strong pair: 2x < 2^m <= y.
// So every strong pair sits inside one bucket, or spans two ADJACENT buckets.
// Sorting turns each bucket into one contiguous slice:
//   - Within a bucket, every pair already qualifies, so that slice is exactly
//     LC 421's Maximum XOR of Two Numbers - insert the whole slice into one
//     BitTrie (DataStructures/Graph/Engines/Dags/Trees/BitTrie.cs) and query
//     every element against it, the same composition
//     MaximumXOROfTwoNumbersInAnArrayTests uses.
//   - Between adjacent buckets, only y <= 2x qualifies; because x only grows
//     walking the lower bucket left-to-right, the set of eligible y's in the
//     upper bucket only grows too (never shrinks), so a second BitTrie can be
//     filled by a monotonic two-pointer sweep - insert-only, the same
//     offline-sweep shape MaximumXORWithAnElementFromArrayTests uses for LC
//     1707, just keyed by "2x" instead of a query's own limit.
// Insert-only is why this composes with BitTrie as it stands: BitTrie has no
// delete, so a single shared trie over a window that must also shrink (the more
// obvious "sort + two-pointer, removing nums[left] as it falls out of range"
// shape) is not something this repo's BitTrie can do.
internal static class MaximumStrongPairXORIISolution
{
    // The textbook O(n^2) scan of every unordered pair. Correct at any scale,
    // and the arm the BitTrie-bucket strategy below has to beat once n reaches
    // LC 2935's own bound.
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

    private static bool IsStrongPair(int firstValue, int secondValue)
    {
        var small = Math.Min(firstValue, secondValue);
        var large = Math.Max(firstValue, secondValue);

        return large - small <= small;
    }

    public static int MaximumStrongPairXorByBitTrieBuckets(int[] nums)
    {
        var sorted = new ArrayIndexedSequence<int>((int[])nums.Clone());
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(sorted);

        return MaximumStrongPairXorByBitTrieBuckets(sorted);
    }

    // Prepared-input overload: `sortedNums` must already be sorted ascending -
    // the benchmark's own [GlobalSetup] calls MergeSort once so this overload
    // measures only the bucket sweep itself, ArrayIndexedSequence<int> (not
    // int[] or IEnumerable<int>) is what keeps it from ever being the overload
    // an unsorted LeetCode-shaped call silently binds to.
    public static int MaximumStrongPairXorByBitTrieBuckets(ArrayIndexedSequence<int> sortedNums)
    {
        var best = 0;
        var previousBucketStart = -1;
        var bucketStart = 0;

        while (bucketStart < sortedNums.Length)
        {
            (best, previousBucketStart, bucketStart) = SweepBucket(
                sortedNums, best, previousBucketStart, bucketStart);
        }

        return best;
    }

    // Advances past one bucket: scores the XORs it holds within itself, scores the
    // ones it holds against the bucket just before it, then hands back the three
    // threaded locals so the sweep can step on to the next bucket.
    private static (int Best, int PreviousBucketStart, int BucketStart) SweepBucket(
        ArrayIndexedSequence<int> sortedNums, int best, int previousBucketStart, int bucketStart)
    {
        var bucketEnd = bucketStart + 1;

        while (bucketEnd < sortedNums.Length &&
               HighestBit(sortedNums.Get(bucketEnd)) == HighestBit(sortedNums.Get(bucketStart)))
        {
            bucketEnd++;
        }

        var withinBucket = MaxXorWithinBucket(sortedNums, bucketStart, bucketEnd);

        best = Math.Max(best, withinBucket);

        if (previousBucketStart >= 0)
        {
            var acrossAdjacent = MaxXorAcrossAdjacentBuckets(
                sortedNums, previousBucketStart, bucketStart, bucketEnd);

            best = Math.Max(best, acrossAdjacent);
        }

        return (best, bucketStart, bucketEnd);
    }

    private static int HighestBit(int value) => 31 - BitOperations.LeadingZeroCount((uint)value);

    private static int MaxXorWithinBucket(ArrayIndexedSequence<int> nums, int start, int end)
    {
        if (end - start < 2)
        {
            return 0;
        }

        var trie = BuildBucketTrie(nums, start, end);

        return MaxXorAgainstTrie(nums, start, end, trie);
    }

    // The bucket's own slice inserted into one BitTrie: within a bucket every pair is
    // already a strong pair, so this is exactly LC 421's problem.
    private static BitTrie BuildBucketTrie(ArrayIndexedSequence<int> nums, int start, int end)
    {
        var trie = new BitTrie();

        for (var i = start; i < end; i++)
        {
            trie.Insert(nums.Get(i));
        }

        return trie;
    }

    // The best XOR any value of [start, end) can make against the trie.
    private static int MaxXorAgainstTrie(
        ArrayIndexedSequence<int> nums, int start, int end, BitTrie trie)
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
    // upperEnd).
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
