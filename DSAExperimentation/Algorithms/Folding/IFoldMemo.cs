using System.Diagnostics.CodeAnalysis;

namespace DSAExperimentation.Algorithms.Folding;

// The one axis the memoizing fold tiers differ by, as IVisitGuard is for the walks: what the
// recursion must remember about nodes it has already met. A DAG needs a memo - a shared descendant
// is combined once and its result reused. An arbitrary graph needs the memo and the nodes still on
// the recursion path, because meeting one of those again is a cycle no fold result can be defined
// over. A tree needs nothing - every node is met once - so the tree tier has no policy at all.
//
// FoldRecursion asks in a fixed order: TryRecall before TryOpen, so a finished shared descendant is
// reused rather than mistaken for a cycle; Close after Combine; Aborted after every child. Every
// policy is a struct and FoldRecursion constrains TMemo to struct, so each tier gets its own
// instantiation. With a reference-type TNode that instantiation is shared generic code, so a
// policy's members are reached through a runtime lookup rather than inlined; set against the
// dictionary work each policy does, that lookup is small.
internal interface IFoldMemo<TNode, TResult>
    where TNode : class
{
    bool Aborted { get; }

    bool TryRecall(TNode node, [MaybeNullWhen(false)] out TResult result);

    bool TryOpen(TNode node);

    void Close(TNode node, TResult result);
}
