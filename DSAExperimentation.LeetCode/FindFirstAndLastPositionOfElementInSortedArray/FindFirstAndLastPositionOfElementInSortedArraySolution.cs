using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindFirstAndLastPositionOfElementInSortedArray;

// LeetCode 34. Find First and Last Position of Element in Sorted Array: report the
// closed [first, last] index range a target occupies in a sorted array, or [-1, -1]
// when it is absent.
//
// Both strategies answer the same question - where does target's run start and end -
// they differ only in how they locate the run's edges.
internal static class FindFirstAndLastPositionOfElementInSortedArraySolution
{
    // The textbook answer: BCL Array.IndexOf/LastIndexOf scan from each end toward the
    // middle. Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int[] SearchRangeByLinearScan(int[] nums, int target)
    {
        var first = Array.IndexOf(nums, target);

        return first < 0
            ? [LeetCodeAnswer.None, LeetCodeAnswer.None]
            : [first, Array.LastIndexOf(nums, target)];
    }

    // This repo's own LowerBound/UpperBound already delimit exactly the run of indices
    // equal to target ([LowerBound, UpperBound)), so the puzzle reduces to one bisection
    // pair and a length check.
    public static int[] SearchRangeByBinarySearchBounds(int[] nums, int target)
    {
        var sequence = new ArraySequence<int>(nums);
        var lower = BinarySearch.LowerBound(sequence, target);
        var upper = BinarySearch.UpperBound(sequence, target);

        return lower == upper
            ? [LeetCodeAnswer.None, LeetCodeAnswer.None]
            : [lower, upper - 1];
    }
}
