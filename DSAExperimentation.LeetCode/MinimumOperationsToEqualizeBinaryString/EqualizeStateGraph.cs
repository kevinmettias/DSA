namespace DSAExperimentation.LeetCode.MinimumOperationsToEqualizeBinaryString;

// The state graph behind LC 3666: one node per possible zero-count 0..stringLength:
// flipping exactly flipCount indices picks some i of the current zeros
// (0 <= i <= min(flipCount, zeroCount)) and flipCount - i of the current ones
// (0 <= flipCount - i <= stringLength - zeroCount), turning zeroCount into
// zeroCount + flipCount - 2i. As i ranges over its valid interval, the reachable
// zero-counts form a contiguous, constant-parity run - every zero-count in
// [zeroCount + flipCount - 2*iMax, zeroCount + flipCount - 2*iMin] stepping by 2.
//
// This is the domain model, not an answer to any one query about it - it knows how
// one flip operation moves between zero-counts and nothing about which start or
// target a caller wants. Callers supply their own.
internal sealed class EqualizeStateGraph
{
    private readonly EqualizeStateNode[] _nodesByZeroCount;

    private EqualizeStateGraph(EqualizeStateNode[] nodesByZeroCount) => _nodesByZeroCount = nodesByZeroCount;

    // One node per zero-count 0..stringLength, each wired to every zero-count one
    // operation reaches from it.
    public static EqualizeStateGraph Build(int stringLength, int flipCount)
    {
        var nodes = new EqualizeStateNode[stringLength + 1];
        for (var zeroCount = 0; zeroCount <= stringLength; zeroCount++)
        {
            nodes[zeroCount] = new EqualizeStateNode(zeroCount);
        }

        for (var zeroCount = 0; zeroCount <= stringLength; zeroCount++)
        {
            foreach (var reachable in ReachableZeroCounts(zeroCount, stringLength, flipCount))
            {
                nodes[zeroCount].Neighbors.Add(nodes[reachable]);
            }
        }

        return new EqualizeStateGraph(nodes);
    }

    public EqualizeStateNode Node(int zeroCount) => _nodesByZeroCount[zeroCount];

    // Pure zero-count arithmetic, so callers that never materialize the graph
    // (a hand-rolled BFS, say) can use it too.
    public static IEnumerable<int> ReachableZeroCounts(int zeroCount, int stringLength, int flipCount)
    {
        var onesCount = stringLength - zeroCount;
        var flippedZerosMin = Math.Max(0, flipCount - onesCount);
        var flippedZerosMax = Math.Min(flipCount, zeroCount);

        for (var flippedZeros = flippedZerosMax; flippedZeros >= flippedZerosMin; flippedZeros--)
        {
            yield return zeroCount + flipCount - 2 * flippedZeros;
        }
    }
}
