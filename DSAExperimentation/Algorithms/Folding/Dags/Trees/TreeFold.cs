using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Folding.Dags.Trees;

// Each shape comes in two forms: (root) folds with the algebra's default value, which is all a
// stateless algebra needs; (root, algebra) hands over an algebra carrying runtime values.
/// <summary>Folds a tree bottom-up, combining each node with its children's results.</summary>
public static class TreeFold
{
    // Defaults to recursive evaluation. Safe to override TStrategy with
    // IterativeFoldEvaluation only when TAlgebra is pure - see IFoldAlgebra.
    /// <summary>Folds a tree recursively with a default algebra and <c>TOrder</c>'s child order.</summary>
    /// <param name="root">The root of the tree to fold, which may be null.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
    public static TResult Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(root, default);

    // In the topology's own child order.
    /// <summary>Folds a tree recursively with a default algebra and the topology's own child order.</summary>
    /// <param name="root">The root of the tree to fold, which may be null.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
    public static TResult Fold<TNode, TTopology, TChildren, TAlgebra, TResult>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, TAlgebra, TResult>(root);

    /// <summary>Folds a tree recursively with the given algebra and <c>TOrder</c>'s child order.</summary>
    /// <param name="root">The root of the tree to fold, which may be null.</param>
    /// <param name="algebra">The algebra that combines each node with its children's results.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
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
    /// <summary>Folds a tree recursively with the given algebra and the topology's own child order.</summary>
    /// <param name="root">The root of the tree to fold, which may be null.</param>
    /// <param name="algebra">The algebra that combines each node with its children's results.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
    public static TResult Fold<TNode, TTopology, TChildren, TAlgebra, TResult>(TNode? root, TAlgebra algebra)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TAlgebra, TResult>(
            root, algebra);

    /// <summary>
    /// Folds a tree using <c>TStrategy</c> with a default algebra and <c>TOrder</c>'s child order.
    /// </summary>
    /// <param name="root">The root of the tree to fold, which may be null.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
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
    /// <summary>
    /// Folds a tree using <c>TStrategy</c> with a default algebra and the topology's own child order.
    /// </summary>
    /// <param name="root">The root of the tree to fold, which may be null.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
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

    /// <summary>
    /// Folds a tree using <c>TStrategy</c> with the given algebra and <c>TOrder</c>'s child order.
    /// </summary>
    /// <param name="root">The root of the tree to fold, which may be null.</param>
    /// <param name="algebra">The algebra that combines each node with its children's results.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
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
    /// <summary>
    /// Folds a tree using <c>TStrategy</c> with the given algebra and the topology's own child order.
    /// </summary>
    /// <param name="root">The root of the tree to fold, which may be null.</param>
    /// <param name="algebra">The algebra that combines each node with its children's results.</param>
    /// <returns>The folded result, or the algebra's Empty when <paramref name="root"/> is null.</returns>
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
