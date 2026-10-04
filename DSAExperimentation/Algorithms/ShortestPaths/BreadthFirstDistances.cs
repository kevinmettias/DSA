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
internal static class BreadthFirstDistances
{
    public static Dictionary<TNode, int> From<TNode, TTopology, TChildren>(TNode root)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Reduce.Graph<
            TNode, TTopology, TChildren,
            BreadthFirstReduceOrder<TNode>,
            DistanceMapReduceAlgebra<TNode>, Dictionary<TNode, int>>(root);
}
