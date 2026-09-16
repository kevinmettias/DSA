using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CombinationSumIVBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - bottom-up tabulation against the Memoizer-backed top-down
// recursion - so a harness whose arms disagree is timing two different problems. The number set is a
// static field and Target is the only [Params] property, so the same Target is the whole workload.
//
// Both arms count the same orderings, so any wrapped int they report is the same wrapped int: the
// class comment says the running counts overflow far below this Target, which is harmless precisely
// because the overflow is identical on both sides and this class measures time, not the count.
public sealed partial class CombinationSumIVBenchmarksTests
{
    private const int SmallestTarget = 200;

    [Fact]
    public void Memoized_OrderingsSummingToTwoHundred_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.Memoized());
    }

    [Fact]
    public void Tabulation_OrderingsSummingToTwoHundred_AgreesWithMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Memoized(), harness.Tabulation());
    }

    private static CombinationSumIVBenchmarks BuildHarness() => new() { Target = SmallestTarget };
}
