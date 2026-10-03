using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Algorithms.Folding;

// The tree tier's recursive evaluation: FoldRecursion with UnmemoizedFold, the policy
// ITreeTopology's one-parent promise earns - nothing remembered, nothing checked.
// The call stack grows with the tree's depth; IterativeFoldEvaluation is the
// stack-safe alternative for a deep, unbalanced tree.
internal readonly struct RecursiveFoldEvaluation<TNode> : IFoldEvaluationStrategy<TNode>
    where TNode : class
{
    public static TResult Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(TNode root)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => FoldRecursion.Visit<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren, UnmemoizedFold<TNode, TResult>, TAlgebra, TResult>(
            root, default);
}
