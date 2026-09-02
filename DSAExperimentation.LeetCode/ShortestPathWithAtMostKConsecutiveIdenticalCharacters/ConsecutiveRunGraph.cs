namespace DSAExperimentation.LeetCode.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

// LC 3970's own state expansion: crossing edge (u, v, w) extends the trailing
// run of identical labels by one when labels[u] == labels[v], otherwise resets
// it to 1, and the crossing is only legal while that run stays within k. Every
// (node, runLength) pair the original graph can be in becomes one
// ConsecutiveRunNode, wired up front over the full n * k state space - the same
// "domain model, not an answer to one query about it" framing LockGraph and
// RecoveryNetwork both use for their own expanded graphs.
internal sealed class ConsecutiveRunGraph
{
    private ConsecutiveRunGraph(ConsecutiveRunNode[,] states, int k)
    {
        States = states;
        K = k;
    }

    // [nodeId, runLength - 1].
    public ConsecutiveRunNode[,] States { get; }

    public int K { get; }

    public int NodeCount => States.GetLength(0);

    // Every path starts at node 0 with a run of exactly 1 (its own label,
    // counted once).
    public ConsecutiveRunNode Source => States[0, 0];

    public static ConsecutiveRunGraph Build(int n, int[][] edges, string labels, int k)
    {
        var states = BuildStates(n, k);

        WireEdges(states, edges, labels, k);

        return new ConsecutiveRunGraph(states, k);
    }

    private static ConsecutiveRunNode[,] BuildStates(int n, int k)
    {
        var states = new ConsecutiveRunNode[n, k];

        for (var node = 0; node < n; node++)
        {
            for (var run = 1; run <= k; run++)
            {
                states[node, run - 1] = new ConsecutiveRunNode(node, run);
            }
        }

        return states;
    }

    private static void WireEdges(ConsecutiveRunNode[,] states, int[][] edges, string labels, int k)
    {
        foreach (var edge in edges)
        {
            var (u, v, weight) = (edge[0], edge[1], (long)edge[2]);

            for (var run = 1; run <= k; run++)
            {
                var nextRun = labels[u] == labels[v] ? run + 1 : 1;

                if (nextRun > k)
                {
                    continue;
                }

                states[u, run - 1].Edges.Add((weight, states[v, nextRun - 1]));
            }
        }
    }
}
