using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.DataStructures.Graph.ShortestPaths;

// Projects Heap's ordering axis down to "compare by priority only, ignore which node it's
// attached to" - the same move EdgeTopologyAsGraphTopology makes for Graph's IGraphTopology,
// just for Collections.Heap's IHeapOrder instead.
/// <summary>
/// A heap order on (Node, Priority) pairs that puts the lowest priority first, ignoring the node.
/// </summary>
public readonly struct ByPriorityOrder<TNode, TWeight> : IHeapOrder<(TNode Node, TWeight Priority)>
    where TWeight : IComparable<TWeight>
{
    public static bool HasPriority((TNode Node, TWeight Priority) candidate, (TNode Node, TWeight Priority) incumbent)
        => candidate.Priority.CompareTo(incumbent.Priority) < 0;
}
