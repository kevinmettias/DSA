using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CoinChangeIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - bottom-up tabulation against a top-down recursion over the same
// (coin index, remaining amount) state - so a harness whose arms disagree is timing two different
// problems. The coin set is a static field and Amount is the only [Params] property, so the same
// Amount is the whole workload and both arms must report the same combination count for it.
public sealed partial class CoinChangeIIBenchmarksTests
{
    private const int SmallestAmount = 200;

    [Fact]
    public void Memoized_AmountTwoHundredOverOneFiveTenTwentyFiveFifty_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.Memoized());
    }

    [Fact]
    public void Tabulation_AmountTwoHundredOverOneFiveTenTwentyFiveFifty_AgreesWithMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Memoized(), harness.Tabulation());
    }

    private static CoinChangeIIBenchmarks BuildHarness() => new() { Amount = SmallestAmount };
}
