using System.Diagnostics.CodeAnalysis;

namespace DSAExperimentation.Algorithms.Folding;

// The one axis the three recursive fold tiers differ by, as IVisitGuard is for the walks: what the
// recursion must remember about nodes it has already met. A tree needs nothing - every node is met
// once. A DAG needs a memo - a shared descendant is combined once and its result reused. An
// arbitrary graph needs the memo and the nodes still on the recursion path, because meeting one of
// those again is a cycle no fold result can be defined over.
//
// FoldRecursion asks in a fixed order: TryRecall before TryOpen, so a finished shared descendant is
// reused rather than mistaken for a cycle; Close after Combine; Aborted after every child. Every
// policy is a struct and FoldRecursion constrains TMemo to struct, so a tier's calls are direct and
// inline - the tree tier's constant answers fold away entirely.
internal interface IFoldMemo<TNode, TResult>
    where TNode : class
{
    bool Aborted { get; }

    bool TryRecall(TNode node, [MaybeNullWhen(false)] out TResult result);

    bool TryOpen(TNode node);

    void Close(TNode node, TResult result);
}
