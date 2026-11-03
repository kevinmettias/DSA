using DSAExperimentation.Algorithms.Walking;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Algorithms.Reducing;

// The third order a reduce walks in, beside depth-first and breadth-first: in-order - the left
// subtree, then the node, then the right subtree - the order a binary search tree holds its keys in.
// In-order needs a node's left and right, which a compacted IChildren view does not keep (see
// BinaryTreeChildren), so this reads BinaryTreeNode<TValue>.Left and Right directly and leaves the
// topology the reduce entry point names unused beyond the tier it proves. Enter fires at the in-order
// moment, between the two subtrees, and Exit never, the way breadth-first fires Enter alone. A done
// state stops the walk where it stands, so a search for the k-th key enters k nodes and no more.
//
// DataStructures' InOrderTraversal walks the same order for BinarySearchTree's own use, and stays
// there: the layering lets DataStructures read no Reducing type, and its hooks observe a whole walk.
// Stopping early belongs to a reduce's state, so an arm that can stop uses this. Each entry point
// picks the walk's stop check from the algebra's StopsEarly, as DepthFirstReduceOrder does.
/// <summary>Walks a binary tree in-order - left subtree, node, right subtree - entering each node between its subtrees.</summary>
public readonly struct InOrderReduceOrder<TValue> : IReduceOrderStrategy<BinaryTreeNode<TValue>>
{
    static TState IReduceOrderStrategy<BinaryTreeNode<TValue>>.Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
        BinaryTreeNode<TValue> root, TState seed, TGuard guard)
    {
        if (TAlgebra.StopsEarly)
        {
            return Visit<TGuard, TAlgebra, TState, AlgebraStopCheck>(root, 0, seed, guard);
        }

        return Visit<TGuard, TAlgebra, TState, NoStopCheck>(root, 0, seed, guard);
    }

    static TState IReduceOrderStrategy<BinaryTreeNode<TValue>>.EvaluateFrom<TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
        IEnumerable<BinaryTreeNode<TValue>> roots, TState seed, TGuard guard)
    {
        if (TAlgebra.StopsEarly)
        {
            return VisitEach<TGuard, TAlgebra, TState, AlgebraStopCheck>(roots, seed, guard);
        }

        return VisitEach<TGuard, TAlgebra, TState, NoStopCheck>(roots, seed, guard);
    }

    // Each root the guard admits in turn, over the one guard.
    private static TState VisitEach<TGuard, TAlgebra, TState, TStopCheck>(
        IEnumerable<BinaryTreeNode<TValue>> roots, TState state, TGuard guard)
        where TGuard : struct, IVisitGuard<BinaryTreeNode<TValue>>
        where TAlgebra : struct, IReduceAlgebra<BinaryTreeNode<TValue>, TState>
        where TStopCheck : struct, IStopCheck
    {
        foreach (var root in roots)
        {
            if (guard.ShouldVisit(root))
            {
                state = Visit<TGuard, TAlgebra, TState, TStopCheck>(root, 0, state, guard);

                if (TStopCheck.IsEnabled && TAlgebra.IsDone(state))
                {
                    return state;
                }
            }
        }

        return state;
    }

    // The left subtree, then the node, then the right one; a done state returns at once, and each
    // caller up the recursion checks again after its own left subtree returns. One method, as
    // FoldRecursion is: a helper per section would be one more stack frame per level.
    private static TState Visit<TGuard, TAlgebra, TState, TStopCheck>(
        BinaryTreeNode<TValue> node, int depth, TState state, TGuard guard)
        where TGuard : struct, IVisitGuard<BinaryTreeNode<TValue>>
        where TAlgebra : struct, IReduceAlgebra<BinaryTreeNode<TValue>, TState>
        where TStopCheck : struct, IStopCheck
    {
        if (node.Left is { } left && guard.ShouldVisit(left))
        {
            state = Visit<TGuard, TAlgebra, TState, TStopCheck>(left, depth + 1, state, guard);

            if (TStopCheck.IsEnabled && TAlgebra.IsDone(state))
            {
                return state;
            }
        }

        state = TAlgebra.Enter(state, node, depth);

        if (TStopCheck.IsEnabled && TAlgebra.IsDone(state))
        {
            return state;
        }

        if (node.Right is { } right && guard.ShouldVisit(right))
        {
            return Visit<TGuard, TAlgebra, TState, TStopCheck>(right, depth + 1, state, guard);
        }

        return state;
    }
}
