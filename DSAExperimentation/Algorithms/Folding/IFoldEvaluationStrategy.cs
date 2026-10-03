using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Algorithms.Folding;

// How to *compute* a fold, not what it means. Interchangeable for every algebra that
// keeps IFoldAlgebra's one rule - a pure Combine - since the strategies differ only in
// the order they call it.
internal interface IFoldEvaluationStrategy<TNode>
    where TNode : class
{
    static abstract TResult Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode root, TAlgebra algebra)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>;
}
