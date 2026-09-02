using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;


namespace DSAExperimentation.LeetCode.MaximumPointsAfterCollectingCoinsFromAllNodes;

// LeetCode 2920. Maximum Points After Collecting Coins From All Nodes: edges[]
// describes an undirected tree rooted at node 0. At every node you either collect
// normally - coins[node] >> h, minus a flat cost k, where h counts how many
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
    public static long MaxPointsByMemoizedRecursion(int[][] edges, int[] coins, int k)
    {
        var adjacency = BuildAdjacency(coins.Length, edges);
        var memo = new Dictionary<(int Node, int Halvings), long>();

        return Dfs(0, -1, 0);

        long Dfs(int node, int parent, int halvings)
        {
            var cappedHalvings = Math.Min(halvings, CoinPointsAlgebra.MaxHalvings);

            if (memo.TryGetValue((node, cappedHalvings), out var cached))
            {
                return cached;
            }

            var nextHalvings = Math.Min(cappedHalvings + 1, CoinPointsAlgebra.MaxHalvings);
            var take = ((long)coins[node] >> cappedHalvings) - k;
            var halve = (long)coins[node] >> nextHalvings;

            foreach (var neighbor in adjacency[node])
            {
                if (neighbor == parent)
                {
                    continue;
                }

                take += Dfs(neighbor, node, cappedHalvings);
                halve += Dfs(neighbor, node, nextHalvings);
            }

            var best = Math.Max(take, halve);
            memo[(node, cappedHalvings)] = best;
            return best;
        }
    }

    // Composed: edges[] converted to a parent array via a BFS over this repo's own
    // Queue<int>, materialized as DataStructures' RootedTreeNode/RootedTreeTopology
    // (the same parent-array tree CountWaysToBuildRoomsInAnAntColonySolution builds),
    // then folded bottom-up with TreeFold closed over CoinPointsAlgebra - each
    // node's Combine produces its whole per-halving-level table in one pass, so no
    // memo table is needed at all: the fold's own post-order visit reaches every
    // node exactly once.
    public static long MaxPointsByTreeFold(int[][] edges, int[] coins, int k)
    {
        var parent = BuildParentArray(coins.Length, edges);
        var nodes = ParentArrayTree.Build(parent);

        CoinPointsAlgebra.Prepare(coins, k);

        var table = TreeFold.Fold<
            RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
            NaturalChildOrder<RootedTreeNode, ListChildren<RootedTreeNode>>, ListChildren<RootedTreeNode>,
            CoinPointsAlgebra, long[]>(nodes[0]);

        return table[0];
    }

    private static List<int>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<int>[n];
        for (var i = 0; i < n; i++)
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

    // edges[] is undirected, so a BFS from the root is what turns it into the
    // parent-points-at-child encoding ParentArrayTree.Build expects.
    private static int[] BuildParentArray(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);
        var parent = new int[n];
        Array.Fill(parent, NoParentAssignedYet);
        parent[0] = -1;

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

        return parent;
    }
}
