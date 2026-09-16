namespace DSAExperimentation.LeetCode.MinimumTimeToReachTargetWithLimitedPower;

// LC 3977's own state expansion: forwarding the signal from node u always costs
// exactly cost[u], regardless of which outgoing edge is taken, so a state is
// (node, remainingPower) and every edge (u, v, t) becomes one transition per
// power level u can still afford it from. Every reachable-by-construction
// (node, remainingPower) pair becomes one PowerStateNode, wired up front over
// the full n * (power + 1) state space - the same "domain model, not an answer
// to one query about it" framing LockGraph and RecoveryNetwork both use for
// their own expanded graphs.
internal sealed class PowerStateGraph
{
    // [nodeId, remainingPower].
    public PowerStateNode[,] States { get; }

    public int Power { get; }

    public int NodeCount => States.GetLength(0);

    private PowerStateGraph(PowerStateNode[,] states, int power)
    {
        States = states;
        Power = power;
    }

    public PowerStateNode SourceState(int source) => States[source, Power];

    public static PowerStateGraph Build(int nodeCount, int[][] edges, int power, int[] cost)
    {
        var states = BuildStates(nodeCount, power);

        WireEdges(states, edges, power, cost);

        return new PowerStateGraph(states, power);
    }

    private static PowerStateNode[,] BuildStates(int nodeCount, int power)
    {
        var states = new PowerStateNode[nodeCount, power + 1];

        for (var node = 0; node < nodeCount; node++)
        {
            for (var remaining = 0; remaining <= power; remaining++)
            {
                states[node, remaining] = new PowerStateNode(node, remaining);
            }
        }

        return states;
    }

    private static void WireEdges(PowerStateNode[,] states, int[][] edges, int power, int[] cost)
    {
        foreach (var edge in edges)
        {
            var (u, v, weight) = (edge[0], edge[1], (long)edge[2]);

            for (var remaining = cost[u]; remaining <= power; remaining++)
            {
                states[u, remaining].Edges.Add((weight, states[v, remaining - cost[u]]));
            }
        }
    }
}
