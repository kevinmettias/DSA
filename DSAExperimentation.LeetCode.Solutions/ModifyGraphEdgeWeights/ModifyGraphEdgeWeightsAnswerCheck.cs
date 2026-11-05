using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using CheckedNode = DSAExperimentation.DataStructures.Graph.Adjacency.WeightedAdjacencyNode<long>;
using CheckedTopology = DSAExperimentation.DataStructures.Graph.Adjacency.WeightedAdjacencyTopology<long>;

namespace DSAExperimentation.LeetCode.Solutions.ModifyGraphEdgeWeights;

// LC 2699 accepts any weights for the -1 edges, each from 1 to 2 * 10^9, that make the shortest distance from
// source to destination exactly target, with every other edge as it was ("If there are multiple
// modifications that make the distance ... equal to target, any of them will be considered correct"), the
// edges in any order; and the empty array only when no assignment can. The catalog records one assignment,
// so an answer stands when it is empty exactly where the recorded one is, or else such an assignment. Both
// strategies stretch the -1 edges from the floor weight in edge order, and neither picks the catalog's
// weights. Distances are summed as longs, since two edges of 2 * 10^9 pass an int.
internal static class ModifyGraphEdgeWeightsAnswerCheck
{
    // The heaviest weight LeetCode lets a -1 edge take.
    private const long HeaviestWeight = 2_000_000_000;

    public static bool IsAccepted(
        int nodeCount, int[][] edges, int source, int destination, int target, int[][] answer, int[][] recorded)
    {
        if (answer.Length == 0 || recorded.Length == 0)
        {
            return answer.Length == recorded.Length;
        }

        return IsTheEdgesReweighted(edges, answer) && DistanceOf(nodeCount, answer, source, destination) == target;
    }

    // Whether the answer holds each edge once, in any order: an edge LeetCode weighs as it was, and a -1 edge
    // at a weight it allows.
    private static bool IsTheEdgesReweighted(int[][] edges, int[][] answer)
    {
        if (answer.Length != edges.Length || answer.Any(edge => edge.Length != edges[0].Length))
        {
            return false;
        }

        var given = edges.OrderBy(edge => (edge[0], edge[1]));
        var answered = answer.OrderBy(edge => (edge[0], edge[1]));

        return given.Zip(answered).All(pair => IsReweighted(pair.First, pair.Second));
    }

    private static bool IsReweighted(int[] given, int[] answered)
    {
        var isTheEdge = given[0] == answered[0] && given[1] == answered[1];

        if (given[2] == AssignableEdgeWeights.Unassigned)
        {
            return isTheEdge && IsAssignableWeight(answered[2]);
        }

        return isTheEdge && answered[2] == given[2];
    }

    private static bool IsAssignableWeight(int weight) => weight >= AssignableEdgeWeights.FloorWeight && weight <= HeaviestWeight;

    // The shortest distance from source to destination over the answer's weights, or -1 when none reaches it.
    private static long DistanceOf(int nodeCount, int[][] answer, int source, int destination)
    {
        var nodes = Enumerable.Range(0, nodeCount).Select(id => new CheckedNode(id)).ToArray();

        foreach (var edge in answer)
        {
            nodes[edge[0]].Edges.Add((edge[2], nodes[edge[1]]));
            nodes[edge[1]].Edges.Add((edge[2], nodes[edge[0]]));
        }

        var distances = ShortestPath.Dijkstra<CheckedNode, CheckedTopology, ListEdges<CheckedNode, long>, long>(nodes[source]);

        return distances.GetValueOrDefault(nodes[destination], -1);
    }
}
