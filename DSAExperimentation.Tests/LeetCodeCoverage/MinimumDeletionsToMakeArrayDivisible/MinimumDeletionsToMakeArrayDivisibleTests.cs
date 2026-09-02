using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumDeletionsToMakeArrayDivisible;

// LeetCode 2344. Minimum Deletions to Make Array Divisible: reduce numsDivide to a
// single divisor via the Euclidean algorithm (the same inline Gcd helper
// FindGreatestCommonDivisorOfArrayTests/CheckIfItIsAGoodArrayTests already reuse - no
// repo container or algorithm primitive applies to a two-integer reduction), sort nums
// with this repo's own Algorithms.Sorting.MergeSort over an ArrayIndexedSequence<int>,
// then scan the sorted array for the first value that evenly divides the divisor - every
// smaller value must be deleted, since the smallest surviving element is what has to
// divide every numsDivide entry.
public sealed partial class MinimumDeletionsToMakeArrayDivisibleTests
{
    [Fact]
    public void MinDeletion_LeetCodeExampleOne_ReturnsDeletionsBeforeFirstValidDivisor()
    {
        int[] nums = [2, 3, 2, 4, 3];
        int[] numsDivide = [9, 6, 9, 3, 15];

        var deletions = MinDeletion(nums, numsDivide);

        Assert.Equal(2, deletions);
    }

    [Fact]
    public void MinDeletion_LeetCodeExampleTwo_ReturnsNegativeOneWhenNoElementDivides()
    {
        int[] nums = [4, 3, 6];
        int[] numsDivide = [8, 2, 6, 10];

        var deletions = MinDeletion(nums, numsDivide);

        Assert.Equal(-1, deletions);
    }

    [Fact]
    public void MinDeletion_EveryElementAlreadyDivides_ReturnsZero()
    {
        int[] nums = [4, 2, 8];
        int[] numsDivide = [16, 8, 32];

        var deletions = MinDeletion(nums, numsDivide);

        Assert.Equal(0, deletions);
    }

    private static int MinDeletion(int[] nums, int[] numsDivide)
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

        return -1;
    }

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
