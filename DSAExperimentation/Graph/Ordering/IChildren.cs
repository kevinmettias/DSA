namespace DSAExperimentation.Graph;

// An indexable view over a node's children. Implementations are thin structs
// wrapping a concretely-typed backing collection (see ListChildren) - once
// JIT-specialized for a concrete TChildren, Count/this[] resolve to direct,
// non-virtual calls all the way down to the backing storage. No interface
// dispatch, no boxing, unlike IReadOnlyList<TNode>.
public interface IChildren<TNode>
{
    int Count { get; }

    TNode this[int index] { get; }
}
