using System.Diagnostics.CodeAnalysis;

namespace DSAExperimentation.Algorithms.Folding;

// The DAG tier's policy: IDagTopology rules out cycles but not sharing, so a node reached along two
// paths is combined once and its result reused. A readonly struct whose one field is the memo, so
// the copies threaded down the recursion all read and write the same Dictionary - the aliasing
// ListChildren has over its List.
internal readonly struct MemoizedFold<TNode, TResult>(Dictionary<TNode, TResult> completed)
    : IFoldMemo<TNode, TResult>
    where TNode : class
{
    public bool Aborted => false;

    public bool TryRecall(TNode node, [MaybeNullWhen(false)] out TResult result) => completed.TryGetValue(node, out result);

    public bool TryOpen(TNode node) => true;

    public void Close(TNode node, TResult result) => completed[node] = result;
}
