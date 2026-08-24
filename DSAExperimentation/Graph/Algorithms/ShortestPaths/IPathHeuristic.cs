using System.Numerics;

namespace DSAExperimentation.Graph.Algorithms.ShortestPaths;

// The single axis ShortestPath.AStar generalizes Dijkstra on: what a candidate's
// queue priority is, beyond its real accumulated distance. ZeroHeuristic makes
// priority == real distance, which is exactly Dijkstra's relaxation order. Any other
// heuristic that never overestimates the true remaining distance to target
// ("admissible") biases the same relaxation loop toward the target without breaking
// its shortest-path correctness - it only changes which node gets settled first, not
// whether a node's first settle is its true shortest distance.
internal interface IPathHeuristic<TNode, TWeight>
    where TNode : class
    where TWeight : INumber<TWeight>
{
    static abstract TWeight Estimate(TNode node, TNode? target);
}
