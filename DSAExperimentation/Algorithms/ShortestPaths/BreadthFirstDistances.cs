using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.ShortestPaths;

// Every reachable node's edge count from root: Reduce.Graph in BreadthFirstReduceOrder with
// DistanceMapReduceAlgebra, named once so a call site states only the three types that vary - the
// node, its topology and its children - instead of the six the engine call spells out. Breadth-first
// order is what makes the depth a node is reported at its shortest edge count; DistanceMapReduceAlgebra
// records why the map covers every reachable node instead of stopping at a target.
//
// Its own file, not a ShortestPath method, for §16.1's reason: the same output shape as Dijkstra's
// distances, no shared mechanism. GridShortestPath and HammingDistances are this call fixed to one node
// type each, and forward here.
//
// What naming it costs: TNode is a reference type, so this method is shared generic code and its call
// into Reduce.Graph costs one generic-dictionary lookup per search, not per node; the walk itself runs as
// the same instantiation a caller spelling out all six types gets.
/// <summary>Every reachable node's edge count from a root, by a breadth-first reduce.</summary>
public static class BreadthFirstDistances
{
    /// <summary>Counts the fewest edges from a root to every node it reaches.</summary>
    /// <param name="root">The node the counts start from.</param>
    /// <returns>
    /// Each reachable node mapped to its shortest edge count from <paramref name="root"/>, which maps to 0.
    /// </returns>
    public static Dictionary<TNode, int> From<TNode, TTopology, TChildren>(TNode root)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Reduce.Graph<
            TNode, TTopology, TChildren,
            BreadthFirstReduceOrder<TNode>,
            DistanceMapReduceAlgebra<TNode>, Dictionary<TNode, int>>(root);

    // One target's edge count, from a walk that stops at the first node of the depth holding it
    // rather than mapping everything the root reaches: DistanceToTarget is done once it enters the
    // target, and declares StopsEarly so the walk asks it. The ask is a lookup at every node, so this
    // wins only when the target is reached well before the whole map would be.
    /// <summary>Counts the fewest edges from a root to one target, stopping once the target is reached.</summary>
    /// <param name="root">The node the count starts from.</param>
    /// <param name="target">The node whose distance is wanted, compared by its own equality.</param>
    /// <returns>
    /// The fewest edges from <paramref name="root"/> to <paramref name="target"/>, 0 when they are equal, or
    /// null when the root does not reach the target.
    /// </returns>
    public static int? To<TNode, TTopology, TChildren>(TNode root, TNode target)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Reduce.Graph<
            TNode, TTopology, TChildren,
            BreadthFirstReduceOrder<TNode>,
            DistanceToTarget<TNode>, TargetSearch<TNode>>(root, new TargetSearch<TNode>(target, Distance: null), []).Distance;

    // Every node's edge count from the nearest of several roots: one breadth-first search with every
    // root at depth 0 - what "how far is each cell from the nearest rotten orange, the nearest 0"
    // asks - rather than one search per root.
    /// <summary>Counts the fewest edges to every reachable node from whichever root is nearest.</summary>
    /// <param name="roots">The nodes the counts start from, each at distance 0.</param>
    /// <returns>Each node some root reaches mapped to its fewest edges from the nearest root.</returns>
    public static Dictionary<TNode, int> FromAll<TNode, TTopology, TChildren>(IEnumerable<TNode> roots)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Reduce.GraphFromAll<
            TNode, TTopology, TChildren,
            BreadthFirstReduceOrder<TNode>,
            DistanceMapReduceAlgebra<TNode>, Dictionary<TNode, int>>(roots, []);

    // The search for one node: what is sought, and its depth once the walk has entered it.
    private readonly record struct TargetSearch<TNode>(TNode Target, int? Distance);

    // Records the target's depth when the walk enters it, and is done from then on.
    private readonly struct DistanceToTarget<TNode> : IReduceAlgebra<TNode, TargetSearch<TNode>>
    {
        public static TargetSearch<TNode> Seed => default;

        public static bool StopsEarly => true;

        public static TargetSearch<TNode> Enter(TargetSearch<TNode> state, TNode node, int depth)
        {
            if (EqualityComparer<TNode>.Default.Equals(node, state.Target))
            {
                return state with { Distance = depth };
            }

            return state;
        }

        public static bool IsDone(TargetSearch<TNode> state) => state.Distance is not null;
    }
}
