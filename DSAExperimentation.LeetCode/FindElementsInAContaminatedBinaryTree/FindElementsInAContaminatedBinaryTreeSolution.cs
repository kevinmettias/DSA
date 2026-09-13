using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.FindElementsInAContaminatedBinaryTree;

// LeetCode 1261. Find Elements in a Contaminated Binary Tree: a design problem -
// the object is seeded with a tree whose every value has been overwritten with -1,
// recovers the real values (root.val = 0, a left child is 2*parent+1 and a right
// child is 2*parent+2), and then answers repeated Find(target) queries.
//
// The contaminated values themselves never matter: recovery overwrites every node,
// so only the tree's shape is input. Both strategies below recover in one walk and
// differ only in what they recover into, which is what decides the cost of every
// later Find - a BCL List scanned linearly, or this repo's own Set<int> hashed in
// O(1).
//
// IFindElements is bespoke to this problem alone - no other LeetCode entry shares a
// "recover a contaminated tree, then answer membership" contract - so it stays here
// rather than in DataStructures/, the same placement CompleteBinaryTreeInserter's
// ICompleteBinaryTreeInserter gets for LC 919.
internal static class FindElementsInAContaminatedBinaryTreeSolution
{
    private const int ChildIndexMultiplier = 2;
    private const int RightChildOffset = 2;

    // The textbook baseline this composition has to justify itself against: recover
    // with a plain recursive DFS into a BCL List<int> and answer each Find with a
    // linear Contains scan, O(recovered count) per query. Deliberately written
    // without this repo's primitives.
    public static IFindElements CreateByListScan(BinaryTreeNode<int> root) => new ListScanElements(root);

    // The composed answer: recover with this repo's own TopDownTraversal, whose
    // ITopDownHooks contract is exactly the "value inherited from the parent,
    // computed fresh per child" shape this problem's 2*parent+1 / 2*parent+2 rule
    // needs, collecting into this repo's own Set<int> so every Find is O(1).
    public static IFindElements CreateByTopDownSet(BinaryTreeNode<int> root) => new TopDownSetElements(root);

    private sealed class ListScanElements : IFindElements
    {
        private readonly List<int> _values = [];

        public ListScanElements(BinaryTreeNode<int> root) => Recover(root, 0, _values);

        public bool Find(int target) => _values.Contains(target);

        private static void Recover(BinaryTreeNode<int>? node, int value, List<int> values)
        {
            if (node is null)
            {
                return;
            }

            node.Value = value;
            values.Add(value);
            Recover(node.Left, (ChildIndexMultiplier * value) + 1, values);
            Recover(node.Right, (ChildIndexMultiplier * value) + RightChildOffset, values);
        }
    }

    private sealed class TopDownSetElements : IFindElements
    {
        private readonly Set<int> _values = new();

        public TopDownSetElements(BinaryTreeNode<int> root) =>
            TopDownTraversal.Walk<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
                RecoverHooks, (int Value, Set<int> Found)>(root, (0, _values));

        public bool Find(int target) => _values.Has(target);

        // A witness for this problem alone: writes the recovered value onto the node
        // and records it, then hands each child the value its own position implies.
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
                => (child == parent.Left
                    ? (ChildIndexMultiplier * parentState.Value) + 1
                    : (ChildIndexMultiplier * parentState.Value) + RightChildOffset, parentState.Found);
        }
    }
}

// The Find contract every strategy above implements - LeetCode's own FindElements
// API, reduced to the one query it exposes. Bespoke to this problem, so it stays
// beside the solution rather than in DataStructures/.
internal interface IFindElements
{
    bool Find(int target);
}
