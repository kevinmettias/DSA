using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Algorithms.Folding;

// How to *compute* a fold, not what it means. Interchangeable for every algebra that
// keeps IFoldAlgebra's one rule - a pure Combine - since the strategies differ only in
// the order they call it.
/// <summary>How a tree fold is computed; for a pure algebra every strategy gives the same result.</summary>
public interface IFoldEvaluationStrategy<TNode>
    where TNode : class
{
    /// <summary>Folds the tree under a root, combining each node once, after all of its children.</summary>
    /// <param name="root">The root of the tree to fold, never null.</param>
    /// <param name="algebra">
    /// The algebra to combine with, handed each node's child results in <c>TOrder</c>'s order.
    /// </param>
    /// <returns>The folded result for <paramref name="root"/>.</returns>
    static abstract TResult Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode root, TAlgebra algebra)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>;
}
