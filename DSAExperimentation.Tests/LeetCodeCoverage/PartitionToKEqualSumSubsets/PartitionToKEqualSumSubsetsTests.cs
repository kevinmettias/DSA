using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionToKEqualSumSubsets;

// LeetCode 698. Partition to K Equal Sum Subsets: the same k-bucket backtracking
// shape MatchsticksToSquareTests already uses for LeetCode 473 (which is exactly
// this problem's k=4 special case) - this repo's own Backtrack.TrySearch assigns
// each number, largest first, to one of k running bucket sums. Candidates only
// offers buckets that don't overshoot the target sum, which by construction
// (every bucket <= target, and the total is a multiple of k) forces every bucket
// to land on exactly the target once all numbers are placed.
public sealed partial class PartitionToKEqualSumSubsetsTests
{
    [Fact]
    public void CanPartitionKSubsets_FourEqualSubsetsPossible_ReturnsTrue()
    {
        var actual = CanPartitionKSubsets([4, 3, 2, 3, 5, 2, 1], k: 4);
        Assert.True(actual);
    }

    [Fact]
    public void CanPartitionKSubsets_TotalNotDivisibleByK_ReturnsFalse()
    {
        var actual = CanPartitionKSubsets([1, 2, 3, 4], k: 3);
        Assert.False(actual);
    }

    [Fact]
    public void CanPartitionKSubsets_DivisibleTotalButNoValidSplit_ReturnsFalse()
    {
        var actual = CanPartitionKSubsets([2, 2, 2, 2, 3, 4, 5], k: 4);
        Assert.False(actual);
    }

    private static bool CanPartitionKSubsets(int[] nums, int k)
    {
        var total = nums.Sum();

        if (!IsPartitionable(nums, k, total, out var target))
        {
            return false;
        }

        var sorted = SortDescending(nums);
        var state = new State(sorted, target, k);
        var steps = BuildSteps(sorted.Length, k);

        return Backtrack.TrySearch(state, steps);
    }

    private static bool IsPartitionable(int[] nums, int k, int total, out int target)
    {
        target = total / k;
        return total % k == 0 && nums.Max() <= target;
    }

    private static int[] SortDescending(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        return sorted;
    }

    private static BacktrackingSteps<State, int> BuildSteps(int length, int k) =>
        new(
            IsSolution: s => s.Index == length,
            Candidates: s => s.Index == length ? [] : Enumerable.Range(0, k).Where(s.CanPlace),
            Choose: (s, bucket) => s.Place(bucket),
            Unchoose: (s, bucket) => s.Remove(bucket),
            OnSolution: _ => true);

    private sealed class State(int[] nums, int target, int k)
    {
        private readonly int[] _buckets = new int[k];

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
