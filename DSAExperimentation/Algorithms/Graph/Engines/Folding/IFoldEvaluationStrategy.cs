using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Algorithms.Graph.Engines.Folding;

// How to *compute* a fold, not what it means - see the purity caveat on
// IFoldAlgebra. Interchangeable strategies for pure algebras; NOT interchangeable
// once Enter/Combine have observable side effects.
internal interface IFoldEvaluationStrategy<TNode>
    where TNode : class
{
    static abstract TResult Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(TNode root)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>;
}
