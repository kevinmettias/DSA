using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SatisfiabilityOfEqualityEquationsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a freshly allocated adjacency list plus a BFS per
// inequality against this repo's own DisjointSet(26) - so a harness whose arms disagree is timing
// two different problems. Setup asks the fixture for the equations at a fixed seed, so the same
// EquationCount must rebuild the same equations; otherwise two published numbers were never
// comparable in the first place.
//
// Both arms read the hoisted equations without writing to them, so one harness is safe to call twice
// in either order and the single-harness rule holds. The fixture draws each equation over two
// uniformly chosen variables, so whether the system is satisfiable is a property of that draw
// rather than something the fixture's documented shape decides; the arms are reconciled against each
// other, which still catches an arm that ever reports the wrong verdict on a system the other
// settled.
public sealed partial class SatisfiabilityOfEqualityEquationsBenchmarksTests
{
    private const int SmallestEquationCount = 100;

    [Fact]
    public void Setup_SameEquationCount_RebuildsTheSameEquations() =>
        Assert.Equal(
            BuildHarness().IsSatisfiableByAdjacencyBfs(),
            BuildHarness().IsSatisfiableByAdjacencyBfs());

    [Fact]
    public void IsSatisfiableByAdjacencyBfs_SeededEquations_AgreesWithTheDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsSatisfiableByDisjointSet(), harness.IsSatisfiableByAdjacencyBfs());
    }

    [Fact]
    public void IsSatisfiableByDisjointSet_SeededEquations_AgreesWithTheAdjacencyBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsSatisfiableByAdjacencyBfs(), harness.IsSatisfiableByDisjointSet());
    }

    private static SatisfiabilityOfEqualityEquationsBenchmarks BuildHarness()
    {
        var harness = new SatisfiabilityOfEqualityEquationsBenchmarks { EquationCount = SmallestEquationCount };
        harness.Setup();

        return harness;
    }
}
