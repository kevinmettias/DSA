using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContainVirusBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds
// the arms to agreeing on a rebuilt workload, so this asserts only a bound that comes from the
// workload's shape rather than from either arm.
//
// The grid is private, so the workload's documented shape is asserted through the walls the
// baseline reports: LC 749's answer counts walls built around quarantined regions, so it is never
// negative and never more than one wall per side of every cell in the grid.
public sealed partial class ContainVirusBenchmarksTests
{
    private const int SmallestSide = 10;
    private const int WallsPerCell = 4;

    [Fact]
    public void NaiveRecursiveFloodFill_SeededInfectionGrid_StaysWithinOneWallPerCellSide() =>
        Assert.InRange(
            BuildHarness().NaiveRecursiveFloodFill(),
            0,
            SmallestSide * SmallestSide * WallsPerCell);

    private static ContainVirusBenchmarks BuildHarness()
    {
        var harness = new ContainVirusBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
