using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfEnclavesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a hand-specialized recursive border flood fill against this
// repo's own DepthFirstSearch.Traverse walking each border-connected land component - so a harness
// whose arms disagree is counting the enclaves of two different grids. Setup fills the seeded
// side x side grid from the fixed land density, so the same Side must rebuild the same cells.
//
// Each strategy clones the shared grid internally before sinking anything, so neither arm can leave
// the workload altered for the other and one harness serves both calls in either order. Both arms
// return an int, so they are compared directly.
public sealed partial class NumberOfEnclavesBenchmarksTests
{
    private const int SmallestSide = 30;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NaiveRecursiveFloodFill(), BuildHarness().NaiveRecursiveFloodFill());

    [Fact]
    public void NaiveRecursiveFloodFill_AgreesWithDepthFirstSearchTraversal()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirstSearchTraversal(), harness.NaiveRecursiveFloodFill());
    }

    [Fact]
    public void DepthFirstSearchTraversal_AgreesWithNaiveRecursiveFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursiveFloodFill(), harness.DepthFirstSearchTraversal());
    }

    private static NumberOfEnclavesBenchmarks BuildHarness()
    {
        var harness = new NumberOfEnclavesBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
