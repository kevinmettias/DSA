using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimumDeletionsToMakeArrayDivisible;

// LeetCode 2344. Minimum Deletions to Make Array Divisible: delete as few elements from nums
// as possible so that the smallest remaining element divides every entry of numsDivide, or
// report -1 when no element ever will.
//
// Both strategies first reduce numsDivide to a single divisor with the Euclidean algorithm -
// an element divides every entry exactly when it divides their gcd - and then differ only in
// how they find the cheapest surviving candidate. Deleting to make `v` the minimum costs
// exactly the number of elements strictly smaller than `v`, so the two formulations agree:
// scan every valid candidate for the smallest such count, or sort once and take the position
// of the first value that divides.
internal static class MinimumDeletionsToMakeArrayDivisibleSolution
{
    // The textbook O(n^2) approach: for each element that divides the gcd, count how many
    // elements are strictly smaller than it - that is what deleting down to it costs - and
    // keep the cheapest. Pure BCL loops, no sort: the baseline the sorted strategy is
    // measured against.
    public static int MinDeletionsByCandidateScan(int[] nums, int[] numsDivide)
    {
        var divisor = GcdOfArray(numsDivide);
        var best = LeetCodeAnswer.None;

        foreach (var candidate in nums)
        {
            if (divisor % candidate != 0)
            {
                continue;
            }

            var precedingCount = CountSmaller(nums, candidate);
            if (best == LeetCodeAnswer.None || precedingCount < best)
            {
                best = precedingCount;
            }
        }

        return best;
    }

    private static int CountSmaller(int[] nums, int candidate)
    {
        var count = 0;

        foreach (var value in nums)
        {
            if (value < candidate)
            {
                count++;
            }
        }

        return count;
    }

    // Sort nums once with this repo's own MergeSort over ArrayIndexedSequence<int>, then walk
    // it for the first value that divides the gcd - every element before it has to go, and
    // nothing after it is cheaper. O(n log n) plus one scan.
    public static int MinDeletionsByMergeSort(int[] nums, int[] numsDivide)
    {
        var divisor = GcdOfArray(numsDivide);
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        for (var i = 0; i < sorted.Length; i++)
        {
            if (divisor % sorted[i] == 0)
            {
                return i;
            }
        }

        return LeetCodeAnswer.None;
    }

    // An element divides every entry of numsDivide exactly when it divides their gcd, so the
    // whole array collapses to one number first. A two-integer Euclidean reduction has no
    // repo primitive to compose - the same precedent FindGreatestCommonDivisorOfArray and
    // CheckIfItIsAGoodArray already set.
    private static int GcdOfArray(int[] values)
    {
        var divisor = values[0];

        foreach (var value in values)
        {
            divisor = Gcd(divisor, value);
        }

        return divisor;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
