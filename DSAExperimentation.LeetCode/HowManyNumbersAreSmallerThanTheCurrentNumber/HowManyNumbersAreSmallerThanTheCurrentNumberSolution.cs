using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.HowManyNumbersAreSmallerThanTheCurrentNumber;

// LeetCode 1365. How Many Numbers Are Smaller Than the Current Number: for each
// element, how many other elements are strictly smaller than it?
//
// Both strategies answer with the same int[] LeetCode asks for; they differ only
// in how that count is obtained - counting every pair, or sorting once and then
// asking each value where it would be inserted.
internal static class HowManyNumbersAreSmallerThanTheCurrentNumberSolution
{
    // The textbook O(n^2) answer: for every element, walk the whole array again
    // and tally the strictly smaller ones. Deliberately plain BCL - it is the arm
    // the composed strategy below has to justify itself against.
    public static int[] SmallerNumbersThanCurrentByPairwiseCount(int[] nums)
    {
        var result = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            var count = 0;

            for (var j = 0; j < nums.Length; j++)
            {
                if (nums[j] < nums[i])
                {
                    count++;
                }
            }

            result[i] = count;
        }

        return result;
    }

    // Sort a copy with this repo's own MergeSort, then look each original value up
    // with BinarySearch.LowerBound: the leftmost insertion index LowerBound already
    // computes is exactly the count of strictly smaller elements, so the whole
    // problem is one sort plus n bisections - O(n log n).
    public static int[] SmallerNumbersThanCurrentBySortAndLowerBound(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var sequence = new ArraySequence<int>(sorted);
        var result = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            result[i] = BinarySearch.LowerBound<int, ArraySequence<int>>(sequence, nums[i]);
        }

        return result;
    }
}
