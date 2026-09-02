namespace DSAExperimentation.LeetCode.MinimumOperationsToEqualizeBinaryString;

// The state graph behind LC 3666: one node per possible zero-count 0..n: flipping
// exactly k indices picks some i of the current zeros (0 <= i <= min(k, zeroCount))
// and k - i of the current ones (0 <= k - i <= n - zeroCount), turning zeroCount
// into zeroCount + k - 2i. As i ranges over its valid interval, the reachable
// zero-counts form a contiguous, constant-parity run - every zero-count in
// [zeroCount + k - 2*iMax, zeroCount + k - 2*iMin] stepping by 2.
//
// This is the domain model, not an answer to any one query about it - it knows how
// one flip-k operation moves between zero-counts and nothing about which start or
// target a caller wants. Callers supply their own.
internal sealed class EqualizeStateGraph
{
    private readonly EqualizeStateNode[] _nodesByZeroCount;

    private EqualizeStateGraph(EqualizeStateNode[] nodesByZeroCount) => _nodesByZeroCount = nodesByZeroCount;

    // One node per zero-count 0..n, each wired to every zero-count one operation
    // reaches from it.
    public static EqualizeStateGraph Build(int n, int k)
    {
        var nodes = new EqualizeStateNode[n + 1];
        for (var zeroCount = 0; zeroCount <= n; zeroCount++)
        {
            nodes[zeroCount] = new EqualizeStateNode(zeroCount);
        }

        for (var zeroCount = 0; zeroCount <= n; zeroCount++)
        {
            foreach (var reachable in ReachableZeroCounts(zeroCount, n, k))
            {
                nodes[zeroCount].Neighbors.Add(nodes[reachable]);
            }
        }

        return new EqualizeStateGraph(nodes);
    }

    public EqualizeStateNode Node(int zeroCount) => _nodesByZeroCount[zeroCount];

    // Pure zero-count arithmetic, so callers that never materialize the graph
    // (a hand-rolled BFS, say) can use it too.
    public static IEnumerable<int> ReachableZeroCounts(int zeroCount, int n, int k)
    {
        var onesCount = n - zeroCount;
        var flippedZerosMin = Math.Max(0, k - onesCount);
        var flippedZerosMax = Math.Min(k, zeroCount);

        for (var flippedZeros = flippedZerosMax; flippedZeros >= flippedZerosMin; flippedZeros--)
        {
            yield return zeroCount + k - 2 * flippedZeros;
        }
    }
}
