using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.PartitionToKEqualSumSubsets;

// LeetCode 698. Partition to K Equal Sum Subsets: can every number be assigned to
// one of k buckets so all k buckets end up with the same sum?
//
// The four-bucket generalization of LeetCode 473 (MatchsticksToSquare) - both
// strategies assign numbers, largest first, to one of the bucketCount running
// bucket sums, backtracking whenever a placement would overshoot the target bucket
// sum. Candidates only offers buckets that don't overshoot, which by construction
// (every bucket <= target, and the total is a multiple of bucketCount) forces every
// bucket to land on exactly the target once all numbers are placed.
internal static class PartitionToKEqualSumSubsetsSolution
{
    // The textbook recursion: a hand-written DFS over bucketCount running bucket sums,
    // undoing a placement on backtrack. Deliberately written without this repo's
    // Backtrack primitive - it is the arm the composed solution below has to
    // justify itself against.
    public static bool CanPartitionKSubsetsByNaiveBacktracking(int[] nums, int bucketCount)
    {
        if (!TryComputeTarget(nums, bucketCount, out var target))
        {
            return false;
        }

        var sorted = SortedDescending(nums);
        var buckets = new int[bucketCount];

        return TrySearchByNaiveBacktracking(sorted, (buckets, target), 0);
    }

    // This repo's own Backtrack.TrySearch (the NQueens/SudokuSolver/
    // MatchsticksToSquare precedent), closed over the same choose/explore/
    // unchoose steps TrySearchByNaiveBacktracking writes out by hand.
    public static bool CanPartitionKSubsetsByGenericBacktrack(int[] nums, int bucketCount)
    {
        if (!TryComputeTarget(nums, bucketCount, out var target))
        {
            return false;
        }

        var sorted = SortedDescending(nums);
        var state = new BucketState(sorted, target, bucketCount);

        return Backtrack.TrySearch<BucketState, int>(state, new BacktrackingSteps<BucketState, int>(
            IsSolution: s => s.Index == sorted.Length,
            Candidates: s =>
                s.Index == sorted.Length ? Array.Empty<int>() : AvailableBuckets(s, bucketCount),
            Choose: (s, bucket) => s.Place(bucket),
            Unchoose: (s, bucket) => s.Remove(bucket),
            OnSolution: _ => true));
    }

    // The candidate buckets for the next number: every bucket it still fits into
    // without overshooting the target sum, offered in bucket order.
    private static IEnumerable<int> AvailableBuckets(BucketState state, int bucketCount)
        => Enumerable.Range(0, bucketCount).Where(state.CanPlace);

    // The running bucket sums and the target each of them has to land on are one
    // thing - the bucket state the search is filling - and how many buckets there
    // are is read off the array rather than threaded alongside it.
    private static bool TrySearchByNaiveBacktracking(
        int[] sorted, (int[] Buckets, int Target) state, int index)
    {
        if (index == sorted.Length)
        {
            return true;
        }

        for (var bucket = 0; bucket < state.Buckets.Length; bucket++)
        {
            if (TryPlaceInBucket(sorted, state, bucket, index))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryPlaceInBucket(int[] sorted, (int[] Buckets, int Target) state, int bucket, int index)
    {
        if (state.Buckets[bucket] + sorted[index] > state.Target)
        {
            return false;
        }

        state.Buckets[bucket] += sorted[index];

        if (TrySearchByNaiveBacktracking(sorted, state, index + 1))
        {
            return true;
        }

        state.Buckets[bucket] -= sorted[index];
        return false;
    }

    private static bool TryComputeTarget(int[] nums, int bucketCount, out int target)
    {
        target = 0;

        if (bucketCount <= 0 || nums.Length < bucketCount)
        {
            return false;
        }

        var total = nums.Sum();

        if (total % bucketCount != 0)
        {
            return false;
        }

        target = total / bucketCount;
        return nums.Max() <= target;
    }

    private static int[] SortedDescending(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        return sorted;
    }

    private sealed class BucketState(int[] nums, int target, int bucketCount)
    {
        private readonly int[] _buckets = new int[bucketCount];

        public int Index { get; private set; }

        public bool CanPlace(int bucket) => _buckets[bucket] + nums[Index] <= target;

        public void Place(int bucket)
        {
            _buckets[bucket] += nums[Index];
            Index++;
        }

        public void Remove(int bucket)
        {
            Index--;
            _buckets[bucket] -= nums[Index];
        }
    }
}
