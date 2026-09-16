using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ClosestSubsequenceSum;

// LeetCode 1755. Closest Subsequence Sum: over every subsequence of nums (the empty
// one included), the smallest |sum - goal|.
//
// Both strategies enumerate subset sums; they differ in how many they have to touch:
//
// - MinAbsoluteDifferenceByBruteForceSubsets walks all 2^n subset masks of the whole
//   array. This is the "what you would write without this repo" arm, deliberately
//   plain BCL bit twiddling - it was previously only the benchmark's unasserted
//   baseline, and its 1 << n mask range is what limits it to small n.
// - MinAbsoluteDifferenceByMeetInTheMiddle splits the array in half and enumerates each
//   half's 2^(n/2) subset sums with Backtrack.Search (the Subsets precedent), sorts
//   one half with MergeSort over an ArrayIndexedSequence, and pairs each sum from the
//   other half with its closest partner via BinarySearch.LowerBound - 2*2^(n/2) work
//   instead of 2^n.
//
// Neither strategy short-circuits on an exact match, so both always scan to
// completion regardless of goal.
internal static class ClosestSubsequenceSumSolution
{
    private const int Taken = 1;

    public static int MinAbsoluteDifferenceByBruteForceSubsets(int[] nums, int goal)
    {
        var best = int.MaxValue;

        for (var mask = 0; mask < (1 << nums.Length); mask++)
        {
            best = Math.Min(best, Math.Abs(SumOfMask(nums, mask) - goal));
        }

        return best;
    }

    private static int SumOfMask(int[] nums, int mask)
    {
        var sum = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                sum += nums[i];
            }
        }

        return sum;
    }

    public static int MinAbsoluteDifferenceByMeetInTheMiddle(int[] nums, int goal)
    {
        var mid = nums.Length / AlgorithmConstants.HalvingFactor;
        var leftSums = SubsetSums(nums[..mid]);
        var rightSums = SubsetSums(nums[mid..]).ToArray();
        var sequence = SortAndWrap(rightSums);

        return FindClosestSumToGoal(leftSums, rightSums, sequence, goal);
    }

    // Every subset sum of one half, as a take/skip decision per position: the choice
    // set is SkipOrTake and the state carries the running sum, so Unchoose only has
    // to subtract back the element Choose added.
    private static List<int> SubsetSums(int[] part)
    {
        var sums = new List<int>();
        var state = new SumState();

        Backtrack.Search<SumState, int>(
            state,
            isSolution: s => s.NextIndex == part.Length,
            candidates: s => s.NextIndex < part.Length ? SkipOrTake() : Array.Empty<int>(),
            choose: (s, take) => ChooseSubsetElement(s, take, part),
            unchoose: (s, take) => UnchooseSubsetElement(s, take, part),
            onSolution: s => sums.Add(s.Sum));

        return sums;
    }

    // The two choices open at every position: leave the element out of the subset, or
    // take it in.
    private static int[] SkipOrTake() => [0, Taken];

    private static void ChooseSubsetElement(SumState state, int take, int[] part)
    {
        if (take == Taken)
        {
            state.Sum += part[state.NextIndex];
        }

        state.NextIndex++;
    }

    private static void UnchooseSubsetElement(SumState state, int take, int[] part)
    {
        state.NextIndex--;

        if (take == Taken)
        {
            state.Sum -= part[state.NextIndex];
        }
    }

    private static ArraySequence<int> SortAndWrap(int[] rightSums)
    {
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(rightSums));
        return new ArraySequence<int>(rightSums);
    }

    // LowerBound lands on the first right-hand sum that is not below the shortfall
    // goal - leftSum; the closest partner is that one or the one just before it, so
    // both are priced.
    private static int FindClosestSumToGoal(
        List<int> leftSums,
        int[] rightSums,
        ArraySequence<int> sequence,
        int goal)
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

    private sealed class SumState
    {
        public int Sum { get; set; }
        public int NextIndex { get; set; }
    }
}
