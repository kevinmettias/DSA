namespace DSAExperimentation.LeetCode.FindMinimumInRotatedSortedArrayII;

// LeetCode 154. Find Minimum in Rotated Sorted Array II: find the minimum value
// in an ascending array that has been rotated at an unknown pivot and may contain
// duplicates.
//
// Duplicates break the monotone step-function predicate this repo's own
// BinarySearch.LowerBound needs (LC 154's non-duplicate sibling
// FindMinimumInRotatedSortedArraySolution uses nums[i] <= nums[^1] as a clean
// 0/1 step; here nums[mid] == nums[high] leaves which side holds the pivot
// ambiguous), so FindMinByDuplicateTolerantBinaryShrink cannot be expressed over
// that witness the way LC 153/LC 81 are - it shrinks the search window by one on
// a tie instead. Worst case (an all-duplicate array) still degrades to O(n), the
// same well-known result LC 81's SearchInRotatedSortedArrayIISolution documents
// for presence search over the same kind of array.
internal static class FindMinimumInRotatedSortedArrayIISolution
{
    // The textbook O(n) scan - the arm DuplicateTolerantBinaryShrink has to beat.
    public static int FindMinByLinearScan(int[] nums) => nums.Min();

    public static int FindMinByDuplicateTolerantBinaryShrink(int[] nums)
    {
        var low = 0;
        var high = nums.Length - 1;

        while (low < high)
        {
            var mid = low + ((high - low) / 2);

            if (nums[mid] > nums[high])
            {
                low = mid + 1;
            }
            else if (nums[mid] < nums[high])
            {
                high = mid;
            }
            else
            {
                high--;
            }
        }

        return nums[low];
    }
}
