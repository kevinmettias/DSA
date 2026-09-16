using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for GoodSubtreeWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3575's
// value and parent arrays describing one tree - a single root, every other node hung off a
// lower-numbered one - with values spread wide enough that the per-node knapsack merge does real
// work on both of its branches.
public sealed partial class GoodSubtreeWorkloadsTests
{
    private const int NodeCount = 64;
    private const int Seed = 3575; // LC problem number
    private const int NoParent = -1;
    private const int RootNode = 0;
    private const int MinValue = 1;
    private const int ValueUpperBound = 99_999; // exclusive

    [Fact]
    public void Build_NodeCount_ReturnsOneValuePerNodeAndOneParentPerNode()
    {
        var (vals, par) = GoodSubtreeWorkloads.Build(NodeCount, Seed);

        Assert.Equal(NodeCount, vals.Length);
        Assert.Equal(NodeCount, par.Length);
    }

    [Fact]
    public void Build_EveryValue_StaysWithinTheDocumentedBound() =>
        Assert.All(
            GoodSubtreeWorkloads.Build(NodeCount, Seed).Vals,
            value => Assert.InRange(value, MinValue, ValueUpperBound - 1));

    // One node without a parent is what makes the forest a single tree, and every other node
    // pointing at a lower-numbered one is what keeps it acyclic with no separate cycle check.
    [Fact]
    public void Build_EveryNodeBeyondTheRoot_PointsAtALowerNumberedParent()
    {
        var (_, par) = GoodSubtreeWorkloads.Build(NodeCount, Seed);

        Assert.Equal(NoParent, par[RootNode]);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.InRange(par[node], RootNode, node - 1);
        }
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (vals, par) = GoodSubtreeWorkloads.Build(NodeCount, Seed);
        var (repeatVals, repeatPar) = GoodSubtreeWorkloads.Build(NodeCount, Seed);

        Assert.Equal(vals, repeatVals);
        Assert.Equal(par, repeatPar);
    }
}
