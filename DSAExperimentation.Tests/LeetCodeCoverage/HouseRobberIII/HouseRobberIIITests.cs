using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobberIII;

// LeetCode 337. House Robber III: this repo's generic TreeFold engine (already
// exercised by FoldTests) closed over a small IFoldAlgebra witness whose TResult is
// the (Robbed, NotRobbed) pair for the subtree rooted at each node - the exact same
// catamorphism shape TreeMetrics.Height uses for MaximumDepthOfBinaryTree, just with
// a richer per-node result than a single int.
public sealed partial class HouseRobberIIITests
{
    [Fact]
    public void Rob_ClassicExample_ReturnsBestNonAdjacentSum()
    {
        var root = new BinaryTreeNode<int>(3)
        {
            Left = new(2) { Right = new(3) },
            Right = new(3) { Right = new(1) },
        };

        Assert.Equal(7, Rob(root));
    }

    [Fact]
    public void Rob_SecondExample_ReturnsBestNonAdjacentSum()
    {
        var root = new BinaryTreeNode<int>(3)
        {
            Left = new(4) { Left = new(1), Right = new(3) },
            Right = new(5) { Right = new(1) },
        };

        Assert.Equal(9, Rob(root));
    }

    private static int Rob(BinaryTreeNode<int> root)
    {
        var (robbed, notRobbed) = TreeFold.Fold<
            BinaryTreeNode<int>,
            BinaryTreeTopology<int>,
            BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>,
            BinaryTreeChildren<int>,
            RobFoldAlgebra,
            (int Robbed, int NotRobbed)>(root);

        return Math.Max(robbed, notRobbed);
    }

    private readonly struct RobFoldAlgebra : IFoldAlgebra<BinaryTreeNode<int>, (int Robbed, int NotRobbed)>
    {
        public static (int Robbed, int NotRobbed) Empty => (0, 0);

        public static (int Robbed, int NotRobbed) Combine(
            BinaryTreeNode<int> node, IReadOnlyList<(int Robbed, int NotRobbed)> children)
        {
            var robbed = node.Value;
            var notRobbed = 0;

            foreach (var child in children)
            {
                robbed += child.NotRobbed;
                notRobbed += Math.Max(child.Robbed, child.NotRobbed);
            }

            return (robbed, notRobbed);
        }
    }
}
