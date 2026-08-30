namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfItIsAGoodArray;

// LeetCode 1250. Check If It Is a Good Array: Bezout's identity says integers
// x_1..x_n admit an integer-coefficient combination summing to 1 iff
// gcd(x_1,...,x_n) = 1 - so the whole problem reduces to one running Gcd fold
// over the array with an early exit once it hits 1. No repo container or
// algorithm primitive applies here - reducing an array to one number via
// repeated pairwise Gcd is a pure fold over a private helper, the same
// "nothing to compose over a running integer" case ReachingPointsTests and
// this repo's own Pow(x, n) already are, and the same private Euclidean Gcd
// helper WaterAndJugProblemBenchmarks/NthMagicalNumberTests/
// XOfAKindInADeckOfCardsTests already reuse inline rather than promoting to a
// shared production type.
public sealed partial class CheckIfItIsAGoodArrayTests
{
    [Fact]
    public void IsGoodArray_TwoCoprimeValuesPresent_ReturnsTrue()
    {
        int[] nums = [12, 5, 7, 23];

        Assert.True(IsGoodArray(nums));
    }

    [Fact]
    public void IsGoodArray_ContainsAPrimeCoprimeWithTheRest_ReturnsTrue()
    {
        int[] nums = [29, 6, 10];

        Assert.True(IsGoodArray(nums));
    }

    [Fact]
    public void IsGoodArray_AllValuesShareACommonFactor_ReturnsFalse()
    {
        int[] nums = [3, 6];

        Assert.False(IsGoodArray(nums));
    }

    [Fact]
    public void IsGoodArray_SingleValueOfOne_ReturnsTrue()
    {
        int[] nums = [1];

        Assert.True(IsGoodArray(nums));
    }

    private static bool IsGoodArray(int[] nums)
    {
        var gcd = nums[0];

        foreach (var value in nums)
        {
            gcd = Gcd(gcd, value);

            if (gcd == 1)
            {
                return true;
            }
        }

        return gcd == 1;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
