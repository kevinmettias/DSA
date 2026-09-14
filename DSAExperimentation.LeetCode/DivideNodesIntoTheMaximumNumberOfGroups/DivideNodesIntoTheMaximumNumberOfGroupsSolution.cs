using DSAExperimentation.Algorithms.Bipartiteness;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.KeyedDisjointSet;

namespace DSAExperimentation.LeetCode.DivideNodesIntoTheMaximumNumberOfGroups;

// LeetCode 2493. Divide Nodes Into the Maximum Number of Groups: number every
// node with a group so that each edge joins two consecutively numbered groups,
// and maximize the number of groups used.
//
// Such a numbering is a layering, so it exists exactly when every connected
// component is bipartite - one odd cycle anywhere and the answer is -1. Within a
// bipartite component the best a root can do is its own BFS eccentricity plus
// one, and the component's answer is that maximized over every possible root;
// components are numbered independently, so the answer sums their bests.
//
// Both strategies follow that same three-part shape - reject on a color
// conflict, group the nodes into components, then replay one BFS per node - and
// differ only in what does the work: hand-rolled arrays, or this repo's own
// BipartiteCheck, KeyedDisjointSet and Reduce.Graph.
internal static class DivideNodesIntoTheMaximumNumberOfGroupsSolution
{
    // LeetCode numbers nodes from 1, so index 0 of a prepared adjacency array is
    // an unused placeholder that every walk here starts past.
    private const int FirstNode = 1;

    // Uncolored; the two sides are +1 and -1 so switching sides is a negation.
    private const sbyte Uncolored = 0;
    private const sbyte FirstSide = 1;

    // No BFS has reached this node yet.
    private const int Unreached = -1;

    // The textbook answer: an sbyte[] color array walked by a BCL Queue for the
    // bipartite check, a hand-rolled int[] union-find for the components, and an
    // int[] distance array plus another Queue for each node's eccentricity.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int MagnificentSetsByArrayAdjacencyBfs(int nodeCount, int[][] edges)
    {
        var adjacency = GroupAdjacency.Build(nodeCount, edges);

        return MagnificentSetsByArrayAdjacencyBfs(adjacency);
    }

    public static int MagnificentSetsByArrayAdjacencyBfs(GroupAdjacency adjacency)
    {
        var neighbors = adjacency.Neighbors;

        if (!IsBipartiteByColorArray(neighbors))
        {
            return LeetCodeAnswer.None;
        }

        var componentRoots = ComponentRoots(neighbors);
        var bestByComponent = new Dictionary<int, int>();

        for (var node = FirstNode; node < neighbors.Length; node++)
        {
            RecordBest(bestByComponent, componentRoots[node], EccentricityFrom(neighbors, node) + 1);
        }

        return bestByComponent.Values.Sum();
    }

    private static bool IsBipartiteByColorArray(int[][] neighbors)
    {
        var color = new sbyte[neighbors.Length];

        for (var start = FirstNode; start < neighbors.Length; start++)
        {
            if (color[start] != Uncolored)
            {
                continue;
            }

            if (!TryColorComponent(neighbors, start, color))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryColorComponent(int[][] neighbors, int start, sbyte[] color)
    {
        var queue = new Queue<int>();
        color[start] = FirstSide;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            if (!TryColorNeighbors(neighbors, node, color, queue))
            {
                return false;
            }
        }

        return true;
    }

    // Gives every uncolored neighbour the opposite side and queues it; a neighbour
    // already on this node's own side is the odd cycle that makes the graph
    // unlayerable.
    private static bool TryColorNeighbors(int[][] neighbors, int node, sbyte[] color, Queue<int> queue)
    {
        foreach (var neighbor in neighbors[node])
        {
            if (color[neighbor] == color[node])
            {
                return false;
            }

            if (color[neighbor] == Uncolored)
            {
                color[neighbor] = (sbyte)-color[node];
                queue.Enqueue(neighbor);
            }
        }

        return true;
    }

    private static int EccentricityFrom(int[][] neighbors, int start)
    {
        var distance = new int[neighbors.Length];
        Array.Fill(distance, Unreached);
        distance[start] = 0;

        var queue = new Queue<int>();
        queue.Enqueue(start);
        var maxDistance = 0;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            var reached = EnqueueUnreached(neighbors, node, distance, queue);

            maxDistance = Math.Max(maxDistance, reached);
        }

        return maxDistance;
    }

