using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindBuildingWhereAliceAndBobCanMeetBenchmarks (ARCHITECTURE 17.9): the generic
// arms check only compares the arms with each other, so this holds the baseline's answer to the
// workload's own shape - Setup draws Length queries, and the answer holds exactly one building index
// per query drawn.
public sealed partial class FindBuildingWhereAliceAndBobCanMeetBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void BruteForce_SeededHeightAndQueryBatch_ReturnsOneBuildingPerQuery() =>
        Assert.Equal(SmallestLength, BuildHarness().BruteForce().Length);

    private static FindBuildingWhereAliceAndBobCanMeetBenchmarks BuildHarness()
    {
        var harness = new FindBuildingWhereAliceAndBobCanMeetBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
