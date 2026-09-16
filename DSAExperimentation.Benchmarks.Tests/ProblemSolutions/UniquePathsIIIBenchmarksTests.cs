using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for UniquePathsIIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// UniquePathsIIISolution's competing strategies for the same question - a purpose-built recursion
// against the generic backtracking engine, whose per-branch delegate dispatch is the cost being
// measured - so a harness whose arms disagree is enumerating two different walks.
//
// The class carries no [Params]: the Hamiltonian-path count is far more sensitive to obstacle
// layout than to grid dimensions, so [GlobalSetup] fixes one 3 x 4 board - 12 walkable cells, the
// single 1-cell at the top-left corner, the single 2-cell at the bottom-right. Exactly four walks
// step from that corner onto every one of the 12 cells and end on the 2-cell; an independent
// enumeration of the board over its four neighbour directions returns the same four, so the count is
// asserted as a literal alongside the arms' agreement rather than left to the two arms to agree on a
// shared wrong number.
public sealed partial class UniquePathsIIIBenchmarksTests
{
    // The fixed 3 x 4 board admits exactly four corner-to-corner Hamiltonian walks.
    private const int ExpectedPathCount = 4;

    [Fact]
    public void Setup_FixedGrid_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().SpecializedRecursive(), BuildHarness().SpecializedRecursive());

    [Fact]
    public void SpecializedRecursive_FixedGrid_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPathCount, harness.SpecializedRecursive());
        Assert.Equal(harness.BacktrackEngine(), harness.SpecializedRecursive());
    }

    [Fact]
    public void BacktrackEngine_FixedGrid_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPathCount, harness.BacktrackEngine());
        Assert.Equal(harness.SpecializedRecursive(), harness.BacktrackEngine());
    }

    private static UniquePathsIIIBenchmarks BuildHarness()
    {
        var harness = new UniquePathsIIIBenchmarks();
        harness.Setup();

        return harness;
    }
}
