using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.RangeFenwickTree;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.RootedTreeNode>;

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
    public static int[] ShortestPathQueriesByBruteForceBfs(int n, int[][] edges, int[][] queries)
    {
        var adjacency = new List<int>[n];

        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        var weight = new Dictionary<(int From, int To), int>();

        AddUndirectedEdges(adjacency, weight, edges);

        return [.. AnswerQueriesByBfs(n, adjacency, weight, queries)];
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
        int n, List<int>[] adjacency, Dictionary<(int From, int To), int> weight, int[][] queries)
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
                var distance = DistanceFromRootByBfs(n, adjacency, weight, query[1] - 1);
                answers.Add(distance);
            }
        }

        return answers;
    }

    private static int DistanceFromRootByBfs(
        int n, List<int>[] adjacency, Dictionary<(int From, int To), int> weight, int target)
    {
        var distance = new int[n];
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
    // ParentArrayTree.Build, then walked with this repo's own Stack<RootedTree-
    // Node> for an iterative pre-order Euler tour: TimeIn[v] is v's visit order,
    // and because a stack-based pre-order never starts a sibling subtree before
    // finishing the current one, [TimeIn[v], TimeOut[v]] is exactly the
    // contiguous range of every node in v's subtree.
    //
    // Node v's distance from the root is the sum of every ancestor edge's
    // weight, and updating one edge changes that sum for exactly the subtree
    // below it - a range-add. This repo's own RangeFenwickTree<long,
    // ScaledSumOperation<long>> (RangeFenwickTree.cs's own doc comment: "a point
    // query is just Query(i, i)") is exactly a range-add/point-query structure,
    // so each edge's weight is posted once as RangeAdd(TimeIn[child],
    // TimeOut[child], weight), an update re-posts the delta over the same range,
    // and a [2, x] query is one point query at TimeIn[x]. O((n + q) log n).
    public static int[] ShortestPathQueriesByEulerFenwick(int n, int[][] edges, int[][] queries)
    {
        var state = BuildEulerSweepState(n, edges);

        return [.. AnswerQueriesByEulerSweep(state, queries)];
    }

    // The Euler-tour sweep's whole working set, built once from edges[] and then
    // walked by the query loop: parent pointers and subtree ranges name which
    // edge a node hangs from, CurrentWeight is what was last posted for it, and
    // Fenwick is the range-add / point-query structure those weights live in.
    private static (
        int[] Parent,
        int[] TimeIn,
        int[] TimeOut,
        int[] CurrentWeight,
        RangeFenwickTree<long, ScaledSumOperation<long>> Fenwick) BuildEulerSweepState(int n, int[][] edges)
    {
        var (parent, parentWeight) = BuildParentArrays(n, edges);
        var nodes = ParentArrayTree.Build(parent);
        var (timeIn, timeOut) = BuildEulerTour(nodes[0], parent, n);
        var (currentWeight, fenwick) = PostInitialEdgeWeights(n, parentWeight, timeIn, timeOut);

        return (parent, timeIn, timeOut, currentWeight, fenwick);
    }

    // edges[] is undirected, so a BFS from the root turns it into the
    // parent-points-at-child encoding ParentArrayTree.Build expects, carrying
    // each child's edge weight alongside its parent pointer.
    private static (int[] Parent, int[] ParentWeight) BuildParentArrays(int n, int[][] edges)
    {
        var adjacency = BuildWeightedAdjacency(n, edges);

        var parent = new int[n];
        var parentWeight = new int[n];
        Array.Fill(parent, NoParentAssignedYet);
        parent[0] = -1;

        AssignParentsByBfs(adjacency, parent, parentWeight);

        return (parent, parentWeight);
    }

    // An edge is undirected, so each endpoint records the other with the weight.
    private static List<(int To, int Weight)>[] BuildWeightedAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<(int To, int Weight)>[n];

        for (var i = 0; i < n; i++)
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

    private static (int[] TimeIn, int[] TimeOut) BuildEulerTour(RootedTreeNode root, int[] parent, int n)
    {
        var (timeIn, visitOrder) = TraversePreOrder(root, n);
        var timeOut = ComputeSubtreeEnds(parent, n, timeIn, visitOrder);

        return (timeIn, timeOut);
    }

    // An iterative pre-order walk of root's RootedTreeNode.Children (repo Stack,
    // not recursion - LC's own worst case is a straight-line chain of up to 1e5
    // nodes, deep enough to risk overflowing the call stack). Because a stack
    // based pre-order never starts a sibling subtree before finishing the
    // current one, [TimeIn[v], TimeOut[v]] is exactly the contiguous range of
    // every node in v's subtree.
    private static (int[] TimeIn, int[] VisitOrder) TraversePreOrder(RootedTreeNode root, int n)
    {
        var timeIn = new int[n];
        var visitOrder = new int[n];
        var stack = new RepoStack();
        stack.Push(root);
        var timer = 0;

        while (stack.TryPop(out var node))
        {
            timeIn[node.Id] = timer;
            visitOrder[timer] = node.Id;
            timer++;

            foreach (var child in node.Children)
            {
                stack.Push(child);
            }
        }

        return (timeIn, visitOrder);
    }

    // Subtree size is read off in one reverse pass over that same pre-order:
    // every node's entire subtree already precedes it there, so accumulating
    // each node's size into its parent's needs no second traversal.
    private static int[] ComputeSubtreeEnds(int[] parent, int n, int[] timeIn, int[] visitOrder)
    {
        var size = new int[n];
        Array.Fill(size, 1);

        for (var i = n - 1; i >= 1; i--)
        {
            var id = visitOrder[i];
            size[parent[id]] += size[id];
        }

        var timeOut = new int[n];

        for (var id = 0; id < n; id++)
        {
            timeOut[id] = timeIn[id] + size[id] - 1;
        }

        return timeOut;
    }

    // Every edge's weight is posted once as a range-add over the child's subtree:
    // that is what makes an update a delta over the same range and a [2, x] query
    // a single point read. CurrentWeight records what was posted, so an update
    // can post only the difference.
    private static (int[] CurrentWeight, RangeFenwickTree<long, ScaledSumOperation<long>> Fenwick)
        PostInitialEdgeWeights(int n, int[] parentWeight, int[] timeIn, int[] timeOut)
    {
        var fenwick = new RangeFenwickTree<long, ScaledSumOperation<long>>(n);
        var currentWeight = new int[n];

        for (var id = 1; id < n; id++)
        {
            currentWeight[id] = parentWeight[id];
            fenwick.RangeAdd(timeIn[id], timeOut[id], parentWeight[id]);
        }

        return (currentWeight, fenwick);
    }

    private static List<int> AnswerQueriesByEulerSweep(
        (int[] Parent, int[] TimeIn, int[] TimeOut, int[] CurrentWeight,
            RangeFenwickTree<long, ScaledSumOperation<long>> Fenwick) state, int[][] queries)
    {
        var answers = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == 1)
            {
                var (u, v, w) = (query[1] - 1, query[2] - 1, query[3]);
                var uIsChild = state.Parent[u] == v;
                var child = uIsChild ? u : v;
                var delta = w - state.CurrentWeight[child];

                state.Fenwick.RangeAdd(state.TimeIn[child], state.TimeOut[child], delta);
                state.CurrentWeight[child] = w;
            }
            else
            {
                var x = query[1] - 1;
                answers.Add((int)state.Fenwick.Query(state.TimeIn[x], state.TimeIn[x]));
            }
        }

        return answers;
    }
}
