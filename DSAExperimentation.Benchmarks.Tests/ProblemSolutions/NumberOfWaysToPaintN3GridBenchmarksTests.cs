using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfWaysToPaintN3GridBenchmarks (ARCHITECTURE 17.9): both arms count the
// ways to paint the n x 3 grid - bottom-up tabulation over the (same, different) row-pattern pair
// against the Memoizer-driven top-down recursion over it - so a harness whose arms disagree is timing
// two different questions. The count modulo 1e9+7 is the problem's whole answer rather than a proxy.
// This class has no Setup: its whole workload is the [Params] axis, so the same RowCount is the whole
// of what a rebuild can vary.
public sealed partial class NumberOfWaysToPaintN3GridBenchmarksTests
{
    private const int SmallestRowCount = 1_000;

    [Fact]
    public void Tabulation_AgreesWithMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Memoized(), harness.Tabulation());
    }

    [Fact]
    public void Memoized_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.Memoized());
    }

    private static NumberOfWaysToPaintN3GridBenchmarks BuildHarness() =>
        new() { RowCount = SmallestRowCount };
}
