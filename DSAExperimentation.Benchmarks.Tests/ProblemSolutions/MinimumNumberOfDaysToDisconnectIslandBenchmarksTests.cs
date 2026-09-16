using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfDaysToDisconnectIslandBenchmarks (ARCHITECTURE 17.9): both
// arms are MinimumNumberOfDaysToDisconnectIslandSolution's, the same methods
// MinimumNumberOfDaysToDisconnectIslandTests proves correct, and both return the fewest days
// that disconnect the island. The hand-rolled flood fill and this repo's own
// DepthFirstSearch.Traverse do the same connectivity work, so arms that disagree are timing two
// different problems.
//
// Setup's workload is deterministic and undocumented-parameter-free: an all-land Side x Side
// grid. The agreement below is weak by construction - a full rectangular grid has no
// articulation cell, so the class comment already pins both arms to the constant 2 and
// agreement would hold even for an arm that only returned that constant. It still catches an arm
// that ever returns anything else, and the fixture is what makes the benchmark measure the
// O((Side*Side)^2) worst case, so it is kept as is.
public sealed partial class MinimumNumberOfDaysToDisconnectIslandBenchmarksTests
{
    // The smallest declared [Params] value: every cell is land, so both arms run one
    // connectivity check per candidate removal regardless of the grid's size.
    private const int SmallestSide = 10;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().NaiveRecursiveFloodFill(),
            BuildHarness().NaiveRecursiveFloodFill());

    [Fact]
    public void NaiveRecursiveFloodFill_AgreesWithPrimitiveComposed()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrimitiveComposed(), harness.NaiveRecursiveFloodFill());
    }

    [Fact]
    public void PrimitiveComposed_AgreesWithNaiveRecursiveFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursiveFloodFill(), harness.PrimitiveComposed());
    }

    private static MinimumNumberOfDaysToDisconnectIslandBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfDaysToDisconnectIslandBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
