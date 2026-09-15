using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.SmallestIntegerDivisibleByK;

// LeetCode 1015. Smallest Integer Divisible by K: the length of the shortest repunit
// (1, 11, 111, ...) divisible by k, or -1 when no such integer exists.
//
// Remainders mod k are the nodes of an implicit graph with one edge from r to
// (r*10+1) % k - the remainder after appending one more '1' digit. The fewest extra
// digits needed to reach remainder 0, plus the first digit already placed, is exactly
// a BFS distance, so the composed strategy is Reduce.Graph in BreadthFirstReduceOrder
// with DistanceMapReduceAlgebra, the same composition OpenTheLock and WordLadder use,
// just over a single-successor graph. That also gets "no such integer exists" for
// free: BFS's own visited-tracking, not a separate gcd(k, 10) check, is what proves
// remainder 0 unreachable when k shares a factor with 10.
internal static class SmallestIntegerDivisibleByKSolution
{
    private const int DecimalDigitBase = 10;

    // The textbook answer: one int updated in place, BCL-only. After k steps the
    // remainders must have repeated, so failing to hit 0 by then proves there is no
    // answer at all.
    public static int SmallestRepunitLengthByModularWalk(int k)
    {
        var remainder = 0;

        for (var length = 1; length <= k; length++)
        {
            remainder = ((remainder * DecimalDigitBase) + 1) % k;

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
    public static int SmallestRepunitLengthByReduceGraph(int k) =>
        SmallestRepunitLengthByReduceGraph(RemainderGraph.Build(k));

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
