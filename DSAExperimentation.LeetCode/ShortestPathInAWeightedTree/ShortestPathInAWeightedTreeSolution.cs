using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.RangeFenwickTree;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.ShortestPathInAWeightedTree;

// LeetCode 3515. Shortest Path in a Weighted Tree: an undirected weighted tree
// rooted at node 1. A [1, u, v, w'] query changes one edge's weight; a [2, x]
// query asks the current path distance from the root to x. Return the answers
// to every [2, x] query in order.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class ShortestPathInAWeightedTreeSolution
{
    private const int NoParentAssignedYet = -2;

    // Textbook: a mutable BCL adjacency map, walked with a fresh BCL BFS from the
    // root for every [2, x] query. O(n) per query, O(n * q) overall - the arm the
    // Euler-tour Fenwick sweep below has to beat.
    public static int[] ShortestPathQueriesByBruteForceBfs(int nodeCount, int[][] edges, int[][] queries)
    {
        var adjacency = new List<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            adjacency[i] = [];
        }

        var weight = new Dictionary<(int From, int To), int>();

        AddUndirectedEdges(adjacency, weight, edges);

        return [.. AnswerQueriesByBfs(nodeCount, adjacency, weight, queries)];
    }

    // Both endpoints must learn the edge: the map stores it under (u, v) and its
    // reverse so a lookup from either end finds the same weight.
    private static void AddUndirectedEdges(
        List<int>[] adjacency, Dictionary<(int From, int To), int> weight, int[][] edges)
    {
        foreach (var edge in edges)
        {
            var (u, v, w) = (edge[0] - 1, edge[1] - 1, edge[2]);
            adjacency[u].Add(v);
            adjacency[v].Add(u);
            weight[(u, v)] = w;
            weight[(v, u)] = w;
        }
    }

    // A type-1 query mutates the weight map in place, so the answers are produced
    // in one pass: every later query sees the updates of the earlier ones.
    private static List<int> AnswerQueriesByBfs(
        int nodeCount, List<int>[] adjacency, Dictionary<(int From, int To), int> weight, int[][] queries)
    {
        var answers = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == 1)
            {
                var (u, v, w) = (query[1] - 1, query[2] - 1, query[3]);
                weight[(u, v)] = w;
                weight[(v, u)] = w;
            }
            else
            {
                var distance = DistanceFromRootByBfs(nodeCount, adjacency, weight, query[1] - 1);
                answers.Add(distance);
            }
        }

        return answers;
    }

    private static int DistanceFromRootByBfs(
        int nodeCount, List<int>[] adjacency, Dictionary<(int From, int To), int> weight, int target)
    {
        var distance = new int[nodeCount];
        Array.Fill(distance, -1);
        distance[0] = 0;

        PropagateDistances(adjacency, weight, distance);

        return distance[target];
    }

    // A BFS from the root: the first time a node is reached is along a shortest
    // path, so its distance is already final and it is never relaxed again.
    private static void PropagateDistances(
        List<int>[] adjacency, Dictionary<(int From, int To), int> weight, int[] distance)
    {
        var queue = new Queue<int>();
        queue.Enqueue(0);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var next in adjacency[node])
            {
                if (distance[next] != -1)
                {
                    continue;
                }

                distance[next] = distance[node] + weight[(node, next)];
                queue.Enqueue(next);
            }
        }
    }

    // Composed: edges[] becomes a parent array via a BFS over this repo's own
    // Queue<int> (the same conversion MaximumPointsAfterCollectingCoinsFromAll-
    // NodesSolution uses), materialized as DataStructures' RootedTreeNode via
    // ParentArrayTree.Build, then laid out by DataStructures' PreOrderTour: every
    // subtree is one contiguous run of positions, SubtreeOf(v), and the tour's
    // ParentOf names which endpoint of an edge hangs below the other.
    //
    // Node v's distance from the root is the sum of every ancestor edge's
    // weight, and updating one edge changes that sum for exactly the subtree
    // below it - a range-add. This repo's own RangeFenwickTree<long,
    // SumOperation<long>> (RangeFenwickTree.cs's own doc comment: "a point
    // query is just Query(i, i)") is exactly a range-add/point-query structure,
    // so each edge's weight is posted once as a RangeAdd over its child's
    // subtree, an update re-posts the delta over the same range, and a [2, x]
    // query is one point query at x's position. O((n + q) log n).
    public static int[] ShortestPathQueriesByEulerFenwick(int nodeCount, int[][] edges, int[][] queries)
    {
        var state = BuildEulerSweepState(nodeCount, edges);

        return [.. AnswerQueriesByEulerSweep(state, queries)];
    }

    // The Euler-tour sweep's whole working set, built once from edges[] and then
    // walked by the query loop: the tour names which edge a node hangs from and
    // the range below it, CurrentWeight is what was last posted for each edge,
    // and Fenwick is the range-add / point-query structure those weights live in.
    private static (
        PreOrderTour Tour,
        int[] CurrentWeight,
        RangeFenwickTree<long, SumOperation<long>> Fenwick) BuildEulerSweepState(int nodeCount, int[][] edges)
    {
        var (parent, parentWeight) = BuildParentArrays(nodeCount, edges);
        var nodes = ParentArrayTree.Build(parent);
        var tour = PreOrderTour.Build(nodes[0], nodeCount);
        var (currentWeight, fenwick) = PostInitialEdgeWeights(parentWeight, tour);

        return (tour, currentWeight, fenwick);
    }

    // edges[] is undirected, so a BFS from the root turns it into the
    // parent-points-at-child encoding ParentArrayTree.Build expects, carrying
    // each child's edge weight alongside its parent pointer.
    private static (int[] Parent, int[] ParentWeight) BuildParentArrays(int nodeCount, int[][] edges)
    {
        var adjacency = BuildWeightedAdjacency(nodeCount, edges);

        var parent = new int[nodeCount];
        var parentWeight = new int[nodeCount];
        Array.Fill(parent, NoParentAssignedYet);
        parent[0] = -1;

        AssignParentsByBfs(adjacency, parent, parentWeight);

        return (parent, parentWeight);
    }

    // An edge is undirected, so each endpoint records the other with the weight.
    private static List<(int To, int Weight)>[] BuildWeightedAdjacency(int nodeCount, int[][] edges)
    {
        var adjacency = new List<(int To, int Weight)>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            var (u, v, w) = (edge[0] - 1, edge[1] - 1, edge[2]);
            adjacency[u].Add((v, w));
            adjacency[v].Add((u, w));
        }

        return adjacency;
    }

    // A BFS from the root gives every node its parent and the weight of the edge
    // between them; NoParentAssignedYet marks "not reached yet", so the -1 the
    // caller seeded on the root is what stops the walk from turning back into it.
    private static void AssignParentsByBfs(
        List<(int To, int Weight)>[] adjacency, int[] parent, int[] parentWeight)
    {
        var queue = new RepoQueue();
        queue.Enqueue(0);

        while (queue.TryDequeue(out var current))
        {
            foreach (var (next, weight) in adjacency[current])
            {
                if (parent[next] != NoParentAssignedYet)
                {
                    continue;
                }

                parent[next] = current;
                parentWeight[next] = weight;
                queue.Enqueue(next);
            }
        }
    }

    // Every edge's weight is posted once as a range-add over the child's subtree:
    // that is what makes an update a delta over the same range and a [2, x] query
    // a single point read. CurrentWeight records what was posted, so an update
    // can post only the difference. Node 0 is the root and hangs from no edge.
    private static (int[] CurrentWeight, RangeFenwickTree<long, SumOperation<long>> Fenwick)
        PostInitialEdgeWeights(int[] parentWeight, PreOrderTour tour)
    {
        var fenwick = new RangeFenwickTree<long, SumOperation<long>>(tour.NodeCount);
        var currentWeight = new int[tour.NodeCount];

        for (var id = 1; id < tour.NodeCount; id++)
        {
            var (first, last) = tour.SubtreeOf(id);
            currentWeight[id] = parentWeight[id];
            fenwick.RangeAdd(first, last, parentWeight[id]);
        }

        return (currentWeight, fenwick);
    }

    private static List<int> AnswerQueriesByEulerSweep(
        (PreOrderTour Tour, int[] CurrentWeight, RangeFenwickTree<long, SumOperation<long>> Fenwick) state,
        int[][] queries)
    {
        var answers = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == 1)
            {
                var (u, v, w) = (query[1] - 1, query[2] - 1, query[3]);
                var uIsChild = state.Tour.ParentOf(u) == v;
                RepostEdgeWeight(state, uIsChild ? u : v, w);
            }
            else
            {
                var position = state.Tour.PositionOf(query[1] - 1);
                answers.Add((int)state.Fenwick.Query(position, position));
            }
        }

        return answers;
    }

    // A [1, u, v, w'] query lands on the edge above child, the endpoint whose parent
    // is the other: the new weight's difference from the posted one is re-posted
    // over child's subtree, and the new weight becomes the posted one.
    private static void RepostEdgeWeight(
        (PreOrderTour Tour, int[] CurrentWeight, RangeFenwickTree<long, SumOperation<long>> Fenwick) state,
        int child,
        int weight)
    {
        var delta = weight - state.CurrentWeight[child];
        var (first, last) = state.Tour.SubtreeOf(child);

        state.Fenwick.RangeAdd(first, last, delta);
        state.CurrentWeight[child] = weight;
    }
}
