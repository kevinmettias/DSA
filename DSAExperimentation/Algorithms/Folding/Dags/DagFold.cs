using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags;
using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Folding.Dags;

// The trusted counterpart to CheckedFold: IDagTopology already promises acyclicity,
// so there's nothing to defend against - no inProgress tracking, no cycle check, just
// memoization for the (still real, still expected) case of a shared descendant. That
// is the whole difference from the other tiers, and it is the policy this entry point
// hands FoldRecursion: MemoizedFold. If the promise turns out to be false, this
// stack-overflows or hangs, the same way RecursiveFoldEvaluation would on a non-tree
// ITreeTopology - CheckedFold is the checked fallback for when you can't vouch for
// acyclicity yourself.
/// <summary>Folds a graph its topology promises is acyclic, combining a shared descendant only once.</summary>
public static class DagFold
{
    /// <summary>Folds a DAG with a default algebra and <c>TOrder</c>'s child order.</summary>
    /// <param name="root">The node to fold from, which may be null.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
    public static TResult Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IDagTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(root, default);

    // In the topology's own child order.
    /// <summary>Folds a DAG with a default algebra and the topology's own child order.</summary>
    /// <param name="root">The node to fold from, which may be null.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
    public static TResult Fold<TNode, TTopology, TChildren, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IDagTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, TAlgebra, TResult>(root);

    /// <summary>Folds a DAG with the given algebra and <c>TOrder</c>'s child order.</summary>
    /// <param name="root">The node to fold from, which may be null.</param>
    /// <param name="algebra">The algebra that combines each node with its children's results.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
    public static TResult Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode? root, TAlgebra algebra)
        where TNode : class
        where TTopology : struct, IDagTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => root is null
            ? algebra.Empty
            : FoldRecursion.Visit<
                TNode, TTopology, TChildren, TOrder, TOrderedChildren, MemoizedFold<TNode, TResult>, TAlgebra, TResult>(
                root, new MemoizedFold<TNode, TResult>([]), algebra);

    // In the topology's own child order.
    /// <summary>Folds a DAG with the given algebra and the topology's own child order.</summary>
    /// <param name="root">The node to fold from, which may be null.</param>
    /// <param name="algebra">The algebra that combines each node with its children's results.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
    public static TResult Fold<TNode, TTopology, TChildren, TAlgebra, TResult>(
        TNode? root, TAlgebra algebra)
        where TNode : class
        where TTopology : struct, IDagTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TAlgebra, TResult>(
            root, algebra);
}
