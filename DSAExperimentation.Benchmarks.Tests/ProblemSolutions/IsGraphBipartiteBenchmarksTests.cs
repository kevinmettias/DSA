using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IsGraphBipartiteBenchmarks (ARCHITECTURE 17.9). Both arms are competing
// strategies for the same question - a hand-rolled colour-array DFS over the problem's own
// adjacency against this repo's composed BipartiteCheck over a materialized graph - so the two
// verdicts are compared directly. The agreement is honest but deliberately weak, and it is worth
// naming why: [GlobalSetup] builds every edge across one fixed A/B split, so the graph is bipartite
// by construction and both arms must answer true for every [Params] value. An arm that ignored its
// graph entirely and returned a constant true would agree just as well, and no return type is
// changed here to strengthen it - the fixture exists so that neither strategy short-circuits on an
// early colour conflict, not so that its verdict is discriminating. What the agreement does still
// catch is an arm that ever answers false, including one that misreads the materialized graph. The
// generated edges are seeded, so the same NodeCount must rebuild the same graph and the same
// verdict.
public sealed partial class IsGraphBipartiteBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    // Bipartite by construction: every edge crosses the fixture's fixed two-way split.
    private const bool BipartiteByConstruction = true;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameGraph() =>
        Assert.Equal(BuildHarness().IsBipartiteByColorArrayDfs(), BuildHarness().IsBipartiteByColorArrayDfs());

    [Fact]
    public void IsBipartiteByColorArrayDfs_TwoWaySplit_AgreesWithIsBipartiteByBipartiteCheck()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsBipartiteByBipartiteCheck(), harness.IsBipartiteByColorArrayDfs());
        Assert.Equal(BipartiteByConstruction, harness.IsBipartiteByColorArrayDfs());
    }

    [Fact]
    public void IsBipartiteByBipartiteCheck_TwoWaySplit_AgreesWithIsBipartiteByColorArrayDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsBipartiteByColorArrayDfs(), harness.IsBipartiteByBipartiteCheck());
        Assert.Equal(BipartiteByConstruction, harness.IsBipartiteByBipartiteCheck());
    }

    private static IsGraphBipartiteBenchmarks BuildHarness()
    {
        var harness = new IsGraphBipartiteBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
