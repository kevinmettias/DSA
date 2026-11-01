using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ErectTheFenceBenchmarks (ARCHITECTURE 17.9): a bound the workload's shape
// fixes without either arm. LC 587's answer is the set of trees the fence passes through; Setup draws
// Length points from one fixed seed inside a bounded grid, and a fence around a non-degenerate point
// set holds at least the three corners any triangle of them forces and at most every point.
public sealed partial class ErectTheFenceBenchmarksTests
{
    private const int SmallestLength = 50;
    private const int MinimumFencePointCountForAGridPointSet = 3;

    [Fact]
    public void EveryPairHalfPlaneScan_FiftyGridPoints_FencesBetweenATriangleAndEveryPoint() =>
        Assert.InRange(
            BuildHarness().EveryPairHalfPlaneScan().Count,
            MinimumFencePointCountForAGridPointSet,
            SmallestLength);

    private static ErectTheFenceBenchmarks BuildHarness()
    {
        var harness = new ErectTheFenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
