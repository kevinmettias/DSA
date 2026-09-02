namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOne;

// LeetCode 2654. Minimum Number of Operations to Make All Array Elements Equal
// to 1: if a 1 is already present, every other element needs exactly one
// gcd-with-a-1-neighbor operation (n - ones). Otherwise, Bezout's identity says
// the shortest contiguous window whose own gcd is 1 can be collapsed to a single
// 1 in (windowLength - 1) operations, which is then spread to the rest of the
// array in (n - 1) more operations - or -1 if no window's gcd is ever 1 (the
// whole array's gcd isn't 1 either). Uses the same private Euclidean Gcd fold
// this repo already reuses inline across CheckIfItIsAGoodArrayTests/
// FindGreatestCommonDivisorOfArrayTests/ReplaceNonCoprimeNumbersInArrayTests
// ("no repo container or algorithm primitive applies here" - a running integer
// fold has nothing to compose over), here driving an O(n^2) scan for the minimum
// such window instead of a single running fold over the whole array.
public sealed partial class MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneTests
{
    [Fact]
    public void MinOperations_NoInitialOne_ReturnsShortestWindowPlusSpreadCost()
    {
        int[] nums = [2, 6, 3, 4];

        Assert.Equal(4, MinOperations(nums));
    }

    [Fact]
    public void MinOperations_OverallGcdNotOne_ReturnsNegativeOne()
    {
        int[] nums = [2, 10, 6, 14];

        Assert.Equal(-1, MinOperations(nums));
    }

    [Fact]
    public void MinOperations_AlreadyContainsOnes_ReturnsCountOfNonOnes()
    {
        int[] nums = [1, 1, 1];

        Assert.Equal(0, MinOperations(nums));
    }

    private static int MinOperations(int[] nums)
    {
        var n = nums.Length;
        var ones = nums.Count(value => value == 1);

        if (ones > 0)
        {
            return n - ones;
        }

        var minWindow = FindShortestWindowWithGcdOne(nums);

        return minWindow == -1 ? -1 : (minWindow - 1) + (n - 1);
    }

    private static int FindShortestWindowWithGcdOne(int[] nums)
    {
        var best = -1;

        for (var start = 0; start < nums.Length; start++)
        {
            var gcd = nums[start];

            for (var end = start + 1; end < nums.Length; end++)
            {
                gcd = Gcd(gcd, nums[end]);

                if (gcd == 1)
                {
                    var length = end - start + 1;
                    best = best == -1 ? length : Math.Min(best, length);
                    break;
                }
            }
        }

        return best;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
