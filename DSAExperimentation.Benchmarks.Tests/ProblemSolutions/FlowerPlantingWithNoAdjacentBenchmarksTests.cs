using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FlowerPlantingWithNoAdjacentBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. Setup derives the path list and its adjacency from GardenCount alone, so the
// same GardenCount must rebuild the same workload.
//
// Both strategies walk the gardens in ascending order and take the first flower type no planted
// neighbour already uses - the solution's own comment pins them to that one answer rather than to
// any valid colouring - so the returned array is positional and AnswerText.Of's order-sensitive
// rendering is the right comparison.
public sealed partial class FlowerPlantingWithNoAdjacentBenchmarksTests
{
    private const int SmallestGardenCount = 500;

    [Fact]
    public void Setup_SameGardenCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RawPathsRescan()),
            AnswerText.Of(BuildHarness().RawPathsRescan()));

    [Fact]
    public void RawPathsRescan_AgreesWithAdjacencyListWithSetTracking()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.AdjacencyListWithSetTracking()),
            AnswerText.Of(harness.RawPathsRescan()));
    }

    [Fact]
    public void AdjacencyListWithSetTracking_AgreesWithRawPathsRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RawPathsRescan()),
            AnswerText.Of(harness.AdjacencyListWithSetTracking()));
    }

    private static FlowerPlantingWithNoAdjacentBenchmarks BuildHarness()
    {
        var harness = new FlowerPlantingWithNoAdjacentBenchmarks { GardenCount = SmallestGardenCount };
        harness.Setup();

        return harness;
    }
}
