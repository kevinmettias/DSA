using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.ShortestDistanceToTargetStringInACircularArray;

// LeetCode 2515. Shortest Distance to Target String in a Circular Array: words is
// laid out on a circle, so from index i one step reaches (i + 1) % n or
// (i - 1 + n) % n. Report the fewest steps from startIndex to any index holding
// target, or -1 when target is absent.
//
// The two strategies differ in whether the wraparound is arithmetic or structure.
// ClosestTargetByLinearScan is the textbook closed form - scan the raw array and
// take min(diff, n - diff) at every match. ClosestTargetByReduceGraph makes each
// index a node with exactly two edges and hands the circle to this repo's own
// Reduce.Graph in BreadthFirstReduceOrder with DistanceMapReduceAlgebra, which is
// already "distance from a root to every node"; exploring both directions at once
// is what BFS does anyway, so no wraparound formula appears anywhere.
internal static class ShortestDistanceToTargetStringInACircularArraySolution
{
    // The textbook answer: one pass over the raw array, closing the circle with
    // min(diff, n - diff). Deliberately written with BCL arrays and integer
    // arithmetic and nothing else - it is the arm the composed strategy below has
    // to justify itself against.
    public static int ClosestTargetByLinearScan(string[] words, string target, int startIndex)
    {
        var length = words.Length;
        var best = LeetCodeAnswer.None;

        for (var i = 0; i < length; i++)
        {
            if (words[i] != target)
            {
                continue;
            }

            var difference = Math.Abs(i - startIndex);
            var distance = Math.Min(difference, length - difference);

            if (best == LeetCodeAnswer.None || distance < best)
            {
                best = distance;
            }
        }

        return best;
    }

    // LeetCode's own input shape: the circle is built here, then searched.
    public static int ClosestTargetByReduceGraph(string[] words, string target, int startIndex) =>
        ClosestTargetByReduceGraph(CircularArrayGraph.Build(words), target, startIndex);

    public static int ClosestTargetByReduceGraph(CircularArrayGraph graph, string target, int startIndex)
    {
        var distances = Reduce.Graph<
            CircularArrayNode, CircularArrayTopology, ListChildren<CircularArrayNode>,
            NaturalChildOrder<CircularArrayNode, ListChildren<CircularArrayNode>>, ListChildren<CircularArrayNode>,
            BreadthFirstReduceOrder<CircularArrayNode>,
            DistanceMapReduceAlgebra<CircularArrayNode>, Dictionary<CircularArrayNode, int>>(graph.Nodes[startIndex]);

        var best = LeetCodeAnswer.None;

        foreach (var node in graph.Nodes)
        {
            if (node.Word != target || !distances.TryGetValue(node, out var distance))
            {
                continue;
            }

            if (best == LeetCodeAnswer.None || distance < best)
            {
                best = distance;
            }
        }

        return best;
    }
}
