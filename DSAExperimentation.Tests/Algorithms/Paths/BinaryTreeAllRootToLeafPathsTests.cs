using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Paths;

// Proves BinaryTreeTopology/BinaryTreeChildren correctly close AllRootToLeafPaths'
// generics - same precedent as AllRootToLeafPathsTests.cs's own TestTopology-based
// cases, against BinaryTreeTrees.Sample():
//       4
//      / \
//     2   6
//    / \   \
//   1   3   7
public sealed partial class BinaryTreeAllRootToLeafPathsTests
{
    [Fact]
    public void Find_ReturnsEveryRootToLeafPath()
    {
        var root = BinaryTreeTrees.Sample();

        var paths = AllRootToLeafPaths.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root);

        var asValues = paths.Select(path => path.Select(n => n.Value).ToArray()).ToArray();

        Assert.Equal(
            new[] { new[] { 4, 2, 1 }, new[] { 4, 2, 3 }, new[] { 4, 6, 7 } },
            asValues);
    }

    [Fact]
    public void Find_SingleNode_ReturnsOnePathOfJustTheRoot()
    {
        var root = BinaryTreeTrees.SingleNode();

        var paths = AllRootToLeafPaths.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root);

        Assert.Equal(new[] { new[] { 1 } }, paths.Select(path => path.Select(n => n.Value).ToArray()));
    }

    [Fact]
    public void Find_NullRoot_ReturnsNoPaths()
    {
        var paths = AllRootToLeafPaths.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(null);

        Assert.Empty(paths);
    }
}
