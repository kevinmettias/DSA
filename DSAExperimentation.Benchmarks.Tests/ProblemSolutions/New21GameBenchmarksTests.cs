using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for New21GameBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the un-memoized recursion tree against the same recurrence
// routed through this repo's Memoizer - so a harness whose arms disagree is computing two different
// probabilities. The class carries no [GlobalSetup] and nothing to prepare: StopAt is the whole
// workload, passed straight through to both arms, so the same StopAt must answer both.
//
// Both arms return a double, so they are compared under a named relative tolerance rather than by
// exact equality - the two recursions sum the same terms in different orders.
public sealed partial class New21GameBenchmarksTests
{
    private const int SmallestStopAt = 12;

    private const double RelativeTolerance = 1E-09;

    [Fact]
    public void MemoizedRecursion_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion(), RelativeTolerance);
    }

    [Fact]
    public void UnmemoizedRecursion_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion(), RelativeTolerance);
    }

    private static New21GameBenchmarks BuildHarness() => new() { StopAt = SmallestStopAt };
}
