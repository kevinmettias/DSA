using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MaximizeAlternatingSumUsingSwapsWorkloads (ARCHITECTURE 17.7). The reading
// depends on enough swaps to join indices into nontrivial components, and on staying inside LC 3695's
// contract: every swap is [p, q] with 0 <= p < q < n, and no pair repeats. At this size and seed the
// raw draws do repeat a pair and put the higher index first, so both assertions are load-bearing.
public sealed partial class MaximizeAlternatingSumUsingSwapsWorkloadsTests
{
    private const int ElementCount = 1_000; // the benchmark's smallest size
    private const int Seed = 3695; // LC problem number
    private const int LowPosition = 0;
    private const int HighPosition = 1;

    [Fact]
    public void BuildSwaps_EverySwap_PairsTwoIndicesLowFirstInsideTheArray() =>
        Assert.All(
            Build(),
            swap =>
            {
                Assert.InRange(swap[LowPosition], 0, swap[HighPosition] - 1);
                Assert.InRange(swap[HighPosition], 1, ElementCount - 1);
            });

    [Fact]
    public void BuildSwaps_EveryPair_AppearsOnce()
    {
        var swaps = Build();

        Assert.Equal(swaps.Length, swaps.Select(swap => (swap[LowPosition], swap[HighPosition])).Distinct().Count());
    }

    [Fact]
    public void BuildSwaps_SameSeed_ReturnsTheSameSwaps() =>
        Assert.Equal(Build(), Build());

    private static int[][] Build() => MaximizeAlternatingSumUsingSwapsWorkloads.BuildSwaps(ElementCount, new Random(Seed));
}
