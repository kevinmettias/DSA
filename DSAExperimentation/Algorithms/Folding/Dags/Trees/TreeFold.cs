using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Folding.Dags.Trees;

// Each shape comes in two forms: (root) folds with the algebra's default value, which is all a
// stateless algebra needs; (root, algebra) hands over an algebra carrying runtime values.
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
        => Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(root, default);

    // In the topology's own child order.
    public static TResult Fold<TNode, TTopology, TChildren, TAlgebra, TResult>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, TAlgebra, TResult>(root);

    public static TResult Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(TNode? root, TAlgebra algebra)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            RecursiveFoldEvaluation<TNode>, TAlgebra, TResult>(root, algebra);

    // In the topology's own child order.
    public static TResult Fold<TNode, TTopology, TChildren, TAlgebra, TResult>(TNode? root, TAlgebra algebra)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TAlgebra, TResult>(
            root, algebra);

    public static TResult Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TStrategy, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TStrategy : struct, IFoldEvaluationStrategy<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TStrategy, TAlgebra, TResult>(root, default);

    // In the topology's own child order.
    public static TResult Fold<TNode, TTopology, TChildren, TStrategy, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TStrategy : struct, IFoldEvaluationStrategy<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TStrategy, TAlgebra, TResult>(
            root);

    public static TResult Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TStrategy, TAlgebra, TResult>(
        TNode? root, TAlgebra algebra)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TStrategy : struct, IFoldEvaluationStrategy<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => root is null
            ? algebra.Empty
            : TStrategy.Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(root, algebra);

    // In the topology's own child order.
    public static TResult Fold<TNode, TTopology, TChildren, TStrategy, TAlgebra, TResult>(
        TNode? root, TAlgebra algebra)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TStrategy : struct, IFoldEvaluationStrategy<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TStrategy, TAlgebra, TResult>(
            root, algebra);
}
