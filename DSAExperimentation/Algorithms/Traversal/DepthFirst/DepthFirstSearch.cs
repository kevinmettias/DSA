namespace DSAExperimentation.Algorithms.Traversal.DepthFirst;

// Successors is a plain runtime object, not a Topology witness, despite superficially
// resembling Graph's IGraphTopology<TNode,TChildren>.GetChildren: the discriminator isn't how
// often it's called (called on every visit here, same as GetChildren on every step there) but
// whether the space of valid choices is closed (ARCHITECTURE.md §9.2). Graph's
// IGraphTopology/IDagTopology/ITreeTopology chain is closed to exactly the three adjacency tiers
// Graph itself formalizes; a standalone successor relation has no enumerable set of "kinds" to
// close over - a chess-move generator, an infinite lattice, a cyclic closure are arbitrary caller
// logic, the same open-ended bucket IComparer<T> occupies. There's also nothing to close a
// witness over even if one were wanted: the only existing tiered hierarchy for "successor
// relation kinds" in this codebase is Graph's own, which is exactly what §5.5 forbids reusing.
//
// No Representation axis at all: a Func<TNode, IEnumerable<TNode>> has no alternate physical
// layout to abstract over the way a sequence does - it's already the atomic, opaque capability.
// The explicit stack and visited set below are Operations-internal scratch state (the same bucket
// as Graph's TrackedVisitGuard or BinarySearch's SearchRange), not a Representation of the input -
// the mirror image of DynamicArray (ARCHITECTURE.md §4.1: Representation only, no Topology).
//
// Unconditional cycle defense, no tiering: unlike Graph's UnguardedVisit/TrackedVisitGuard split
// (earned because Graph's tree tier has a real guarantee to exploit), Traverse never gets any
// promise stronger than "arbitrary function," so it only ever needs the guarded shape.
//
// Precondition law: successors must produce a finite reachable set from start. An infinite one -
// with no cycle for the visited set to catch - makes Traverse run forever, with no compiler or
// runtime error to catch it, the same unchecked-precondition shape as BinarySearch's sortedness
// or ShortestPath's non-negative-edge-weight assumption.
//
// notnull, deliberately not `class`: Graph's TNode : class assumes reference identity flowing
// through IChildren, a Graph-domain choice, not an inherent property of "successor-shaped"
// algorithms - an implicit-graph node (a chess position, say) is naturally a value-typed record.
internal static class DepthFirstSearch
{
    public static List<TNode> Traverse<TNode>(TNode start, Func<TNode, IEnumerable<TNode>> successors)
        where TNode : notnull
        => Traverse(start, successors, EqualityComparer<TNode>.Default);

    public static List<TNode> Traverse<TNode>(
        TNode start, Func<TNode, IEnumerable<TNode>> successors, IEqualityComparer<TNode> comparer)
        where TNode : notnull
    {
        var visited = new HashSet<TNode>(comparer);
        var order = new List<TNode>();
        var pending = new DSAExperimentation.DataStructures.Stack.Stack<TNode>();

        pending.Push(start);

        while (pending.TryPop(out var node))
        {
            if (!visited.Add(node))
            {
                continue;
            }

            order.Add(node);
            PushUnvisitedSuccessors(pending, visited, successors(node));
        }

        return order;
    }

    // Pushed in reverse so popping matches recursive DFS's visit order: the first-listed
    // successor is explored (and fully backtracked) before the second-listed one.
    private static void PushUnvisitedSuccessors<TNode>(
        DSAExperimentation.DataStructures.Stack.Stack<TNode> pending, HashSet<TNode> visited, IEnumerable<TNode> successors)
        where TNode : notnull
    {
        foreach (var successor in successors.Reverse())
        {
            if (!visited.Contains(successor))
            {
                pending.Push(successor);
            }
        }
    }
}
