using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.JumpGameIV;

// LeetCode 1345. Jump Game IV: minimum steps from index 0 to the last index,
// where one step goes to i+1, to i-1, or to any index sharing arr[i]'s value.
//
// The two strategies answer the same question two ways: the textbook level-order
// BFS straight over the array - with the "clear a value's index group once every
// member has been enqueued" trick that keeps a large equal-value run from being
// re-scanned on every visit - and modeling the exact same reachability as an
// implicit unweighted-hop graph answered with this repo's own
// ShortestPath.Dijkstra.
internal static class JumpGameIVSolution
{
    // The textbook answer: a BCL Queue, a BCL Dictionary of same-value index
    // groups, and a visited flag array. Deliberately written without this repo's
    // primitives - the arm the composed solution below has to justify itself
    // against.
    public static int MinJumpsByBfsWithGroupPruning(int[] arr)
    {
        var walk = CreateWalk(arr);

        return RunLevelOrderSearch(arr, walk);
    }

    private static HopWalk CreateWalk(int[] arr)
    {
        var visited = new bool[arr.Length];
        visited[0] = true;
        var frontier = new Queue<int>();
        frontier.Enqueue(0);

        return new HopWalk(arr.Length, GroupIndicesByValue(arr), visited, frontier);
    }

    private static int RunLevelOrderSearch(int[] arr, HopWalk walk)
    {
        var steps = 0;

        while (walk.Frontier.Count > 0)
        {
            var levelSize = walk.Frontier.Count;

            for (var k = 0; k < levelSize; k++)
            {
                var i = walk.Frontier.Dequeue();

                if (ExpandFrom(i, arr, walk))
                {
                    return steps;
                }
            }

            steps++;
        }

        return LeetCodeAnswer.None;
    }

    private static bool ExpandFrom(int i, int[] arr, HopWalk walk)
    {
        if (i == walk.Length - 1)
        {
            return true;
        }

        if (walk.IndicesByValue.TryGetValue(arr[i], out var sameValue))
        {
            foreach (var j in sameValue)
            {
                EnqueueUnvisited(j, walk);
            }

            walk.IndicesByValue.Remove(arr[i]);
        }

        EnqueueUnvisited(i + 1, walk);
        EnqueueUnvisited(i - 1, walk);

        return false;
    }

    private static void EnqueueUnvisited(int target, HopWalk walk)
    {
        if (CannotBeEnqueued(walk, target))
        {
            return;
        }

        walk.Visited[target] = true;
        walk.Frontier.Enqueue(target);
    }

    // A hop target only joins the frontier when it lands on the array and has not been
    // reached yet - anything else is a step to nowhere or a step already taken.
    private static bool CannotBeEnqueued(HopWalk walk, int target) =>
        target < 0 || target >= walk.Length || walk.Visited[target];

    private readonly record struct HopWalk(
        int Length,
        Dictionary<int, List<int>> IndicesByValue,
        bool[] Visited,
        Queue<int> Frontier);

    public static int MinJumpsByDijkstraOverHopGraph(int[] arr)
    {
        var nodes = BuildHopGraph(arr);

        return MinJumpsByDijkstraOverHopGraph(nodes);
    }

    // The prepared-input overload: a benchmark hoists graph construction into
    // [GlobalSetup] so it isn't charged to the search being measured.
    // ValueHopNode[] is never ambiguous with the int[] overload above - they share
    // no common type the caller's argument could bind to either way.
    public static int MinJumpsByDijkstraOverHopGraph(ValueHopNode[] nodes)
    {
        var distances = ShortestPath.Dijkstra<
            ValueHopNode, ValueHopTopology, ListEdges<ValueHopNode, int>, int>(nodes[0]);

        return distances[nodes[^1]];
    }

    public static ValueHopNode[] BuildHopGraph(int[] arr)
    {
        var nodes = CreateNodes(arr.Length);
        var indicesByValue = GroupIndicesByValue(arr);

        for (var i = 0; i < arr.Length; i++)
        {
            ConnectAdjacentIndices(nodes, i);
            ConnectSameValueIndices(nodes, i, indicesByValue[arr[i]]);
        }

        return nodes;
    }

    private static ValueHopNode[] CreateNodes(int length)
    {
        var nodes = new ValueHopNode[length];

        for (var i = 0; i < length; i++)
        {
            nodes[i] = new ValueHopNode(i);
        }

        return nodes;
    }

    private static void ConnectAdjacentIndices(ValueHopNode[] nodes, int i)
    {
        if (i + 1 < nodes.Length)
        {
            nodes[i].Edges.Add((1, nodes[i + 1]));
        }

        if (i - 1 >= 0)
        {
            nodes[i].Edges.Add((1, nodes[i - 1]));
        }
    }

    private static void ConnectSameValueIndices(ValueHopNode[] nodes, int i, List<int> sameValueIndices)
    {
        foreach (var j in sameValueIndices)
        {
            if (j != i)
            {
                nodes[i].Edges.Add((1, nodes[j]));
            }
        }
    }

    private static Dictionary<int, List<int>> GroupIndicesByValue(int[] arr)
    {
        var indicesByValue = new Dictionary<int, List<int>>();

        for (var i = 0; i < arr.Length; i++)
        {
            if (!indicesByValue.TryGetValue(arr[i], out var indices))
            {
                indices = [];
                indicesByValue[arr[i]] = indices;
            }

            indices.Add(i);
        }

        return indicesByValue;
    }
}
