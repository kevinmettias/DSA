namespace DSAExperimentation.LeetCode.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

// LC 3970's own state expansion: crossing edge (u, v, w) extends the trailing
// run of identical labels by one when labels[u] == labels[v], otherwise resets
// it to 1, and the crossing is only legal while that run stays within
// maxRunLength. Every (node, runLength) pair the original graph can be in
// becomes one ConsecutiveRunNode, wired up front over the full
// nodeCount * maxRunLength state space - the same "domain model, not an answer
// to one query about it" framing LockGraph and RecoveryNetwork both use for
// their own expanded graphs.
internal sealed class ConsecutiveRunGraph
{
    // [nodeId, runLength - 1].
    public ConsecutiveRunNode[,] States { get; }

    public int MaxRunLength { get; }

    public int NodeCount => States.GetLength(0);

    // Every path starts at node 0 with a run of exactly 1 (its own label,
    // counted once).
    public ConsecutiveRunNode Source => States[0, 0];

    private ConsecutiveRunGraph(ConsecutiveRunNode[,] states, int maxRunLength)
    {
        States = states;
        MaxRunLength = maxRunLength;
    }

    public static ConsecutiveRunGraph Build(int nodeCount, int[][] edges, string labels, int maxRunLength)
    {
        var states = BuildStates(nodeCount, maxRunLength);

        WireEdges(states, edges, labels, maxRunLength);

        return new ConsecutiveRunGraph(states, maxRunLength);
    }

    private static ConsecutiveRunNode[,] BuildStates(int nodeCount, int maxRunLength)
    {
        var states = new ConsecutiveRunNode[nodeCount, maxRunLength];

        for (var node = 0; node < nodeCount; node++)
        {
            for (var run = 1; run <= maxRunLength; run++)
            {
                states[node, run - 1] = new ConsecutiveRunNode(node, run);
            }
        }

        return states;
    }

    private static void WireEdges(
        ConsecutiveRunNode[,] states, int[][] edges, string labels, int maxRunLength)
    {
        foreach (var edge in edges)
        {
            var (u, v, weight) = (edge[0], edge[1], (long)edge[2]);

            for (var run = 1; run <= maxRunLength; run++)
            {
                var nextRun = HasEqualLabels(labels, u, v) ? ExtendedRun(run) : 1;

                if (nextRun > maxRunLength)
                {
                    continue;
                }

                states[u, run - 1].Edges.Add((weight, states[v, nextRun - 1]));
            }
        }
    }

    // Crossing an edge whose endpoints share a label lengthens the trailing run;
    // any other edge resets it.
    private static bool HasEqualLabels(string labels, int sourceNode, int targetNode) =>
        labels[sourceNode] == labels[targetNode];

    private static int ExtendedRun(int run) => run + 1;
}
