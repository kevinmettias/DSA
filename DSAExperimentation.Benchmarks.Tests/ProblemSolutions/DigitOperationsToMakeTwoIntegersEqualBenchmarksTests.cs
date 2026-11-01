using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DigitOperationsToMakeTwoIntegersEqualBenchmarks (ARCHITECTURE 17.9): a
// lower bound the workload's shape fixes without either arm. Setup pins each digit count to the
// widest composite endpoint pair LC 3377 admits, and 10 (the two-digit start) is composite, so a
// real cost exists and can never fall below the start value itself: LC 3377 charges the sum of
// every value the walk passes through, startValue included.
public sealed partial class DigitOperationsToMakeTwoIntegersEqualBenchmarksTests
{
    private const int SmallestDigitCount = 2;
    private const int TwoDigitStartValue = 10;

    [Fact]
    public void BruteForceDijkstra_CompositeEndpointPair_CostsAtLeastTheStartValue() =>
        Assert.True(BuildHarness().BruteForceDijkstra() >= TwoDigitStartValue);

    private static DigitOperationsToMakeTwoIntegersEqualBenchmarks BuildHarness()
    {
        var harness = new DigitOperationsToMakeTwoIntegersEqualBenchmarks { DigitCount = SmallestDigitCount };
        harness.Setup();

        return harness;
    }
}
