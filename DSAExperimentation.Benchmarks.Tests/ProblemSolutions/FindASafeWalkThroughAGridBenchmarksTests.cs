using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindASafeWalkThroughAGridBenchmarks (ARCHITECTURE 17.9): the generic arms check
// only compares the arms with each other, so this pins the baseline's verdict on the seeded grid.
// Setup hands both arms LeetCode 3286's own maximum health, twice the grid's side, which the class
// documents as what keeps every run's answer meaningful instead of trivially false.
public sealed partial class FindASafeWalkThroughAGridBenchmarksTests
{
    private const int SmallestGridSize = 10;

    // Setup's documented outcome: at LeetCode's own health cap for this grid size, the walk is safe.
    private const bool ExpectedIsSafe = true;

    [Fact]
    public void IsSafeByBruteForceArrayDijkstra_SeededGridAtHealthCap_FindsASafeWalk() =>
        Assert.Equal(ExpectedIsSafe, BuildHarness().IsSafeByBruteForceArrayDijkstra());

    private static FindASafeWalkThroughAGridBenchmarks BuildHarness()
    {
        var harness = new FindASafeWalkThroughAGridBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
