namespace DSAExperimentation.Tests.LeetCodeCoverage.FindGreatestCommonDivisorOfArray;

// LeetCode 1979. Find Greatest Common Divisor of Array: the answer is just
// Gcd(min(nums), max(nums)) - finding the array's own min/max is a one-pass scan
// with no interesting container to compose, and reducing two integers via the
// Euclidean algorithm is the same private helper CheckIfItIsAGoodArrayTests/
// XOfAKindInADeckOfCardsTests/NumberOfDifferentSubsequencesGCDsTests already reuse
// inline rather than promoting to a shared production type - CheckIfItIsAGoodArrayTests'
// own reasoning applies verbatim: "no repo container or algorithm primitive
// applies here."
public sealed partial class FindGreatestCommonDivisorOfArrayTests
{
    [Fact]
    public void FindGcd_ClassicExample_ReturnsGcdOfMinAndMax()
    {
        int[] nums = [2, 5, 6, 9, 10];

        var gcd = FindGcd(nums);

        Assert.Equal(2, gcd);
    }

    [Fact]
    public void FindGcd_MinAndMaxAreCoprime_ReturnsOne()
    {
        int[] nums = [7, 5, 6, 8, 3];

        var gcd = FindGcd(nums);

        Assert.Equal(1, gcd);
    }

    [Fact]
    public void FindGcd_SingleElement_ReturnsThatElement()
    {
        int[] nums = [3];

        var gcd = FindGcd(nums);

        Assert.Equal(3, gcd);
    }

    private static int FindGcd(int[] nums)
    {
        var min = nums[0];
        var max = nums[0];

        foreach (var num in nums)
        {
            min = Math.Min(min, num);
            max = Math.Max(max, num);
        }

        return Gcd(min, max);
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
