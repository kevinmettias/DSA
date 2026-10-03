using System.Diagnostics.CodeAnalysis;

namespace DSAExperimentation.Algorithms.Folding;

// The tree tier's policy: ITreeTopology promises every node one parent, so nothing is ever met
// twice. No memo, no cycle check - every answer is a constant, so FoldRecursion over this policy
// compiles to the plain recursion with nothing remembered.
internal readonly struct UnmemoizedFold<TNode, TResult> : IFoldMemo<TNode, TResult>
    where TNode : class
{
    public bool Aborted => false;

    public bool TryRecall(TNode node, [MaybeNullWhen(false)] out TResult result)
    {
        result = default;

        return false;
    }

    public bool TryOpen(TNode node) => true;

    public void Close(TNode node, TResult result)
    {
    }
}
