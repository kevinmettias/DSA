namespace DSAExperimentation.Algorithms.Traversal.DepthFirst;

// Enter fires before a node's children are walked, Exit after. Pre-order acts in Enter
// and leaves Exit empty, post-order the reverse, and a telemetry-style walk (open a
// span, recurse, close it) uses both - they're not three different traversals, just
// three ways of using this one contract.
//
// Members are instance members on a struct type parameter, so a hook carries what it
// records into - a list, a counter, the answer it is building - as its own fields, and
// every walk returns the hook value it finished with. Both members are required: a
// default interface member reached through a struct type parameter boxes the hook, so
// an empty body is how a hook ignores an event. A hook that holds a reference (a list
// it appends to) can be read through that reference afterwards; one that holds its
// counters by value must be read from the value the walk returns.
/// <summary>The hook a depth-first walk calls on entering every node and again on leaving it.</summary>
public interface IDepthFirstHooks<TNode>
{
    /// <summary>Observes a node as the walk reaches it, before the walk goes on to its children.</summary>
    /// <param name="node">The node being entered.</param>
    /// <param name="depth">The node's depth below the walk's root, 0 for the root.</param>
    void Enter(TNode node, int depth);

    /// <summary>Observes a node as the walk leaves it, after every node reached through it has been left.</summary>
    /// <param name="node">The node being left.</param>
    /// <param name="depth">The node's depth below the walk's root, 0 for the root.</param>
    void Exit(TNode node, int depth);
}
