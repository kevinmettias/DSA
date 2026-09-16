using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;


namespace DSAExperimentation.LeetCode.MaximumPointsAfterCollectingCoinsFromAllNodes;

// LeetCode 2920. Maximum Points After Collecting Coins From All Nodes: edges[]
// describes an undirected tree rooted at node 0. At every node you either collect
// normally - coins[node] >> h, minus a flat cost, where h counts how many
// ancestors already chose to halve on the way down - or halve here too -
// coins[node] >> (h + 1), no cost, with h + 1 then flowing into every child.
// Maximize the total.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class MaximumPointsAfterCollectingCoinsFromAllNodesSolution
{
    private const int NoParentAssignedYet = -2;

    // Plain recursive memoized DP over a hand-rolled adjacency list -
    // Dictionary<(node,h),long> keyed lookups, no repo primitive involved beyond
    // the adjacency BFS's own Queue<int>. The arm the tree-fold strategy has to
    // beat.
    public static long MaxPointsByMemoizedRecursion(int[][] edges, int[] coins, int cost)
    {
        var adjacency = BuildAdjacency(coins.Length, edges);
        var memo = new Dictionary<(int Node, int Halvings), long>();
        var state = (Adjacency: adjacency, Coins: coins, Cost: cost, Memo: memo);

        return Dfs(state, 0, -1, 0);
    }

    // Composed: edges[] converted to a parent array via a BFS over this repo's own
    // Queue<int>, materialized as DataStructures' RootedTreeNode/RootedTreeTopology
    // (the same parent-array tree CountWaysToBuildRoomsInAnAntColonySolution builds),
    // then folded bottom-up with TreeFold closed over CoinPointsAlgebra - each
    // node's Combine produces its whole per-halving-level table in one pass, so no
    // memo table is needed at all: the fold's own post-order visit reaches every
    // node exactly once.
    public static long MaxPointsByTreeFold(int[][] edges, int[] coins, int cost)
    {
        var parent = BuildParentArray(coins.Length, edges);
        var nodes = ParentArrayTree.Build(parent);

        CoinPointsAlgebra.Prepare(coins, cost);

        var table = TreeFold.Fold<
            RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
            NaturalChildOrder<RootedTreeNode, ListChildren<RootedTreeNode>>, ListChildren<RootedTreeNode>,
            CoinPointsAlgebra, long[]>(nodes[0]);

        return table[0];
    }

    // edges[] is undirected, so a BFS from the root is what turns it into the
    // parent-points-at-child encoding ParentArrayTree.Build expects.
    private static int[] BuildParentArray(int nodeCount, int[][] edges)
    {
        var adjacency = BuildAdjacency(nodeCount, edges);
        var parent = new int[nodeCount];
        Array.Fill(parent, NoParentAssignedYet);
        parent[0] = -1;

        AssignParentsByBfs(adjacency, parent);

        return parent;
    }

    // A BFS from the root fills in parent[]; NoParentAssignedYet marks "not
    // reached yet", so the -1 the caller seeded on the root is what stops the
    // walk from turning back into it.
    private static void AssignParentsByBfs(List<int>[] adjacency, int[] parent)
    {
        var queue = new RepoQueue();
        queue.Enqueue(0);

        while (queue.TryDequeue(out var current))
        {
            foreach (var neighbor in adjacency[current])
            {
                if (parent[neighbor] != NoParentAssignedYet)
                {
                    continue;
                }

                parent[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }
    }

    // The memo table, the adjacency and the problem constants travel together,
    // so they are bundled rather than threaded one by one through the recursion.
    private static long Dfs(
        (List<int>[] Adjacency, int[] Coins, int Cost, Dictionary<(int Node, int Halvings), long> Memo) state,
        int node, int parent, int halvings)
    {
        var cappedHalvings = Math.Min(halvings, HalvingDepth.Max);

        if (state.Memo.TryGetValue((node, cappedHalvings), out var cached))
        {
            return cached;
        }

        var nextHalvings = Math.Min(cappedHalvings + 1, HalvingDepth.Max);
        var take = ((long)state.Coins[node] >> cappedHalvings) - state.Cost;
        var halve = (long)state.Coins[node] >> nextHalvings;

        var (takeDelta, halveDelta) = AccumulateChildBranches(state, node, parent, cappedHalvings);
        take += takeDelta;
        halve += halveDelta;

        var best = Math.Max(take, halve);
        state.Memo[(node, cappedHalvings)] = best;
        return best;
    }

    // Each child is asked for both of its branches - the one that keeps this
    // node's halving count and the one that increments it - and the two totals
    // are what Dfs adds to its own two candidates. Children are visited in
    // adjacency order so the memo writes land in the same sequence as before.
    private static (long Take, long Halve) AccumulateChildBranches(
        (List<int>[] Adjacency, int[] Coins, int Cost, Dictionary<(int Node, int Halvings), long> Memo) state,
        int node, int parent, int cappedHalvings)
    {
        var nextHalvings = Math.Min(cappedHalvings + 1, HalvingDepth.Max);
        long take = 0;
        long halve = 0;

        foreach (var neighbor in state.Adjacency[node])
        {
            if (neighbor == parent)
            {
                continue;
            }

            take += Dfs(state, neighbor, node, cappedHalvings);
            halve += Dfs(state, neighbor, node, nextHalvings);
        }

        return (take, halve);
    }

    private static List<int>[] BuildAdjacency(int nodeCount, int[][] edges)
    {
        var adjacency = new List<int>[nodeCount];
        for (var i = 0; i < nodeCount; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        return adjacency;
    }
}
