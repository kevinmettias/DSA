using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestDistanceToTargetStringInACircularArrayBenchmarks (ARCHITECTURE 17.9).
// Arm agreement is BenchmarkArmsTests' job; this pins the answer the workload is built to have. Setup
// builds the word circle through the shared CircularArrayWorkloads fixture, which places the single
// target word diametrically opposite StartIndex - pinning the answer independently of either
// strategy: the shorter way round the circle is half its length.
public sealed partial class ShortestDistanceToTargetStringInACircularArrayBenchmarksTests
{
    private const int SmallestLength = 10;

    // The fixture's target sits diametrically opposite StartIndex, so half the circle is the
    // shortest way round.
    private const int DiametricDivisor = 2;
    private const int ExpectedDistance = SmallestLength / DiametricDivisor;

    [Fact]
    public void LinearScan_DiametricTarget_ReturnsHalfTheCircle() =>
        Assert.Equal(ExpectedDistance, BuildHarness().LinearScan());

    private static ShortestDistanceToTargetStringInACircularArrayBenchmarks BuildHarness()
    {
        var harness = new ShortestDistanceToTargetStringInACircularArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
