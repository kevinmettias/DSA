using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubtreesWithMaxDistanceBetweenCitiesBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// a property of the answer that comes from the workload's shape rather than from either arm.
//
// The adjacency list is private, but the answer's own length is decided by it: the arms report one
// bucket per distance 1..CityCount - 1, so a tree of the documented size pins that length exactly.
public sealed partial class CountSubtreesWithMaxDistanceBetweenCitiesBenchmarksTests
{
    private const int SmallestCityCount = 10;

    [Fact]
    public void AllPairsBfsPerMask_SmallestCityCount_ReportsOneBucketPerDistance() =>
        Assert.Equal(SmallestCityCount - 1, BuildHarness().AllPairsBfsPerMask().Length);

    private static CountSubtreesWithMaxDistanceBetweenCitiesBenchmarks BuildHarness()
    {
        var harness = new CountSubtreesWithMaxDistanceBetweenCitiesBenchmarks { CityCount = SmallestCityCount };
        harness.Setup();

        return harness;
    }
}
