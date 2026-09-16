namespace DSAExperimentation.LeetCode.SmallestIntegerDivisibleByK;

// The k-node functional graph LC 1015 searches: one node per remainder mod k, one
// outgoing edge per node (r -> (r*10+1) % k, "append one more '1' digit"). Build is
// deterministic construction of the problem's own structure, so it belongs here in
// the solution tier rather than in a benchmark fixture (ARCHITECTURE.md §17.7); the
// benchmark only chooses how large a k to measure.
//
// Start is the remainder of the one-digit repunit "1" and Zero is the node the
// search is trying to reach; for k = 1 they are the same node, which is exactly why
// the answer there is a single digit.
internal sealed class RemainderGraph
{
    public RemainderNode Start { get; }

    public RemainderNode Zero { get; }

    private RemainderGraph(RemainderNode start, RemainderNode zero)
    {
        Start = start;
        Zero = zero;
    }

    public static RemainderGraph Build(int k)
    {
        var nodesByRemainder = new Dictionary<int, RemainderNode>();

        for (var remainder = 0; remainder < k; remainder++)
        {
            nodesByRemainder[remainder] = new RemainderNode(remainder);
        }

        foreach (var node in nodesByRemainder.Values)
        {
            node.Neighbors.Add(nodesByRemainder[((node.Remainder * RepunitDigitBase.Value) + 1) % k]);
        }

        return new RemainderGraph(nodesByRemainder[1 % k], nodesByRemainder[0]);
    }
}
