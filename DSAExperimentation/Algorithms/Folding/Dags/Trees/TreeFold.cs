using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Folding.Dags.Trees;

internal static class TreeFold
{
    // Defaults to recursive evaluation. Safe to override TStrategy with
    // IterativeFoldEvaluation only when TAlgebra is pure - see IFoldAlgebra.
    public static TResult Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            RecursiveFoldEvaluation<TNode>, TAlgebra, TResult>(root);

    public static TResult Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TStrategy, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TStrategy : struct, IFoldEvaluationStrategy<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => root is null
            ? TAlgebra.Empty
            : TStrategy.Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(root);
}
