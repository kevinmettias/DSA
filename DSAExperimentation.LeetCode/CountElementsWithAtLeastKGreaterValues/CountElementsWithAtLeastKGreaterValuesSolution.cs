using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.CountElementsWithAtLeastKGreaterValues;

// LeetCode 3759. Count Elements With at Least K Greater Values: an element
// qualifies when at least k other elements in nums are strictly greater than
// it. Return how many elements qualify.
//
// Sorting turns "how many elements are strictly greater than v" into one
// bisection: once nums is sorted, every index at or past v's own run
// (BinarySearch.UpperBound) holds a strictly greater value, so the count is
// just n minus that insertion point.
internal static class CountElementsWithAtLeastKGreaterValuesSolution
{
    // The textbook answer: compare every element against every other one.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int CountQualifiedByBruteForce(int[] nums, int k)
    {
        var qualified = 0;

        foreach (var value in nums)
        {
            var greaterCount = 0;

            foreach (var other in nums)
            {
                if (other > value)
                {
                    greaterCount++;
                }
            }

            if (greaterCount >= k)
            {
                qualified++;
            }
        }

        return qualified;
    }

    // MergeSort.Sort puts nums in ascending order, then BinarySearch.UpperBound
    // locates the first index past each value's own run - everything from
    // there to the end is strictly greater, so its distance from the end is
    // exactly the "greater than me" count LC 3759 asks for.
    public static int CountQualifiedBySortedUpperBound(int[] nums, int k)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        return CountQualifiedBySortedUpperBound(new ArraySequence<int>(sorted), k);
    }

    public static int CountQualifiedBySortedUpperBound(ArraySequence<int> sortedNums, int k)
    {
        var n = sortedNums.Length;
        var qualified = 0;

        for (var i = 0; i < n; i++)
        {
            var greaterCount = n - BinarySearch.UpperBound(sortedNums, sortedNums.Get(i));

            if (greaterCount >= k)
            {
                qualified++;
            }
        }

        return qualified;
    }
}
