using System.Numerics;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.Solutions.MaximumStrongPairXORII;

// LC 2935's bucket strategy, written once over any maximum-XOR set (IMaxXorSet): sort, group the
// values by highest set bit, score each bucket against itself and against the bucket before it
// (MaximumStrongPairXORIISolution states the two facts that make that enough). The set is the
// class's struct type argument, so the strategy is compiled for each set with direct calls and no
// interface call. LC 2935 runs it over the one trie's BitTrie (BitTrieSet); LC 2932, whose buckets
// hold a handful of values, over TwoFieldBitTrie, the hand-specialized set, which inlines whole
// into the sweep where the one trie's layered walk runs out of the JIT's inlining budget
// (ARCHITECTURE 13.9). It is a class of its own rather than methods of the solution, because the
// catalog runs every static method of a solution class as one strategy, and a method over any set
// is none.
internal static class StrongPairBuckets<TSet>
    where TSet : struct, IMaxXorSet<TSet>
{
    // The bucket strategy over any maximum-XOR set, sorting a copy of nums first.
    public static int MaxXor(int[] nums)
    {
        var sorted = new ArrayIndexedSequence<int>((int[])nums.Clone());
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(sorted);

        return MaxXor(sorted);
    }

    // The bucket strategy over any maximum-XOR set, `sortedNums` sorted ascending.
    public static int MaxXor(ArrayIndexedSequence<int> sortedNums)
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

    // Advances past one bucket: scores the XORs it holds within itself, scores the ones it holds
    // against the bucket just before it, then hands back the three threaded locals so the sweep can
    // step on to the next bucket.
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

        var set = BuildBucketSet(nums, start, end);

        return MaxXorAgainstSet(nums, start, end, set);
    }

    // The bucket's own slice inserted into one set: within a bucket every pair is already a strong
    // pair, so this is exactly LC 421's problem.
    private static TSet BuildBucketSet(ArrayIndexedSequence<int> nums, int start, int end)
    {
        var set = TSet.Empty();

        for (var i = start; i < end; i++)
        {
            set.Insert(nums.Get(i));
        }

        return set;
    }

    // The best XOR any value of [start, end) can make against the set.
    private static int MaxXorAgainstSet(
        ArrayIndexedSequence<int> nums, int start, int end, TSet set)
    {
        var best = 0;

        for (var i = start; i < end; i++)
        {
            if (set.TryMaxXor(nums.Get(i), out var candidate))
            {
                best = Math.Max(best, candidate);
            }
        }

        return best;
    }

    // Lower bucket is [lowerStart, upperStart); upper bucket is [upperStart, upperEnd).
    private static int MaxXorAcrossAdjacentBuckets(
        ArrayIndexedSequence<int> nums, int lowerStart, int upperStart, int upperEnd)
    {
        var set = TSet.Empty();
        var best = 0;
        var y = upperStart;

        for (var x = lowerStart; x < upperStart; x++)
        {
            var limit = 2L * nums.Get(x);

            while (y < upperEnd && nums.Get(y) <= limit)
            {
                set.Insert(nums.Get(y));
                y++;
            }

            if (set.TryMaxXor(nums.Get(x), out var candidate))
            {
                best = Math.Max(best, candidate);
            }
        }

        return best;
    }
}
