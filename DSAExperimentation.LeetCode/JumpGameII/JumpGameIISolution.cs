using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.JumpGameII;

// LeetCode 45. Jump Game II: minimum jumps from index 0 to the last index, where
// nums[i] is the farthest one jump from i can reach.
//
// The two strategies answer the same question two ways: the textbook O(n) greedy
// two-pointer scan, and modeling reachability as an implicit unweighted-hop graph
// (index i has a weight-1 edge to every index reachable in one jump) answered with
// this repo's own ShortestPath.Dijkstra - not the greedy scan.
internal static class JumpGameIISolution
{
    // The textbook answer: extend the farthest index reachable within the current
    // jump's range, and pay for one more jump each time that range is exhausted.
    // Deliberately written without this repo's primitives - the arm the composed
    // solution below has to justify itself against.
    public static int MinJumpsByGreedyTwoPointer(int[] nums)
    {
        var jumps = 0;
        var currentEnd = 0;
        var farthest = 0;

        for (var i = 0; i < nums.Length - 1; i++)
        {
            farthest = Math.Max(farthest, i + nums[i]);

            if (i == currentEnd)
            {
                jumps++;
                currentEnd = farthest;
            }
        }

        return jumps;
    }

    public static int MinJumpsByDijkstraOverHopGraph(int[] nums)
    {
        var nodes = BuildHopGraph(nums);

        return MinJumpsByDijkstraOverHopGraph(nodes);
    }

    // The prepared-input overload: a benchmark hoists graph construction into
    // [GlobalSetup] so it isn't charged to the search being measured. HopNode[] is
    // never ambiguous with the int[] overload above - they share no common type
    // the caller's argument could bind to either way.
    public static int MinJumpsByDijkstraOverHopGraph(HopNode[] nodes)
    {
        var distances = ShortestPath.Dijkstra<
            HopNode, HopTopology, ListEdges<HopNode, int>, int>(nodes[0]);

        return distances[nodes[^1]];
    }

    public static HopNode[] BuildHopGraph(int[] nums)
    {
        var nodes = new HopNode[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            nodes[i] = new HopNode(i);
        }

        for (var i = 0; i < nums.Length; i++)
        {
            var reach = Math.Min(i + nums[i], nums.Length - 1);

            for (var j = i + 1; j <= reach; j++)
            {
                nodes[i].Edges.Add((1, nodes[j]));
            }
        }

        return nodes;
    }
}
