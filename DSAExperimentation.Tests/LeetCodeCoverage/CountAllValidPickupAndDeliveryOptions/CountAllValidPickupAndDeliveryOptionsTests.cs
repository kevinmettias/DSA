using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountAllValidPickupAndDeliveryOptions;

// LeetCode 1359. Count All Valid Pickup and Delivery Options: inserting order i's
// P_i/D_i pair into an already-valid sequence of length 2(i-1) has exactly
// i * (2i - 1) valid placements (i slots for P_i among the 2i-1 new positions,
// since D_i must land somewhere after wherever P_i lands - LeetCode's own editorial
// derivation), giving the recurrence f(i) = f(i-1) * i * (2i-1). Memoized via this
// repo's own Memoizer the same way UniqueBinarySearchTreesTests memoizes its Catalan
// recurrence, with every running product reduced mod 1e9+7 as LeetCode requires.
public sealed partial class CountAllValidPickupAndDeliveryOptionsTests
{
    private const long Mod = 1_000_000_007;

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 6)]
    [InlineData(3, 90)]
    public void CountOrders_LeetCodeExamples_ReturnsValidSequenceCount(int n, long expected)
        => Assert.Equal(expected, CountOrders(n));

    private static long CountOrders(int n) => Memoizer.Memoize<int, long>(n, Ways);

    private static long Ways(int orders, Func<int, long> ways)
        => orders == 0 ? 1L : ways(orders - 1) * orders % Mod * (2 * orders - 1) % Mod;
}
