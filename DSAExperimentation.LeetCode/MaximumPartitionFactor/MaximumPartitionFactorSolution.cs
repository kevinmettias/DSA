using DSAExperimentation.Algorithms.Bipartiteness;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MaximumPartitionFactor;

// LeetCode 3710. Maximum Partition Factor: split n points into exactly two
// non-empty groups, maximizing the minimum Manhattan distance among pairs that
// land in the SAME group.
//
// A candidate factor F is achievable iff the "too close" graph - an edge between
// every pair of points less than F apart - is bipartite: two points an edge
// forces apart simply land in opposite groups (and n >= 3 guarantees both final
// groups end up non-empty, since a bipartite graph's own two color classes, or
// any pair of never-connected points when there are no edges at all, already
// give two non-empty sides). Feasibility is monotonic in F - raising F can only
// add edges to that graph, and adding edges can only break bipartiteness, never
// restore it - so the answer is the LARGEST of the input's own pairwise
// distances at which the graph is still bipartite (the optimal split's tightest
// same-group pair is always one of those distances). n == 2 is a standalone
// special case LeetCode defines as 0: two singleton groups have no same-group
// pair at all.
internal static class MaximumPartitionFactorSolution
{
    private const int TrivialPairFactor = 0;

    // The textbook answer: scan every distinct pairwise distance from largest to
    // smallest, rebuilding a plain BCL adjacency list and BFS-2-coloring it from
    // scratch at each candidate, stopping at the first one that is bipartite.
    // Deliberately written without this repo's primitives - the arm the binary
    // search below has to justify itself against.
    public static int MaxPartitionFactorByLinearScan(int[][] points)
    {
        if (points.Length == 2)
        {
            return TrivialPairFactor;
        }

        foreach (var candidate in DistinctPairwiseDistances(points, DistanceOrder.Descending))
        {
            if (IsBipartiteAtThreshold(points, candidate))
            {
                return candidate;
            }
        }

        return TrivialPairFactor;
    }

    // Binary search over the sorted distinct pairwise distances for the largest
    // feasible one, composing this repo's own Algorithms.Bipartiteness.BipartiteCheck
    // over a PartitionNode/PartitionTopology graph rebuilt for each candidate -
    // the same "Domain graph + repo traversal algorithm" composition
    // OpenTheLockSolution.MinTurnsByReduceGraph uses for LC 752.
    public static int MaxPartitionFactorByBinarySearchBipartiteCheck(int[][] points)
    {
        if (points.Length == 2)
        {
            return TrivialPairFactor;
        }

        var distances = DistinctPairwiseDistances(points, DistanceOrder.Ascending).ToArray();
        var nodes = points.Select(point => new PartitionNode(point[0], point[1])).ToList();

        return LargestFeasibleDistance(nodes, distances);
    }

    // The binary search itself: the distances arrive sorted ascending, so the
    // feasible ones form a prefix and the answer is its last element.
    private static int LargestFeasibleDistance(List<PartitionNode> nodes, int[] distances)
    {
        var low = 0;
        var high = distances.Length - 1;
        var best = TrivialPairFactor;

        while (low <= high)
        {
            var mid = low + ((high - low) / 2);

            if (IsBipartiteAtThreshold(nodes, distances[mid]))
            {
                best = distances[mid];
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        return best;
    }

    private static bool IsBipartiteAtThreshold(List<PartitionNode> nodes, int threshold)
    {
        RebuildConflictEdges(nodes, threshold);

        return BipartiteCheck.IsBipartite<
            PartitionNode, PartitionTopology, ListChildren<PartitionNode>,
            NaturalChildOrder<PartitionNode, ListChildren<PartitionNode>>, ListChildren<PartitionNode>>(nodes);
    }

    private static void RebuildConflictEdges(List<PartitionNode> nodes, int threshold)
    {
        foreach (var node in nodes)
        {
            node.Neighbors.Clear();
        }

        for (var i = 0; i < nodes.Count; i++)
        {
            for (var j = i + 1; j < nodes.Count; j++)
            {
                if (ManhattanDistance(nodes[i].X, nodes[i].Y, nodes[j].X, nodes[j].Y) < threshold)
                {
                    nodes[i].Neighbors.Add(nodes[j]);
                    nodes[j].Neighbors.Add(nodes[i]);
                }
            }
        }
    }

    private static bool IsBipartiteAtThreshold(int[][] points, int threshold)
    {
        var adjacency = new List<int>[points.Length];

        for (var i = 0; i < points.Length; i++)
        {
            adjacency[i] = [];
        }

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                if (ManhattanDistance(points[i][0], points[i][1], points[j][0], points[j][1]) < threshold)
                {
                    adjacency[i].Add(j);
                    adjacency[j].Add(i);
                }
            }
        }

        return IsBipartiteByBfsColoring(adjacency);
    }

    private static bool IsBipartiteByBfsColoring(List<int>[] adjacency)
    {
        var color = new int[adjacency.Length];
        Array.Fill(color, -1);

        for (var start = 0; start < adjacency.Length; start++)
        {
            if (!TryColorComponent(adjacency, color, start))
            {
                return false;
            }
        }

        return true;
    }

    // Colors every point reachable from `start`, reporting false as soon as one of
    // them is forced to take two colors at once. The caller's `continue` on an
    // already-colored start is this helper's `true`.
    private static bool TryColorComponent(List<int>[] adjacency, int[] color, int start)
    {
        if (color[start] != -1)
        {
            return true;
        }

        color[start] = 0;
        var frontier = new Queue<int>();
        frontier.Enqueue(start);

        while (frontier.Count > 0)
        {
            if (!TryColorNeighbors(adjacency, color, frontier))
            {
                return false;
            }
        }

        return true;
    }

    // One breadth-first step: paint the front node's neighbours that have no color
    // yet, and report the conflict when one already shares the node's color.
    private static bool TryColorNeighbors(List<int>[] adjacency, int[] color, Queue<int> frontier)
    {
        var node = frontier.Dequeue();

        foreach (var neighbor in adjacency[node])
        {
            if (color[neighbor] == -1)
            {
                color[neighbor] = 1 - color[node];
                frontier.Enqueue(neighbor);
            }
            else if (color[neighbor] == color[node])
            {
                return false;
            }
        }

        return true;
    }

    private static int ManhattanDistance(int x1, int y1, int x2, int y2) =>
        Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

    private static IEnumerable<int> DistinctPairwiseDistances(int[][] points, DistanceOrder order)
    {
        var distances = new HashSet<int>();

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var distance = ManhattanDistance(points[i][0], points[i][1], points[j][0], points[j][1]);
                distances.Add(distance);
            }
        }

        return order == DistanceOrder.Descending ? distances.OrderDescending() : distances.Order();
    }

    // Which end of the sorted distinct distances a caller wants first. Named where
    // a rewritten `true`/`false` at the call site said it only by position.
    private enum DistanceOrder
    {
        Descending,
        Ascending,
    }
}
