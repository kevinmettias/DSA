namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfPointIsReachable;

// LeetCode 2543. Check if Point Is Reachable: starting from (1, 1), the
// allowed moves are (x, y) -> (x, x + y), (x, y) -> (x + y, y),
// (x, y) -> (2x, y), and (x, y) -> (x, 2y). The two doubling moves inject (or,
// read backwards, freely remove) any factor of 2, while the two addition moves
// are exactly one step of the deterministic, reversible subtractive Euclidean
// algorithm on whatever is left - which only ever bottoms out at (1, 1) when
// that remainder is already 1. So (targetX, targetY) is reachable iff
// Gcd(targetX, targetY)'s odd part is 1, i.e. the gcd itself is a power of two
// (the one-line n & (n - 1) == 0 bit trick). Same private Euclidean Gcd helper
// this repo already reuses verbatim across FindGreatestCommonDivisorOfArrayTests/
// CheckIfItIsAGoodArrayTests/XOfAKindInADeckOfCardsTests - "no repo container or
// algorithm primitive applies here" for that piece, per those files' own
// reasoning.
public sealed partial class CheckIfPointIsReachableTests
{
    [Theory]
    [InlineData(6, 9, false)]
    [InlineData(4, 7, true)]
    [InlineData(1, 1, true)]
    [InlineData(2, 2, true)]
    [InlineData(12, 18, false)]
    public void IsReachable_LeetCodeExamples_ReturnsWhetherTargetIsReachable(int targetX, int targetY, bool expected)
    {
        var actual = IsReachable(targetX, targetY);
        Assert.Equal(expected, actual);
    }

    private static bool IsReachable(int targetX, int targetY)
    {
        var gcd = Gcd(targetX, targetY);
        return (gcd & (gcd - 1)) == 0;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
