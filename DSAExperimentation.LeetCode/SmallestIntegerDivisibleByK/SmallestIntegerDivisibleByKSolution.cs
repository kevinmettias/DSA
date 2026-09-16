using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.SmallestIntegerDivisibleByK;

// LeetCode 1015. Smallest Integer Divisible by K: the length of the shortest repunit
// (1, 11, 111, ...) that the divisor divides, or -1 when no such integer exists.
//
// Remainders mod the divisor are the nodes of an implicit graph with one edge from
// remainder to (remainder*10+1) % divisor - the remainder after appending one more '1'
// digit. The fewest extra digits needed to reach remainder 0, plus the first digit
// already placed, is exactly a BFS distance, so the composed strategy is Reduce.Graph in
// BreadthFirstReduceOrder with DistanceMapReduceAlgebra, the same composition OpenTheLock
// and WordLadder use, just over a single-successor graph. That also gets "no such integer
// exists" for free: BFS's own visited-tracking, not a separate gcd(divisor, 10) check, is
// what proves remainder 0 unreachable when the divisor shares a factor with 10.
internal static class SmallestIntegerDivisibleByKSolution
{
    // The textbook answer: one int updated in place, BCL-only. After divisor steps the
    // remainders must have repeated, so failing to hit 0 by then proves there is no
    // answer at all.
    public static int SmallestRepunitLengthByModularWalk(int divisor)
    {
        var remainder = 0;

        for (var length = 1; length <= divisor; length++)
        {
            remainder = ((remainder * RepunitDigitBase.Value) + 1) % divisor;

            if (remainder == 0)
            {
                return length;
            }
        }

        return LeetCodeAnswer.None;
    }

    // This repo's own BFS over the materialized remainder graph. The distance map is
    // keyed by node, so the answer is one lookup: distance counts the digits appended
    // after the first, hence the + 1.
    public static int SmallestRepunitLengthByReduceGraph(int divisor) =>
        SmallestRepunitLengthByReduceGraph(RemainderGraph.Build(divisor));

    public static int SmallestRepunitLengthByReduceGraph(RemainderGraph graph)
    {
        var distances = Reduce.Graph<
            RemainderNode, RemainderTopology, ListChildren<RemainderNode>,
            NaturalChildOrder<RemainderNode, ListChildren<RemainderNode>>, ListChildren<RemainderNode>,
            BreadthFirstReduceOrder<RemainderNode>,
            DistanceMapReduceAlgebra<RemainderNode>, Dictionary<RemainderNode, int>>(graph.Start);

        return distances.TryGetValue(graph.Zero, out var distance)
            ? RepunitLength(distance)
            : LeetCodeAnswer.None;
    }

    // The repunit's digit count: the map's distance plus the leading digit it counts from.
    private static int RepunitLength(int distance) => distance + 1;
}
