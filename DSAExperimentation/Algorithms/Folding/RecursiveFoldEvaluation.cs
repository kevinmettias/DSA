using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Algorithms.Folding;

// The tree tier's recursive evaluation. ITreeTopology promises every node one parent, so nothing
// is ever met twice and there is nothing to remember: fold the children in order, Combine, return.
// The call stack grows with the tree's depth; IterativeFoldEvaluation is the stack-safe
// alternative for a deep, unbalanced tree.
//
// Its own recursion rather than FoldRecursion's, which the DAG and graph tiers share. A memo
// policy is free only when the JIT can inline it, and with a reference-type TNode - every node
// type in this library - FoldRecursion is shared generic code, where each call into a policy
// generic over TNode goes through a runtime lookup instead. Four such calls per node cost the
// tree tier 40% on FoldTierBenchmarks; the tier that remembers nothing does not carry the axis.
/// <summary>Evaluates a tree fold by recursion, so the call stack grows with the tree's depth.</summary>
public readonly struct RecursiveFoldEvaluation<TNode> : IFoldEvaluationStrategy<TNode>
    where TNode : class
{
    public static TResult Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode root, TAlgebra algebra)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Visit<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(root, algebra, new FoldStack<TResult>());

    private static TResult Visit<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode node, TAlgebra algebra, FoldStack<TResult> stack)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        var orderedChildren = TOrder.Apply(TTopology.GetChildren(node));
        var start = stack.Top;

        for (var i = 0; i < orderedChildren.Count; i++)
        {
            var childResult = Visit<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
                orderedChildren.Get(i), algebra, stack);
            stack.Push(childResult);
        }

        var result = algebra.Combine(node, stack.Since(start));
        stack.PopTo(start);

        return result;
    }
}
