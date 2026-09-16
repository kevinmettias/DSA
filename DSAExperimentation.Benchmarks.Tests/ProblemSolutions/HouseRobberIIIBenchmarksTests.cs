using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HouseRobberIIIBenchmarks (ARCHITECTURE 17.9): both arms are
// HouseRobberIIISolution's - the hand-rolled post-order pair against this repo's generic TreeFold
// engine closed over RobFoldAlgebra - so a harness whose arms disagree is timing two different
// trees. Both arms answer with the best haul over the same random full binary tree, and that number
// is a property of the drawn node values rather than a value the class comment names, so agreement
// is the assertion. Neither arm mutates the tree, so one harness is safe to call twice in either
// order. Setup rebuilds that tree off one seed and the depth alone, so the same Depth must rebuild
// the same tree and the same haul.
public sealed partial class HouseRobberIIIBenchmarksTests
{
    private const int SmallestDepth = 10;

    [Fact]
    public void Setup_SameDepth_RebuildsTheSameTree() =>
        Assert.Equal(BuildHarness().RecursivePair(), BuildHarness().RecursivePair());

    [Fact]
    public void RecursivePair_RandomFullTree_AgreesWithTreeFoldAlgebra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TreeFoldAlgebra(), harness.RecursivePair());
    }

    [Fact]
    public void TreeFoldAlgebra_RandomFullTree_AgreesWithRecursivePair()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursivePair(), harness.TreeFoldAlgebra());
    }

    private static HouseRobberIIIBenchmarks BuildHarness()
    {
        var harness = new HouseRobberIIIBenchmarks { Depth = SmallestDepth };
        harness.Setup();

        return harness;
    }
}
