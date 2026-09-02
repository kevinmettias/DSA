using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToRearrangeSticksWithKSticksVisible;

// LeetCode 1866. Number of Ways to Rearrange Sticks With K Sticks Visible: counting
// arrangements of sticks [1..n] with exactly k left-to-right visible (a stick is
// visible iff it's taller than every stick before it) is the unsigned Stirling
// numbers of the first kind, T(n,k) = T(n-1,k-1) + (n-1)*T(n-1,k) - placing stick n
// (the tallest) either as its own new visible stick (T(n-1,k-1), one way) or
// slotted in anywhere after one of the n-1 already-arranged sticks where it stays
// hidden behind them (T(n-1,k) ways, times n-1 insertion slots). Same top-down
// recurrence shape UniqueBinarySearchTreesTests already uses for a different
// counting recurrence, via this repo's own Memoizer<TState,TResult>, keyed on the
// (n,k) pair and reduced mod 1e9+7.
public sealed class NumberOfWaysToRearrangeSticksWithKSticksVisibleTests
{
    private const int Mod = 1_000_000_007;

    [Fact]
    public void RearrangeSticks_ThreeSticksTwoVisible_ReturnsThree()
    {
        var actual = RearrangeSticks(n: 3, k: 2);
        Assert.Equal(3, actual);
    }

    [Fact]
    public void RearrangeSticks_FiveSticksAllVisible_ReturnsOne()
    {
        var actual = RearrangeSticks(n: 5, k: 5);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void RearrangeSticks_TwentySticksElevenVisible_ReturnsSixHundredFortySevenMillionModResult()
    {
        var actual = RearrangeSticks(n: 20, k: 11);
        Assert.Equal(647_427_950, actual);
    }

    private static int RearrangeSticks(int n, int k)
        => (int)Memoizer.Memoize<(int N, int K), long>((n, k), Ways);

    private static long Ways((int N, int K) state, Func<(int N, int K), long> ways)
    {
        var (n, k) = state;

        if (n == 0)
        {
            return k == 0 ? 1 : 0;
        }

        if (k == 0)
        {
            return 0;
        }

        var placeAsNewVisible = ways((n - 1, k - 1));
        var hideAfterExistingStick = (n - 1) * ways((n - 1, k)) % Mod;
        return (placeAsNewVisible + hideAfterExistingStick) % Mod;
    }
}
