namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSymbolInGrammar;

// LeetCode 779. K-th Symbol in Grammar: recursive halving over a single (n, k)
// pair - k's parent in row n-1 is (k+1)/2, and k picks up a bit flip exactly
// when it lands in the second half of that parent's expansion. No repo
// container or algorithm primitive applies here - there is nothing to compose
// over one running row/index pair, the same "lighter repo-primitive fit" case
// Pow(x, n)'s exponentiation by squaring already is.
public sealed partial class KthSymbolInGrammarTests
{
    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(2, 1, 0)]
    [InlineData(2, 2, 1)]
    [InlineData(3, 1, 0)]
    [InlineData(3, 2, 1)]
    [InlineData(3, 3, 1)]
    [InlineData(3, 4, 0)]
    [InlineData(4, 5, 1)]
    public void KthGrammar_MatchesBruteForceRowExpansion_ReturnsExpectedSymbol(int n, int k, int expected)
    {
        var actual = KthGrammar(n, k);
        Assert.Equal(expected, actual);
    }

    private static int KthGrammar(int n, int k)
    {
        if (n == 1)
        {
            return 0;
        }

        var parent = KthGrammar(n - 1, (k + 1) / 2);
        var isSecondHalfOfParent = k % 2 == 0;

        return isSecondHalfOfParent ? 1 - parent : parent;
    }
}
