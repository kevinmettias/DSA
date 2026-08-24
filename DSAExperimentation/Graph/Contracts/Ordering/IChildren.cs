namespace DSAExperimentation.Graph.Contracts.Ordering;

// An indexable view over a node's children. Implementations are thin structs
// wrapping a concretely-typed backing collection (see ListChildren) - once
// JIT-specialized for a concrete TChildren, Count/Get resolve to direct,
// non-virtual calls all the way down to the backing storage. No interface
// dispatch, no boxing, unlike IReadOnlyList<TNode> - Get is a named lookup
// rather than an indexer precisely so this stays a non-collection contract.
internal interface IChildren<TNode>
{
    int Count { get; }

    TNode Get(int index);
}
