using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.BinarySearchAlgorithm;

// LeetCode 704. Binary Search: the index of target in a sorted array, or -1 when
// it is absent. (Folder/namespace named "...BinarySearchAlgorithm" rather than the
// bare title, because a sibling namespace literally named "BinarySearch" would
// shadow the production DSAExperimentation.Algorithms.Searching.BinarySearch type
// for every other LeetCode solution class that imports it unqualified.)
//
// The two strategies differ in how much of the array they actually inspect: an
// O(n) linear scan, or this repo's own O(log n) BinarySearch.Find over an
// ArraySequence<int>, whose null ("not found") this class translates to LeetCode's
// -1 convention at the call site.
internal static class BinarySearchAlgorithmSolution
{
    // The textbook baseline this composition has to justify itself against: a
    // plain left-to-right scan. Deliberately written without this repo's
    // primitives.
    public static int FindIndexByLinearScan(int[] nums, int target)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            if (nums[i] == target)
            {
                return i;
            }
        }

        return LeetCodeAnswer.None;
    }

    public static int FindIndexByBinarySearch(int[] nums, int target) =>
        BinarySearch.Find(new ArraySequence<int>(nums), target) ?? LeetCodeAnswer.None;
}
