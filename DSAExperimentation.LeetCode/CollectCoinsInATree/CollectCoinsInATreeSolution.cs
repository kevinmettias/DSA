using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.CollectCoinsInATree;

// LeetCode 2603. Collect Coins in a Tree: starting anywhere, collect every coin
// (coins within distance 2 of the current vertex are free) and return to the start,
// counting each edge traversal separately.
//
// The editorial reduction, shared by both strategies: trim leaves with no coin to a
// fixed point (a coin-less dead end is never worth walking into), then trim two more
// leaf layers unconditionally - the radius-2 collection rule means whatever leaves
// survive the coin-aware trim are still within 2 hops of an inner node once that
// inner node is visited, so the outermost two layers are always collected for free.
// The answer is twice whatever tree remains, since every surviving edge is walked
// out and back.
//
// The two strategies differ only in how the first, coin-aware round finds its
// removable frontier.
internal static class CollectCoinsInATreeSolution
{
    // The radius-2 collection rule: two unconditional leaf layers account for the
    // "free within 2 hops" allowance.
    private const int FreeLeafLayers = 2;

    // Every surviving edge is walked out and back.
    private const int TraversalsPerEdge = 2;

    // The textbook answer: no queue at all - repeatedly rescan every node for a
    // removable zero-coin leaf until a full pass finds none (O(n) per round, up to
    // O(n) rounds on a path-shaped tree). Deliberately written without this repo's
    // primitives - it is the arm the queue-driven peel below has to justify itself
    // against.
    public static int MinEdgesByRescan(int[][] edges, int[] coins)
    {
        var adjacency = BuildAdjacency(coins.Length, edges);

        return MinEdgesByRescan(adjacency, coins);
    }

    public static int MinEdgesByRescan(List<int>[] adjacency, int[] coins)
    {
        if (adjacency.Length <= 1)
        {
            return 0;
        }

        var trim = LeafTrim.Over(adjacency);
        TrimZeroCoinLeavesByRescanning(trim, coins);

        return RemainingRoundTrip(trim);
    }

    // This repo's own Queue<int>: every zero-coin leaf is enqueued once and dequeued
    // once, so the fixed-point trim runs in one O(n) pass instead of rescanning every
    // surviving node on every round. Same "peel the degree-1 frontier with a queue"
    // shape MinimumHeightTrees uses for its centroid search and TopologicalSort's
    // Kahn's algorithm uses for in-degree - just undirected degree, gated by a coin
    // predicate for this first round only.
    public static int MinEdgesByLeafQueue(int[][] edges, int[] coins)
    {
        var adjacency = BuildAdjacency(coins.Length, edges);

        return MinEdgesByLeafQueue(adjacency, coins);
    }

    public static int MinEdgesByLeafQueue(List<int>[] adjacency, int[] coins)
    {
        if (adjacency.Length <= 1)
        {
            return 0;
        }

        var trim = LeafTrim.Over(adjacency);
        TrimZeroCoinLeavesByQueue(trim, coins);

        return RemainingRoundTrip(trim);
    }

    private static int RemainingRoundTrip(LeafTrim trim)
    {
        for (var layer = 0; layer < FreeLeafLayers; layer++)
        {
            TrimLeafLayerUnconditionally(trim);
        }

        return TraversalsPerEdge * RemainingEdgeCount(trim);
    }

    private static void TrimZeroCoinLeavesByRescanning(LeafTrim trim, int[] coins)
    {
        bool removedAny;

        do
        {
            removedAny = false;

            for (var node = 0; node < trim.Adjacency.Length; node++)
            {
                if (IsRemovableCoinlessLeaf(trim, coins, node))
                {
                    RemoveNode(trim, node);
                    removedAny = true;
                }
            }
        } while (removedAny);
    }

    // A node still in the tree that carries no coin and has exactly one surviving
    // neighbour is a dead end: the walk never needs to reach past it.
    private static bool IsRemovableCoinlessLeaf(LeafTrim trim, int[] coins, int node) =>
        !trim.Removed[node] && trim.Degree[node] == 1 && coins[node] == 0;

    private static void TrimZeroCoinLeavesByQueue(LeafTrim trim, int[] coins)
    {
        var queue = SeedZeroCoinLeafQueue(trim, coins);

        PeelZeroCoinLeaves(trim, coins, queue);
    }

    // Every zero-coin leaf the tree starts with is removed and handed back in the queue
    // the peel below drains.
    private static RepoQueue SeedZeroCoinLeafQueue(LeafTrim trim, int[] coins)
    {
        var queue = new RepoQueue();

        for (var node = 0; node < trim.Adjacency.Length; node++)
        {
            if (trim.Degree[node] == 1 && coins[node] == 0)
            {
                trim.Removed[node] = true;
                queue.Enqueue(node);
            }
        }

        return queue;
    }

    // Each dequeue drops one neighbour's degree; a neighbour that becomes a coinless
    // leaf is itself removable, so it joins the frontier exactly once.
    private static void PeelZeroCoinLeaves(LeafTrim trim, int[] coins, RepoQueue queue)
    {
        while (queue.TryDequeue(out var node))
        {
            foreach (var neighbor in trim.Adjacency[node])
            {
                if (trim.Removed[neighbor])
                {
                    continue;
                }

                trim.Degree[neighbor]--;

                if (trim.Degree[neighbor] == 1 && coins[neighbor] == 0)
                {
                    trim.Removed[neighbor] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private static void TrimLeafLayerUnconditionally(LeafTrim trim)
    {
        var leaves = new List<int>();

        for (var node = 0; node < trim.Adjacency.Length; node++)
        {
            if (!trim.Removed[node] && trim.Degree[node] <= 1)
            {
                leaves.Add(node);
            }
        }

        foreach (var leaf in leaves)
        {
            RemoveNode(trim, leaf);
        }
    }

    private static void RemoveNode(LeafTrim trim, int node)
    {
        trim.Removed[node] = true;

        foreach (var neighbor in trim.Adjacency[node])
        {
            if (!trim.Removed[neighbor])
            {
                trim.Degree[neighbor]--;
            }
        }
    }

    private static int RemainingEdgeCount(LeafTrim trim)
    {
        var endpoints = 0;

        for (var node = 0; node < trim.Adjacency.Length; node++)
        {
            if (trim.Removed[node])
            {
                continue;
            }

            foreach (var neighbor in trim.Adjacency[node])
            {
                if (!trim.Removed[neighbor])
                {
                    endpoints++;
                }
            }
        }

        // Each surviving edge is seen from both of its endpoints.
        return endpoints / 2;
    }

    private static List<int>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<int>[n];

        for (var node = 0; node < n; node++)
        {
            adjacency[node] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        return adjacency;
    }

    // The per-run scratch the peel mutates. The adjacency lists themselves are never
    // written to, so a caller that hands the same prepared tree to both strategies
    // gets a fresh degree/removed pair each time rather than a tree one arm already
    // ate.
    private readonly record struct LeafTrim(List<int>[] Adjacency, int[] Degree, bool[] Removed)
    {
        public static LeafTrim Over(List<int>[] adjacency)
        {
            var degree = new int[adjacency.Length];

            for (var node = 0; node < adjacency.Length; node++)
            {
                degree[node] = adjacency[node].Count;
            }

            return new LeafTrim(adjacency, degree, new bool[adjacency.Length]);
        }
    }
}
