using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.CriticalConnectionsInANetwork;

// LeetCode 1192. Critical Connections in a Network: a connection is critical when
// removing it leaves some server unreachable - which is exactly the definition of
// a bridge in an undirected graph.
//
// The baseline takes that definition literally: drop each connection in turn and
// re-run a full BFS to see whether the network is still connected, O(E*(V+E)). The
// composed strategy hands the same question to this repo's low-link DFS
// (Algorithms.Connectivity.BridgesAndArticulationPoints.Find), which finds every
// bridge in one O(V+E) pass and reports articulation points alongside - only the
// Bridges half of its tuple is read here.
//
// LeetCode accepts the connections in any order, so each strategy reports each
// pair with its smaller server first and leaves the outer order to its own walk.
internal static class CriticalConnectionsInANetworkSolution
{
    // The textbook answer: rebuild a List<int>[] adjacency without one connection,
    // BFS from server 0 with a bool[] visited array, and call that connection
    // critical when the walk fails to reach all n servers. Deliberately written
    // without this repo's primitives - it is the arm the composed solution below
    // has to justify itself against. LeetCode's own int[][] is already the prepared
    // shape here, so this strategy needs no hoisted overload.
    public static int[][] CriticalConnectionsByEdgeRemovalScan(int serverCount, int[][] connections)
    {
        var critical = new List<int[]>();

        for (var i = 0; i < connections.Length; i++)
        {
            if (!IsConnectedWithoutConnection(serverCount, connections, i))
            {
                critical.Add(Pair(connections[i][0], connections[i][1]));
            }
        }

        return [.. critical];
    }

    private static bool IsConnectedWithoutConnection(int serverCount, int[][] connections, int skipIndex)
    {
        var adjacency = BuildAdjacencyWithoutConnection(serverCount, connections, skipIndex);
        var visited = new bool[serverCount];
        var queue = new Queue<int>();
        queue.Enqueue(0);
        visited[0] = true;
        var visitedCount = 1;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var neighbor in adjacency[current])
            {
                VisitNeighbor(neighbor, visited, queue, ref visitedCount);
            }
        }

        return visitedCount == serverCount;
    }

    private static void VisitNeighbor(int neighbor, bool[] visited, Queue<int> queue, ref int visitedCount)
    {
        if (visited[neighbor])
        {
            return;
        }

        visited[neighbor] = true;
        visitedCount++;
        queue.Enqueue(neighbor);
    }

    private static List<int>[] BuildAdjacencyWithoutConnection(
        int serverCount, int[][] connections, int skipIndex)
    {
        var adjacency = new List<int>[serverCount];

        for (var i = 0; i < serverCount; i++)
        {
            adjacency[i] = [];
        }

        for (var i = 0; i < connections.Length; i++)
        {
            if (i == skipIndex)
            {
                continue;
            }

            adjacency[connections[i][0]].Add(connections[i][1]);
            adjacency[connections[i][1]].Add(connections[i][0]);
        }

        return adjacency;
    }

    // This repo's own answer: BridgesAndArticulationPoints.Find is already the
    // discovery-index/low-link DFS that names every bridge in a single pass, so the
    // problem reduces to materializing the connection list as a ServerNetwork and
    // reading the Bridges half of the result - the same "graph + repo traversal
    // algorithm" composition IsGraphBipartiteSolution uses for LC 785.
    public static int[][] CriticalConnectionsByLowLinkSearch(int serverCount, int[][] connections)
    {
        var network = ServerNetwork.Build(serverCount, connections);

        return CriticalConnectionsByLowLinkSearch(network);
    }

    public static int[][] CriticalConnectionsByLowLinkSearch(ServerNetwork network)
    {
        var (bridges, _) = BridgesAndArticulationPoints.Find<
            ServerNode, ServerTopology, ListChildren<ServerNode>,
            NaturalChildOrder<ServerNode, ListChildren<ServerNode>>, ListChildren<ServerNode>>(
            network.Servers);

        var critical = new int[bridges.Count][];

        for (var i = 0; i < bridges.Count; i++)
        {
            critical[i] = Pair(bridges[i].A.Id, bridges[i].B.Id);
        }

        return critical;
    }

    // An undirected connection has no direction to report, so both strategies emit
    // the smaller server first and the two arms stay directly comparable.
    private static int[] Pair(int first, int second) =>
        first <= second ? [first, second] : [second, first];
}
