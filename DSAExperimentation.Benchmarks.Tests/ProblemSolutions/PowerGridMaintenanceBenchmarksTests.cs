using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PowerGridMaintenanceBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the per-query answers over the same grid and query stream - so a
// harness whose arms disagree is timing two different problems. StationCount is the only [Params]
// axis and Setup derives both the connections and the queries from it, so the same StationCount must
// rebuild the same workload.
public sealed partial class PowerGridMaintenanceBenchmarksTests
{
    private const int SmallestStationCount = 200;

    [Fact]
    public void Setup_SameStationCount_RebuildsTheSameQueries() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().UnionFindSortedSet()),
            AnswerText.Of(BuildHarness().UnionFindSortedSet()));

    [Fact]
    public void UnionFindSortedSet_AgreesWithUnionFindHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.UnionFindHeap()),
            AnswerText.Of(harness.UnionFindSortedSet()));
    }

    [Fact]
    public void UnionFindHeap_AgreesWithUnionFindSortedSet()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.UnionFindSortedSet()),
            AnswerText.Of(harness.UnionFindHeap()));
    }

    private static PowerGridMaintenanceBenchmarks BuildHarness()
    {
        var harness = new PowerGridMaintenanceBenchmarks { StationCount = SmallestStationCount };
        harness.Setup();

        return harness;
    }
}
