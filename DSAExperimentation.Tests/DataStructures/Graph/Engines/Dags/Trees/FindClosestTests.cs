using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed partial class FindClosestTests
{
    [Fact]
    public void TryFind_EmptyTree_ReturnsFalse()
    {
        var found = FindClosest.TryFind<int>(null, 5, out _);

        Assert.False(found);
    }

    [Fact]
    public void TryFind_SingleNode_ReturnsThatValue()
    {
        var found = FindClosest.TryFind(BinaryTreeTrees.SingleNode(), 5, out var closest);

        Assert.True(found);
        Assert.Equal(1, closest);
    }

    // BinaryTreeTrees.Sample() holds {1,2,3,4,6,7}, validly BST-ordered.
    [Fact]
    public void TryFind_TargetBelowMinimum_ReturnsMinimum()
    {
        var found = FindClosest.TryFind(BinaryTreeTrees.Sample(), -10, out var closest);

        Assert.True(found);
        Assert.Equal(1, closest);
    }

    [Fact]
    public void TryFind_TargetAboveMaximum_ReturnsMaximum()
    {
        var found = FindClosest.TryFind(BinaryTreeTrees.Sample(), 100, out var closest);

        Assert.True(found);
        Assert.Equal(7, closest);
    }

    [Fact]
    public void TryFind_ExactMatchDeepInTree_ReturnsThatValue()
    {
        var found = FindClosest.TryFind(BinaryTreeTrees.Sample(), 3, out var closest);

        Assert.True(found);
        Assert.Equal(3, closest);
    }

    [Fact]
    public void TryFind_ExactMatchAtLeaf_ReturnsThatValue()
    {
        var found = FindClosest.TryFind(BinaryTreeTrees.Sample(), 7, out var closest);

        Assert.True(found);
        Assert.Equal(7, closest);
    }

    // 5 is equidistant from 4 (the root) and 6 (4's right child) - the shallower,
    // first-reached value wins the tie.
    [Fact]
    public void TryFind_TiedDistance_ReturnsTheShallowerValue()
    {
        var found = FindClosest.TryFind(BinaryTreeTrees.Sample(), 5, out var closest);

        Assert.True(found);
        Assert.Equal(4, closest);
    }
}
