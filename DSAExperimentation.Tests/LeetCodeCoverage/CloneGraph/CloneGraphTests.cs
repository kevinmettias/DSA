using DSAExperimentation.LeetCode.CloneGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CloneGraph;

// Harness only. Both strategies live in CloneGraphSolution - a BCL-Dictionary
// memoized depth-first walk (pre-migration, an untested benchmark placeholder)
// and the same walk composed over this repo's own HashMap (pre-migration, the
// test's own private helper) - and are asserted against the same graphs, so a
// disagreement between them fails here rather than surfacing only as a
// benchmark/test mismatch. Node is internal, so - as in SameTreeTests - it stays
// out of a public TheoryData/[Theory] signature; a case names its graph in
// LeetCode's own adjacency-list shape and Build reconstructs it from that.
public sealed class CloneGraphTests
{
    // LeetCode's adjacency-list shape: entry i names the 1-based values that node
    // i + 1 is adjacent to, so [[2], [1]] is LeetCode's two-node example and [[]]
    // is a lone node with no neighbours.
    public static TheoryData<int[][]> Graphs =>
        new()
        {
            { [[2], [1]] },
            { [[]] },
            { [[2, 3], [1, 3], [1, 2]] },
        };

    [Theory]
    [MemberData(nameof(Graphs))]
    public void CloneByDictionaryDfs_LeetCodeExamples_CreatesADistinctIsomorphicCopy(int[][] adjacency)
    {
        var source = Build(adjacency);

        AssertDistinctIsomorphicCopy(source, CloneGraphSolution.CloneByDictionaryDfs(source));
    }

    [Theory]
    [MemberData(nameof(Graphs))]
    public void CloneByHashMapDfs_LeetCodeExamples_CreatesADistinctIsomorphicCopy(int[][] adjacency)
    {
        var source = Build(adjacency);

        AssertDistinctIsomorphicCopy(source, CloneGraphSolution.CloneByHashMapDfs(source));
    }

    [Fact]
    public void CloneByDictionaryDfs_NullNode_ReturnsNull() =>
        Assert.Null(CloneGraphSolution.CloneByDictionaryDfs(null));

    [Fact]
    public void CloneByHashMapDfs_NullNode_ReturnsNull() =>
        Assert.Null(CloneGraphSolution.CloneByHashMapDfs(null));

    // A node reached through two different edges (here, node 3 from both 1 and 2)
    // must still be cloned exactly once - the case both strategies' memoization
    // exists for - so every source node is walked to its counterpart: a different
    // object, the same value, the same neighbours in the same order.
    private static void AssertDistinctIsomorphicCopy(Node source, Node? copy)
    {
        var clone = Assert.IsType<Node>(copy);
        var counterparts = new Dictionary<Node, Node>();

        AssertCorresponds(source, clone, counterparts);
    }

    // The counterparts map is both the isomorphism's proof and its visited set: a
    // source node already seen must come back as the SAME clone, which is what
    // pins the shared descendant to one copy and what terminates a cyclic graph.
    // A node seen for the first time is recorded and then checked against its
    // neighbours.
    private static void AssertCorresponds(Node source, Node clone, Dictionary<Node, Node> counterparts)
    {
        AssertDistinctCopyOfTheSameNode(source, clone);

        if (counterparts.TryGetValue(source, out var known))
        {
            Assert.Same(known, clone);
            return;
        }

        counterparts[source] = clone;
        AssertNeighborsCorrespond(source, clone, counterparts);
    }

    private static void AssertDistinctCopyOfTheSameNode(Node source, Node clone)
    {
        Assert.NotSame(source, clone);
        Assert.Equal(source.Value, clone.Value);
    }

    private static void AssertNeighborsCorrespond(Node source, Node clone, Dictionary<Node, Node> counterparts)
    {
        Assert.Equal(source.Neighbors.Count, clone.Neighbors.Count);

        for (var i = 0; i < source.Neighbors.Count; i++)
        {
            AssertCorresponds(source.Neighbors[i], clone.Neighbors[i], counterparts);
        }
    }

    // Node i + 1 is constructed first and the edges are wired in a second pass, in
    // the order the case gave them, so a case states its graph exactly the way
    // LeetCode publishes it.
    private static Node Build(int[][] adjacency)
    {
        var nodes = new Node[adjacency.Length];

        for (var i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new Node(i + 1);
        }

        for (var i = 0; i < adjacency.Length; i++)
        {
            foreach (var value in adjacency[i])
            {
                nodes[i].Neighbors.Add(nodes[value - 1]);
            }
        }

        return nodes[0];
    }
}
