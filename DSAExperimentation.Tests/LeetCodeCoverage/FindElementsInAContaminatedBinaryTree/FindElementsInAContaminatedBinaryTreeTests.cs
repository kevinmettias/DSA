using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindElementsInAContaminatedBinaryTree;

// LeetCode 1261. Find Elements in a Contaminated Binary Tree: recover the tree's
// values with this repo's own TopDownTraversal - root.val = 0, and Descend computes
// 2*parent+1 for a left child / 2*parent+2 for a right child, exactly the "value
// inherited from the parent, computed fresh per child" shape ITopDownHooks exists
// for - collecting every recovered value into this repo's own Set<int> along the
// way, giving Find O(1) lookup instead of a search.
public sealed partial class FindElementsInAContaminatedBinaryTreeTests
{
    [Fact]
    public void Find_ValuesPresentInRecoveredTree_ReturnsTrue()
    {
        var elements = new FindElements(ContaminatedTree());

        Assert.True(elements.Find(0));
        Assert.True(elements.Find(1));
        Assert.True(elements.Find(2));
        Assert.True(elements.Find(3));
        Assert.True(elements.Find(4));
    }

    [Fact]
    public void Find_ValuesAbsentFromRecoveredTree_ReturnsFalse()
    {
        var elements = new FindElements(ContaminatedTree());

        Assert.False(elements.Find(5));
        Assert.False(elements.Find(6));
    }

    // Left/Right shape only - the contaminated values themselves (-1, per the
    // problem statement) never matter, since recovery overwrites every node's Value.
    // Shape: root -> left -> {left.left, left.right}; root -> right (leaf).
    private static BinaryTreeNode<int> ContaminatedTree()
        => new(-1)
        {
            Left = new(-1) { Left = new(-1), Right = new(-1) },
            Right = new(-1),
        };

    private sealed class FindElements
    {
        private readonly Set<int> _values;

        public FindElements(BinaryTreeNode<int> root)
        {
            _values = new Set<int>();

            TopDownTraversal.Walk<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
                RecoverHooks, (int Value, Set<int> Found)>(root, (0, _values));
        }

        public bool Find(int target) => _values.Has(target);

        private readonly struct RecoverHooks : ITopDownHooks<BinaryTreeNode<int>, (int Value, Set<int> Found)>
        {
            public static void Visit(
                BinaryTreeNode<int> node, (int Value, Set<int> Found) state, int depth, NodePosition position)
            {
                node.Value = state.Value;
                state.Found.TryAdd(state.Value);
            }

            public static (int Value, Set<int> Found) Descend(
                BinaryTreeNode<int> parent, (int Value, Set<int> Found) parentState, BinaryTreeNode<int> child)
                => (child == parent.Left ? 2 * parentState.Value + 1 : 2 * parentState.Value + 2, parentState.Found);
        }
    }
}
