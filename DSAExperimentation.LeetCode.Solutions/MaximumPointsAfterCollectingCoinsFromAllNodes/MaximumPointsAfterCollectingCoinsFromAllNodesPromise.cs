namespace DSAExperimentation.LeetCode.Solutions.MaximumPointsAfterCollectingCoinsFromAllNodes;

// LC 2920's promise about its input: the edges join every node into one tree. LeetCode's judge
// never poses an input that breaks it. The tree fold refuses edges that leave a node out. Generated
// cases do not always keep the promise, so a harness that runs them asks this first.
internal static class MaximumPointsAfterCollectingCoinsFromAllNodesPromise
{
    public static bool IsKeptBy(int[][] edges, int[] coins, int cost)
        => edges.Length == coins.Length - 1 && IsConnected(edges, coins.Length);

    // Every node reached from node 0 along the edges.
    private static bool IsConnected(int[][] edges, int nodeCount)
    {
        var neighbors = Enumerable.Range(0, nodeCount).Select(_ => new List<int>()).ToArray();

        foreach (var edge in edges.Where(edge => edge.All(node => node >= 0 && node < nodeCount)))
        {
            neighbors[edge[0]].Add(edge[1]);
            neighbors[edge[1]].Add(edge[0]);
        }

        var reached = new HashSet<int> { 0 };
        var pending = new Queue<int>(reached);

        while (pending.Count > 0)
        {
            foreach (var next in neighbors[pending.Dequeue()].Where(reached.Add))
            {
                pending.Enqueue(next);
            }
        }

        return reached.Count == nodeCount;
    }
}
