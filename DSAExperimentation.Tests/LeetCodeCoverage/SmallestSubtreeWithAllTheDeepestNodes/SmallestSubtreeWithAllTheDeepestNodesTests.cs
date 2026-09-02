using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestSubtreeWithAllTheDeepestNodes;

// LeetCode 865. Smallest Subtree with all the Deepest Nodes: one TreeFold pass
// over this repo's own BinaryTreeNode<T> - DeepestSubtreeAlgebra tracks, per node,
// the deepest level reached in its subtree alongside the subtree root that already
// contains every one of ITS deepest nodes (a tie between children promotes to this
// node; otherwise the single deeper child's answer propagates unchanged), the same
// "(depth, candidate) fold state" shape DiameterAlgebra already establishes for
// LC543's DiameterOfBinaryTreeTests. No ready-made TreeMetrics facade fits this
// exact combination, so the algebra stays local to this test rather than joining
// Algorithms/Metrics - the same "don't promote a witness until a second consumer
// earns it" judgment call ARCHITECTURE.md Sec.5 step 3 already makes for Representation.
public sealed partial class SmallestSubtreeWithAllTheDeepestNodesTests
{
    [Fact]
    public void SubtreeWithAllDeepest_AllLeavesTiedAtSameDepth_ReturnsRoot()
    {
        var root = new BinaryTreeNode<int>(1)
        {
            Left = new BinaryTreeNode<int>(2) { Left = new BinaryTreeNode<int>(4), Right = new BinaryTreeNode<int>(5) },
            Right = new BinaryTreeNode<int>(3) { Left = new BinaryTreeNode<int>(6), Right = new BinaryTreeNode<int>(7) },
        };

        Assert.Same(root, SubtreeWithAllDeepest(root));
    }

    [Fact]
    public void SubtreeWithAllDeepest_SingleDeepestLeaf_ReturnsThatLeaf()
    {
        var deepest = new BinaryTreeNode<int>(3);
        var root = new BinaryTreeNode<int>(0) { Left = new BinaryTreeNode<int>(1) { Right = deepest } };

        Assert.Same(deepest, SubtreeWithAllDeepest(root));
    }

    private static BinaryTreeNode<int>? SubtreeWithAllDeepest(BinaryTreeNode<int>? root)
        => TreeFold.Fold<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            DeepestSubtreeAlgebra, (int Depth, BinaryTreeNode<int>? Node)>(root).Node;

    private readonly struct DeepestSubtreeAlgebra
        : IFoldAlgebra<BinaryTreeNode<int>, (int Depth, BinaryTreeNode<int>? Node)>
    {
        public static (int Depth, BinaryTreeNode<int>? Node) Empty => (-1, null);

        public static (int Depth, BinaryTreeNode<int>? Node) Combine(
            BinaryTreeNode<int> node, IReadOnlyList<(int Depth, BinaryTreeNode<int>? Node)> children)
        {
            if (children.Count == 0)
            {
                return (0, node);
            }

            var maxDepth = ComputeMaxDepth(children);
            var (deepest, tieCount) = FindDeepestAtMaxDepth(children, maxDepth);

            return (maxDepth + 1, tieCount == 1 ? deepest : node);
        }

        private static int ComputeMaxDepth(IReadOnlyList<(int Depth, BinaryTreeNode<int>? Node)> children)
        {
            var maxDepth = 0;
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i].Depth > maxDepth)
                {
                    maxDepth = children[i].Depth;
                }
            }

            return maxDepth;
        }

        private static (BinaryTreeNode<int>? Deepest, int TieCount) FindDeepestAtMaxDepth(
            IReadOnlyList<(int Depth, BinaryTreeNode<int>? Node)> children, int maxDepth)
        {
            BinaryTreeNode<int>? deepest = null;
            var tieCount = 0;
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i].Depth == maxDepth)
                {
                    tieCount++;
                    deepest = children[i].Node;
                }
            }

            return (deepest, tieCount);
        }
    }
}
