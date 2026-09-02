using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SearchInsertPosition;

// LeetCode 35. Search Insert Position: the index target occupies, or the index it
// would be inserted at to keep nums sorted.
//
// Both strategies answer the same "first index whose element is not less than
// target" question - they differ only in how they find it.
internal static class SearchInsertPositionSolution
{
    // The textbook answer: a BCL linear scan for the first element not less than
    // target, falling off the end when every element is smaller. Deliberately
    // written without this repo's primitives - it is the arm the composed solution
    // below has to justify itself against.
    public static int SearchInsertByLinearScan(int[] nums, int target)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            if (nums[i] >= target)
            {
                return i;
            }
        }

        return nums.Length;
    }

    // LowerBound's own definition - "the leftmost index at which target could be
    // inserted without disturbing sort order" - is LeetCode's answer verbatim.
    public static int SearchInsertByBinarySearchLowerBound(int[] nums, int target) =>
        BinarySearch.LowerBound(new ArraySequence<int>(nums), target);
}
