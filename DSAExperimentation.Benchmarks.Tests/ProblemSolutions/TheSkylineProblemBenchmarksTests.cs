using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TheSkylineProblemBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - re-deriving the active height from every building at each
// critical x against maintaining it incrementally in a max-heap - so a harness whose arms disagree
// is timing two different problems. Both arms return the [x, height] key points left to right,
// which is the order LC 218 itself reports the contour in, so the two lists are rendered
// order-sensitively. Setup draws the overlapping buildings from a fixed seed, so the same
// BuildingCount must rebuild the same workload.
public sealed partial class TheSkylineProblemBenchmarksTests
{
    private const int SmallestBuildingCount = 100;

    [Fact]
    public void Setup_SameBuildingCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceCriticalPoints()),
            AnswerText.Of(BuildHarness().BruteForceCriticalPoints()));

    [Fact]
    public void BruteForceCriticalPoints_SmallestBuildingCount_AgreesWithSweepLineHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SweepLineHeap()),
            AnswerText.Of(harness.BruteForceCriticalPoints()));
    }

    [Fact]
    public void SweepLineHeap_SmallestBuildingCount_AgreesWithBruteForceCriticalPoints()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceCriticalPoints()),
            AnswerText.Of(harness.SweepLineHeap()));
    }

    private static TheSkylineProblemBenchmarks BuildHarness()
    {
        var harness = new TheSkylineProblemBenchmarks { BuildingCount = SmallestBuildingCount };
        harness.Setup();

        return harness;
    }
}
