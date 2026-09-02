using DSAExperimentation.LeetCode.CloneGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CloneGraph;

// Harness only. Both strategies live in CloneGraphSolution - a BCL-Dictionary
// memoized depth-first walk (pre-migration, an untested benchmark placeholder)
// and the same walk composed over this repo's own HashMap (pre-migration, the
// test's own private helper) - and are asserted against the same graphs, so a
// disagreement between them fails here rather than surfacing only as a
// benchmark/test mismatch. Node is internal, so - as in SameTreeTests - it
// stays out of a public TheoryData/[Theory] signature and is only ever handed
// to the solution through private helpers.
public sealed class CloneGraphTests
{
    [Fact]
    public void CloneByDictionaryDfs_TwoConnectedNodes_CreatesDistinctIsomorphicCopy()
    {
        var (first, _) = TwoConnectedNodes();

        var clone = CloneGraphSolution.CloneByDictionaryDfs(first)!;

        Assert.NotSame(first, clone);
        Assert.Equal(1, clone.Value);
        Assert.Equal(2, clone.Neighbors[0].Value);
        Assert.Same(clone, clone.Neighbors[0].Neighbors[0]);
    }

    [Fact]
    public void CloneByDictionaryDfs_NullNode_ReturnsNull() =>
        Assert.Null(CloneGraphSolution.CloneByDictionaryDfs(null));

    [Fact]
    public void CloneByDictionaryDfs_SingleNodeNoNeighbors_CreatesDistinctCopyWithNoNeighbors()
    {
        var node = SingleNode();

        var clone = CloneGraphSolution.CloneByDictionaryDfs(node)!;

        Assert.NotSame(node, clone);
        Assert.Equal(1, clone.Value);
        Assert.Empty(clone.Neighbors);
    }

    [Fact]
    public void CloneByDictionaryDfs_TriangleGraph_ClonesSharedNodeOnce()
    {
        var clone = CloneGraphSolution.CloneByDictionaryDfs(TriangleGraph())!;

        AssertTriangleClonedSharedNodeOnce(clone);
    }

    [Fact]
    public void CloneByHashMapDfs_TwoConnectedNodes_CreatesDistinctIsomorphicCopy()
    {
        var (first, _) = TwoConnectedNodes();

        var clone = CloneGraphSolution.CloneByHashMapDfs(first)!;

        Assert.NotSame(first, clone);
        Assert.Equal(1, clone.Value);
        Assert.Equal(2, clone.Neighbors[0].Value);
        Assert.Same(clone, clone.Neighbors[0].Neighbors[0]);
    }

    [Fact]
    public void CloneByHashMapDfs_NullNode_ReturnsNull() =>
        Assert.Null(CloneGraphSolution.CloneByHashMapDfs(null));

    [Fact]
    public void CloneByHashMapDfs_SingleNodeNoNeighbors_CreatesDistinctCopyWithNoNeighbors()
    {
        var node = SingleNode();

        var clone = CloneGraphSolution.CloneByHashMapDfs(node)!;

        Assert.NotSame(node, clone);
        Assert.Equal(1, clone.Value);
        Assert.Empty(clone.Neighbors);
    }

    [Fact]
    public void CloneByHashMapDfs_TriangleGraph_ClonesSharedNodeOnce()
    {
        var clone = CloneGraphSolution.CloneByHashMapDfs(TriangleGraph())!;

        AssertTriangleClonedSharedNodeOnce(clone);
    }

    // A node reached through two different edges (here, node 3 from both 1
    // and 2) must still be cloned exactly once - the case both strategies'
    // memoization exists for.
    private static void AssertTriangleClonedSharedNodeOnce(Node cloneA)
    {
        var cloneCViaB = cloneA.Neighbors.Single(n => n.Value == 2).Neighbors.Single(n => n.Value == 3);
        var cloneCViaDirectEdge = cloneA.Neighbors.Single(n => n.Value == 3);

        Assert.Same(cloneCViaDirectEdge, cloneCViaB);
    }

    private static (Node First, Node Second) TwoConnectedNodes()
    {
        var first = new Node(1);
        var second = new Node(2);
        first.Neighbors.Add(second);
        second.Neighbors.Add(first);
        return (first, second);
    }

    private static Node SingleNode() => new(1);

    private static Node TriangleGraph()
    {
        var a = new Node(1);
        var b = new Node(2);
        var c = new Node(3);
        a.Neighbors.Add(b); a.Neighbors.Add(c);
        b.Neighbors.Add(a); b.Neighbors.Add(c);
        c.Neighbors.Add(a); c.Neighbors.Add(b);
        return a;
    }
}
