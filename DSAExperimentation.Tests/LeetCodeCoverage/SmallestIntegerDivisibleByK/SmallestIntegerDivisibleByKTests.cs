using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.SmallestIntegerDivisibleByK.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestIntegerDivisibleByK;

// LeetCode 1015. Smallest Integer Divisible by K: remainders mod k are nodes of an
// implicit graph, with a single edge from r to (r*10+1) % k - the remainder after
// appending one more '1' digit to a repunit. The fewest additional digits needed to
// reach remainder 0 (plus the first digit already placed) is exactly Reduce.Graph's
// own BFS distance from remainder (1 % k) - the same DistanceMapReduceAlgebra
// composition OpenTheLockTests/WordLadderTests already use, just over a
// single-successor graph instead of a multi-neighbor one. This also gets the "no
// such integer exists" case for free: BFS's own visited-tracking, not a separate
// gcd(k, 10) check, is what proves remainder 0 unreachable when k shares a factor
// with 10.
public sealed partial class SmallestIntegerDivisibleByKTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, -1)]
    [InlineData(3, 3)]
    [InlineData(7, 6)]
    public void SmallestRepunitLength_LeetCodeExamples_ReturnsShortestLengthOrNegativeOne(int k, int expected)
        => Assert.Equal(expected, SmallestRepunitLength(k));

    private static int SmallestRepunitLength(int k)
    {
        var nodesByRemainder = BuildRemainderGraph(k);
        var startNode = nodesByRemainder[1 % k];
        var zeroNode = nodesByRemainder[0];

        var distances = Reduce.Graph<
            RemainderNode, RemainderTopology, ListChildren<RemainderNode>,
            NaturalChildOrder<RemainderNode, ListChildren<RemainderNode>>, ListChildren<RemainderNode>,
            BreadthFirstReduceOrder<RemainderNode>,
            DistanceMapReduceAlgebra<RemainderNode>, Dictionary<RemainderNode, int>>(startNode);

        return distances.TryGetValue(zeroNode, out var distance) ? distance + 1 : -1;
    }

    private static Dictionary<int, RemainderNode> BuildRemainderGraph(int k)
    {
        var nodesByRemainder = new Dictionary<int, RemainderNode>();

        for (var remainder = 0; remainder < k; remainder++)
        {
            nodesByRemainder[remainder] = new RemainderNode(remainder);
        }

        foreach (var node in nodesByRemainder.Values)
        {
            node.Neighbors.Add(nodesByRemainder[(node.Remainder * 10 + 1) % k]);
        }

        return nodesByRemainder;
    }
}
