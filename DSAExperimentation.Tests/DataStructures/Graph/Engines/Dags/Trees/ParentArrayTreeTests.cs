using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed class ParentArrayTreeTests
{
    [Fact]
    public void Build_ReturnsOneNodePerEntryIndexedById()
    {
        var nodes = ParentArrayTree.Build([-1, 0, 0, 1]);

        var indices = Enumerable.Range(0, nodes.Length);

        Assert.Equal(4, nodes.Length);
        Assert.All(indices, i => Assert.Equal(i, nodes[i].Id));
    }

    [Fact]
    public void Build_PointsEdgesFromParentToChild()
    {
        var nodes = ParentArrayTree.Build([-1, 0, 0, 1]);

        Assert.Equal([1, 2], nodes[0].Children.Select(c => c.Id));
        Assert.Equal([3], nodes[1].Children.Select(c => c.Id));
        Assert.Empty(nodes[2].Children);
    }

    [Fact]
    public void Build_TreatsAnyNegativeEntryAsTheRoot()
    {
        var nodes = ParentArrayTree.Build([-1, 0]);

        Assert.DoesNotContain(nodes, node => node.Children.Contains(nodes[0]));
    }

    [Fact]
    public void Build_GivesEveryNonRootNodeExactlyOneParent()
    {
        var nodes = ParentArrayTree.Build([-1, 0, 0, 1, 1, 2]);

        var parentCounts = nodes.ToDictionary(node => node.Id, _ => 0);

        foreach (var child in nodes.SelectMany(node => node.Children))
        {
            parentCounts[child.Id]++;
        }

        Assert.Equal(0, parentCounts[0]);
        Assert.All(parentCounts.Where(p => p.Key != 0), p => Assert.Equal(1, p.Value));
    }

    [Fact]
    public void Build_SingleNode_HasNoChildren()
    {
        var nodes = ParentArrayTree.Build([-1]);

        Assert.Single(nodes);
        Assert.Empty(nodes[0].Children);
    }

    [Fact]
    public void Chain_ReturnsTheRootOfAPathWhereEachNodeHasOneChild()
    {
        var root = ParentArrayTree.Chain(4);

        Assert.Equal(0, root.Id);

        var walked = 0;
        var current = root;

        while (current.Children.Count > 0)
        {
            Assert.Single(current.Children);
            current = current.Children[0];
            walked++;
        }

        Assert.Equal(3, walked);
        Assert.Equal(3, current.Id);
    }

    [Fact]
    public void Chain_OfOne_IsASingleChildlessNode()
    {
        var root = ParentArrayTree.Chain(1);

        Assert.Equal(0, root.Id);
        Assert.Empty(root.Children);
    }
}
