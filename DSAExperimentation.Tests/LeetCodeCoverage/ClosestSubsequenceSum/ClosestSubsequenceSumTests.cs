using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClosestSubsequenceSum;

// LeetCode 1755. Closest Subsequence Sum: meet-in-the-middle over an input
// too large for direct 2^n subset enumeration. Backtrack.Search (the Subsets
// precedent) enumerates every subset sum of each half separately - 2*2^(n/2)
// work instead of 2^n - MergeSort orders the second half's sums over an
// ArrayIndexedSequence, and BinarySearch.LowerBound locates each first
// half's closest partner sum on the sorted side.
public sealed partial class ClosestSubsequenceSumTests
{
    [Theory]
    [InlineData(new[] { 5, -7, 3, 5 }, 6, 0)]
    [InlineData(new[] { 7, -9, 15, -2 }, -5, 1)]
    [InlineData(new[] { 1, 2, 3 }, -7, 7)]
    public void MinAbsDifference_LeetCodeExamples_ReturnsClosestAchievableSum(int[] nums, int goal, int expected)
    {
        var actual = MinAbsDifference(nums, goal);
        Assert.Equal(expected, actual);
    }

    private static int MinAbsDifference(int[] nums, int goal)
    {
        var mid = nums.Length / 2;
        var leftSums = SubsetSums(nums[..mid]);
        var rightSums = SubsetSums(nums[mid..]).ToArray();
        var sequence = BuildSortedRightSequence(rightSums);

        return FindClosestAchievableSum(leftSums, rightSums, sequence, goal);
    }

    private static ArraySequence<int> BuildSortedRightSequence(int[] rightSums)
    {
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(rightSums));
        return new ArraySequence<int>(rightSums);
    }

    private static int FindClosestAchievableSum(List<int> leftSums, int[] rightSums, ArraySequence<int> sequence, int goal)
    {
        var best = int.MaxValue;

        foreach (var leftSum in leftSums)
        {
            var index = BinarySearch.LowerBound<int, ArraySequence<int>>(sequence, goal - leftSum);

            if (index < rightSums.Length)
            {
                best = Math.Min(best, Math.Abs(leftSum + rightSums[index] - goal));
            }

            if (index > 0)
            {
                best = Math.Min(best, Math.Abs(leftSum + rightSums[index - 1] - goal));
            }
        }

        return best;
    }

    private static List<int> SubsetSums(int[] part)
    {
        var sums = new List<int>();
        var state = new SumState();

        Backtrack.Search<SumState, int>(
            state,
            isSolution: s => s.NextIndex == part.Length,
            candidates: s => s.NextIndex < part.Length ? new[] { 0, 1 } : Array.Empty<int>(),
            choose: (s, take) => Choose(s, take, part),
            unchoose: (s, take) => Unchoose(s, take, part),
            onSolution: s => sums.Add(s.Sum));

        return sums;
    }

    private static void Choose(SumState state, int take, int[] part)
    {
        if (take == 1)
        {
            state.Sum += part[state.NextIndex];
        }

        state.NextIndex++;
    }

    private static void Unchoose(SumState state, int take, int[] part)
    {
        state.NextIndex--;

        if (take == 1)
        {
            state.Sum -= part[state.NextIndex];
        }
    }

    private sealed class SumState
    {
        public int Sum { get; set; }
        public int NextIndex { get; set; }
    }
}
