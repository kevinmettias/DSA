using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CloneGraph;

public sealed partial class CloneGraphTests
{
    [Fact]
    public void CloneGraph_TwoConnectedNodes_CreatesDistinctIsomorphicCopy()
    {
        var first = new Node(1); var second = new Node(2); first.Neighbors.Add(second); second.Neighbors.Add(first);
        var clone = Clone(first)!;
        Assert.NotSame(first, clone);
        Assert.Equal(1, clone.Value);
        Assert.Equal(2, clone.Neighbors[0].Value);
        Assert.Same(clone, clone.Neighbors[0].Neighbors[0]);
    }

    private static Node? Clone(Node? node)
    {
        if (node is null) return null;
        var clones = new HashMap<Node, Node>();
        Node Copy(Node current)
        {
            if (clones.TryGetValue(current, out var existing)) return existing;
            var clone = new Node(current.Value); clones.Set(current, clone);
            foreach (var neighbor in current.Neighbors) clone.Neighbors.Add(Copy(neighbor));
            return clone;
        }
        return Copy(node);
    }

    private sealed class Node(int value)
    {
        public int Value { get; } = value;
        public List<Node> Neighbors { get; } = [];
    }
}
