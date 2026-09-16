using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.LargestColorValueInADirectedGraph;

// LeetCode 1857. Largest Color Value in a Directed Graph: over every path in a
// directed graph, the largest number of nodes sharing one color; -1 when the graph
// has a cycle, because a cycle makes the count unbounded.
//
// Both strategies run the same per-color counting DP - count[node][c] is the most
// c-colored nodes on any path ending at node - and differ only in how they order
// the relaxations. KahnsTopologicalSort composes this repo's own
// TopologicalSort.TrySort, which both detects the cycle case (false -> -1, the same
// leftover-in-degree signal Course Schedule relies on) and hands back a
// dependency-respecting order in which every edge is relaxed exactly once, O(V+E).
// RepeatedRelaxation is the textbook answer when you have no ordering to hand:
// relax every edge, once per node, for as many rounds as there are nodes
// (Bellman-Ford style), which converges regardless of processing order at O(V*E).
//
// The answer is read off each node's count for its OWN color: for the path and
// color that achieve the maximum, the last node on that path carrying that color
// ends a prefix with the same count, so nothing is missed.
internal static class LargestColorValueInADirectedGraphSolution
{
    // Colors are the 26 lowercase letters, so a per-node count vector is a fixed
    // 26-slot array rather than a map.
    private const int AlphabetSize = 26;

    // Depth-first cycle-detection marks for the baseline's own walk.
    private const int Unvisited = 0;
    private const int InProgress = 1;
    private const int Finished = 2;

    // LeetCode's own input shape: colors[i] is node i's lowercase color letter and
    // edges[j] = [from, to] is a directed edge.
    public static int LargestPathValueByKahnsTopologicalSort(string colors, int[][] edges)
    {
        var nodes = BuildGraph(colors, edges);

        return LargestPathValueByKahnsTopologicalSort(nodes);
    }

    public static int LargestPathValueByKahnsTopologicalSort(List<ColorGraphNode> nodes)
    {
        var sorted = TopologicalSort.TrySort<
            ColorGraphNode, ColorGraphTopology, ListChildren<ColorGraphNode>,
            NaturalChildOrder<ColorGraphNode, ListChildren<ColorGraphNode>>, ListChildren<ColorGraphNode>>(
            nodes, out var ordering);

        if (!sorted)
        {
            return LeetCodeAnswer.None;
        }

        var counts = nodes.ToDictionary(node => node, _ => new int[AlphabetSize]);
        var best = 0;

        foreach (var node in ordering)
        {
            var nodeBest = RelaxForward(node, counts);

            best = Math.Max(best, nodeBest);
        }

        return best;
    }

    // Increments node's own color count and relaxes it forward onto every successor
    // (each child's count[c] becomes the max of its own and node's, since Kahn's
    // order guarantees node is fully finalized before any child is visited).
    // Returns node's own updated count for its color.
    private static int RelaxForward(ColorGraphNode node, Dictionary<ColorGraphNode, int[]> counts)
    {
        var nodeCounts = counts[node];
        nodeCounts[node.Color]++;

        var children = ColorGraphTopology.GetChildren(node);

        for (var i = 0; i < children.Count; i++)
        {
            var childCounts = counts[children.Get(i)];

            for (var c = 0; c < AlphabetSize; c++)
            {
                childCounts[c] = Math.Max(childCounts[c], nodeCounts[c]);
            }
        }

        return nodeCounts[node.Color];
    }

    // The textbook answer: a depth-first three-colour cycle check, then relax every
    // edge of every node for as many rounds as there are nodes, which reaches the
    // fixed point whatever order the nodes happen to be enumerated in. Deliberately
    // written with BCL collections and no ordering primitive - it is the arm the
    // composed solution above has to justify itself against.
    public static int LargestPathValueByRepeatedRelaxation(string colors, int[][] edges)
    {
        var nodes = BuildGraph(colors, edges);

        return LargestPathValueByRepeatedRelaxation(nodes);
    }

    public static int LargestPathValueByRepeatedRelaxation(List<ColorGraphNode> nodes)
    {
        if (HasCycle(nodes))
        {
            return LeetCodeAnswer.None;
        }

        var counts = BuildInitialCounts(nodes);
        RelaxAllRounds(nodes, counts);

        return counts.Values.SelectMany(nodeCounts => nodeCounts).Max();
    }

    private static Dictionary<ColorGraphNode, int[]> BuildInitialCounts(List<ColorGraphNode> nodes)
        => nodes.ToDictionary(node => node, node =>
        {
            var counts = new int[AlphabetSize];
            counts[node.Color] = 1;
            return counts;
        });

    private static void RelaxAllRounds(List<ColorGraphNode> nodes, Dictionary<ColorGraphNode, int[]> counts)
    {
        for (var round = 0; round < nodes.Count; round++)
        {
            foreach (var node in nodes)
            {
                RelaxEveryEdge(counts, node);
            }
        }
    }

    private static void RelaxEveryEdge(Dictionary<ColorGraphNode, int[]> counts, ColorGraphNode node)
    {
        var nodeCounts = counts[node];

        foreach (var child in node.Successors)
        {
            var childCounts = counts[child];

            for (var c = 0; c < AlphabetSize; c++)
            {
                var candidate = nodeCounts[c] + (c == child.Color ? 1 : 0);

                if (candidate > childCounts[c])
                {
                    childCounts[c] = candidate;
                }
            }
        }
    }

    private static bool HasCycle(List<ColorGraphNode> nodes)
    {
        var marks = nodes.ToDictionary(node => node, _ => Unvisited);

        return nodes.Any(node => marks[node] == Unvisited && HasPathToOwnAncestor(node, marks));
    }

    private static bool HasPathToOwnAncestor(ColorGraphNode node, Dictionary<ColorGraphNode, int> marks)
    {
        marks[node] = InProgress;

        foreach (var child in node.Successors)
        {
            if (IsClosingCycle(child, marks))
            {
                return true;
            }
        }

        marks[node] = Finished;
        return false;
    }

    // A child still in progress on the walk's own path closes a cycle outright; an
    // unvisited child closes one if it can reach back to an ancestor from below.
    private static bool IsClosingCycle(ColorGraphNode child, Dictionary<ColorGraphNode, int> marks)
        => marks[child] == InProgress || (marks[child] == Unvisited && HasPathToOwnAncestor(child, marks));

    private static List<ColorGraphNode> BuildGraph(string colors, int[][] edges)
    {
        var nodes = colors.Select((color, id) => new ColorGraphNode(id, color - 'a')).ToList();

        foreach (var edge in edges)
        {
            nodes[edge[0]].Successors.Add(nodes[edge[1]]);
        }

        return nodes;
    }
}
