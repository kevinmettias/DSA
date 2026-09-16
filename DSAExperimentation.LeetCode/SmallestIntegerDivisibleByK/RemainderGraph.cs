namespace DSAExperimentation.LeetCode.SmallestIntegerDivisibleByK;

// The divisor-sized functional graph LC 1015 searches: one node per remainder mod the
// divisor, one outgoing edge per node (remainder -> (remainder*10+1) % divisor,
// "append one more '1' digit"). Build is deterministic construction of the problem's own
// structure, so it belongs here in the solution tier rather than in a benchmark fixture
// (ARCHITECTURE.md §17.7); the benchmark only chooses how large a divisor to measure.
//
// Start is the remainder of the one-digit repunit "1" and Zero is the node the
// search is trying to reach; for a divisor of 1 they are the same node, which is
// exactly why the answer there is a single digit.
internal sealed class RemainderGraph
{
    public RemainderNode Start { get; }

    public RemainderNode Zero { get; }

    private RemainderGraph(RemainderNode start, RemainderNode zero)
    {
        Start = start;
        Zero = zero;
    }

    public static RemainderGraph Build(int divisor)
    {
        var nodesByRemainder = new Dictionary<int, RemainderNode>();

        for (var remainder = 0; remainder < divisor; remainder++)
        {
            nodesByRemainder[remainder] = new RemainderNode(remainder);
        }

        foreach (var node in nodesByRemainder.Values)
        {
            node.Neighbors.Add(nodesByRemainder[((node.Remainder * RepunitDigitBase.Value) + 1) % divisor]);
        }

        return new RemainderGraph(nodesByRemainder[1 % divisor], nodesByRemainder[0]);
    }
}
