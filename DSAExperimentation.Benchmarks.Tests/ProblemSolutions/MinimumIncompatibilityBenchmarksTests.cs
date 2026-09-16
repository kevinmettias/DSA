using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumIncompatibilityBenchmarks (ARCHITECTURE 17.9): both arms are
// MinimumIncompatibilitySolution's, the same methods MinimumIncompatibilityTests proves
// correct, and both answer the same question - the smallest total incompatibility over every
// legal grouping - so arms that disagree are timing two different problems.
//
// Setup's workload is seeded, so the same parameters must rebuild the same workload;
// otherwise two published numbers were never comparable in the first place.
public sealed partial class MinimumIncompatibilityBenchmarksTests
{
    // The smallest declared [Params] value: the harness only needs one real workload, and
    // the memoized and unmemoized arms search the same state space regardless of its size.
    private const int SmallestLength = 10;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().UnmemoizedRecursion(),
            BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static MinimumIncompatibilityBenchmarks BuildHarness()
    {
        var harness = new MinimumIncompatibilityBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
