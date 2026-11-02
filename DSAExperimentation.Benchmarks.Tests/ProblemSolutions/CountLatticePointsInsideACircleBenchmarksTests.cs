using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountLatticePointsInsideACircleBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing, and to answering the same on a rebuilt
// harness and when called again on one, so this asserts what it cannot: a lower bound on the
// answer that comes from the workload's shape rather than from either arm.
public sealed partial class CountLatticePointsInsideACircleBenchmarksTests
{
    private const int SmallestCircleCount = 50;

    // The documented shape: every circle's radius is drawn from [1, 6), so each one covers at
    // least its own centre and the union the answer counts can never be empty.
    [Fact]
    public void FullGridScan_FiftySeededCircles_CountsAtLeastOnePoint() =>
        Assert.InRange(BuildHarness().FullGridScan(), 1, int.MaxValue);

    private static CountLatticePointsInsideACircleBenchmarks BuildHarness()
    {
        var harness = new CountLatticePointsInsideACircleBenchmarks { CircleCount = SmallestCircleCount };
        harness.Setup();

        return harness;
    }
}
