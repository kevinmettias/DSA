using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindMinimumInRotatedSortedArray;

// LeetCode 153. Find Minimum in Rotated Sorted Array: an ascending array with no
// duplicate values (LC 154 relaxes that), rotated at an unknown pivot. Every
// element from the start up to the pivot is >= nums[^1]; every element from the
// pivot on is <= nums[^1] - encoding "is nums[i] <= nums[^1]" as a 0/1 sequence
// makes it monotonic, so this repo's own BinarySearch.LowerBound lands exactly
// on the pivot, which is the minimum. Same proxy-sequence idiom
// PeakIndexInAMountainArray uses for its own unimodal search.
internal static class FindMinimumInRotatedSortedArraySolution
{
    // The textbook answer: walk the whole array. O(n), no repo primitive.
    public static int FindMinByLinearScan(int[] nums)
    {
        var min = nums[0];

        for (var i = 1; i < nums.Length; i++)
        {
            min = Math.Min(min, nums[i]);
        }

        return min;
    }

    // O(log n): BinarySearch.LowerBound over the pivot proxy sequence.
    public static int FindMinByPivotLowerBound(int[] nums)
    {
        var pivot = BinarySearch.LowerBound<int, PivotSequence>(new PivotSequence(nums), 1);

        return nums[pivot];
    }

    // Get(index) is 1 exactly when nums[index] is on the low side of the
    // rotation (at or below the last element) - the pivot itself is the first
    // such index, and the no-duplicates precondition is what keeps this
    // sequence monotonic, the same unenforced-sortedness shape
    // BinarySearch.LowerBound already leans on.
    private readonly struct PivotSequence(int[] nums) : IRandomAccessSequence<int>
    {
        public int Length => nums.Length;

        public int Get(int index) => nums[index] <= nums[^1] ? 1 : 0;
    }
}
