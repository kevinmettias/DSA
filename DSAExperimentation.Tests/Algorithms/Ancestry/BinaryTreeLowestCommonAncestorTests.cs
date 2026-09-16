using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Ancestry;

// Proves BinaryTreeTopology/BinaryTreeChildren correctly close LowestCommonAncestor's
// generics - same precedent as LowestCommonAncestorTests.cs's own TestTopology-based
// cases, against BinaryTreeTrees.Sample():
//       4
//      / \
//     2   6
//    / \   \
//   1   3   7
public sealed partial class BinaryTreeLowestCommonAncestorTests
{
    [Fact]
    public void Find_NodesInDifferentSubtrees_ReturnsSharedAncestor()
    {
        var root = BinaryTreeTrees.Sample();
        var one = Child(Child(root.Left).Left);
        var seven = Child(Child(root.Right).Right);

        var lca = LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root, one, seven);

        Assert.Same(root, lca);
    }

    [Fact]
    public void Find_OneNodeIsAncestorOfTheOther_ReturnsTheAncestor()
    {
        var root = BinaryTreeTrees.Sample();
        var six = Child(root.Right);
        var seven = Child(six.Right);

        var lca = LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root, six, seven);

        Assert.Same(six, lca);
    }

    [Fact]
    public void Find_SiblingsUnderSameParent_ReturnsTheParent()
    {
        var root = BinaryTreeTrees.Sample();
        var two = Child(root.Left);
        var one = Child(two.Left);
        var three = Child(two.Right);

        var lca = LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root, one, three);

        Assert.Same(two, lca);
    }

    // Each case above walks Sample()'s own documented shape back out, and the fixture
    // builds every edge of it with an object initializer, so no child it names is
    // absent. Reaching this means the fixture's shape changed under these cases.
    private static BinaryTreeNode<int> Child(BinaryTreeNode<int>? node) =>
        node ?? throw new InvalidOperationException(
            "BinaryTreeTrees.Sample() builds every edge of the shape its comment draws, so this child exists.");
}
