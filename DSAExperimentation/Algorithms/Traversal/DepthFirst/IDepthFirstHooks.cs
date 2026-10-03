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
internal interface IDepthFirstHooks<TNode>
{
    void Enter(TNode node, int depth);

    void Exit(TNode node, int depth);
}
