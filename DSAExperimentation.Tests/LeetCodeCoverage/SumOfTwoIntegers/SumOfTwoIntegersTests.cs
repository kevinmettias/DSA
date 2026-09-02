namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfTwoIntegers;

// LeetCode 371. Sum of Two Integers: the problem's own constraint (no + or -) rules
// out every arithmetic operator, and no data structure applies either - this is a
// pure bitwise carry-propagation loop, the same "no stronger reusable primitive"
// shape MaximumSubarrayBenchmarks/GasStation/BestTimeToBuyAndSellStock already
// establish. XOR gives the carry-less sum of each bit pair; AND-then-shift gives
// the carry to fold back in next iteration; repeat until no carry remains.
public sealed partial class SumOfTwoIntegersTests
{
    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(2, 3, 5)]
    [InlineData(-2, 3, 1)]
    [InlineData(-1, -1, -2)]
    [InlineData(0, 0, 0)]
    [InlineData(int.MaxValue, 0, int.MaxValue)]
    public void GetSum_LeetCodeExamples_ReturnsArithmeticSum(int a, int b, int expected)
    {
        var actual = GetSum(a, b);
        Assert.Equal(expected, actual);
    }

    private static int GetSum(int a, int b)
    {
        while (b != 0)
        {
            var carry = (a & b) << 1;
            a ^= b;
            b = carry;
        }

        return a;
    }
}