    // Hands every not-yet-reached neighbour its distance and queues it, returning
    // the greatest distance handed out here - zero when this node expands nothing.
    // The root is the only node at distance zero, so a maximum over every
    // expansion is the eccentricity.
    private static int EnqueueUnreached(int[][] neighbors, int node, int[] distance, Queue<int> queue)
    {
        var maxDistance = 0;

        foreach (var neighbor in neighbors[node])
        {
            if (distance[neighbor] != Unreached)
            {
                continue;
            }

            distance[neighbor] = distance[node] + 1;
            maxDistance = Math.Max(maxDistance, distance[neighbor]);
            queue.Enqueue(neighbor);
        }

        return maxDistance;
    }

    private static int[] ComponentRoots(int[][] neighbors)
    {
        var parent = Enumerable.Range(0, neighbors.Length).ToArray();

        for (var node = FirstNode; node < neighbors.Length; node++)
        {
            foreach (var neighbor in neighbors[node])
            {
                Union(parent, node, neighbor);
            }
        }

        var roots = new int[neighbors.Length];

        for (var node = FirstNode; node < neighbors.Length; node++)
        {
            roots[node] = Find(parent, node);
        }

        return roots;
    }

    private static int Find(int[] parent, int node)
    {
        while (parent[node] != node)
        {
            parent[node] = parent[parent[node]];
            node = parent[node];
        }

        return node;
    }

    private static void Union(int[] parent, int first, int second)
    {
        var firstRoot = Find(parent, first);
        var secondRoot = Find(parent, second);

        if (firstRoot != secondRoot)
        {
            parent[firstRoot] = secondRoot;
        }
    }

    // This repo's own answer, one primitive per part of the shape:
    // Algorithms.Bipartiteness.BipartiteCheck is already the multi-root
    // 2-coloring (the same composition PossibleBipartitionSolution uses for LC
    // 886), KeyedDisjointSet<int> groups nodes into components by union over the
    // edges, and Reduce.Graph in BreadthFirstReduceOrder with
    // DistanceMapReduceAlgebra is already "distance from a root to every node" -
    // the same composition OpenTheLockSolution uses for LC 752, read here for its
    // largest value rather than one target's, and replayed once per node.
    public static int MagnificentSetsByReducePrimitives(int nodeCount, int[][] edges)
    {
        var graph = GroupGraph.Build(nodeCount, edges);

        return MagnificentSetsByReducePrimitives(graph);
    }

    public static int MagnificentSetsByReducePrimitives(GroupGraph graph)
    {
        var nodes = graph.Nodes;

        if (!BipartiteCheck.IsBipartite<
            GroupNode, GroupTopology, ListChildren<GroupNode>,
            NaturalChildOrder<GroupNode, ListChildren<GroupNode>>, ListChildren<GroupNode>>(nodes))
        {
            return LeetCodeAnswer.None;
        }

        var components = Components(nodes);
        var bestByComponent = new Dictionary<int, int>();

        foreach (var node in nodes)
        {
            components.TryFind(node.Id, out var componentRoot);
            RecordBest(bestByComponent, componentRoot, EccentricityFrom(node) + 1);
        }

        return bestByComponent.Values.Sum();
    }

    private static KeyedDisjointSet<int> Components(IReadOnlyList<GroupNode> nodes)
    {
        var components = new KeyedDisjointSet<int>(nodes.Select(node => node.Id));

        foreach (var node in nodes)
        {
            foreach (var neighbor in node.Neighbors)
            {
                components.TryUnion(node.Id, neighbor.Id);
            }
        }

        return components;
    }

    private static int EccentricityFrom(GroupNode root)
    {
        var distances = Reduce.Graph<
            GroupNode, GroupTopology, ListChildren<GroupNode>,
            NaturalChildOrder<GroupNode, ListChildren<GroupNode>>, ListChildren<GroupNode>,
            BreadthFirstReduceOrder<GroupNode>,
            DistanceMapReduceAlgebra<GroupNode>, Dictionary<GroupNode, int>>(root);

        return distances.Values.Max();
    }

    // A component's answer is the best any of its nodes can do as a BFS root.
    private static void RecordBest(Dictionary<int, int> bestByComponent, int component, int candidate)
    {
        if (!bestByComponent.TryGetValue(component, out var best) || candidate > best)
        {
            bestByComponent[component] = candidate;
        }
    }
}
