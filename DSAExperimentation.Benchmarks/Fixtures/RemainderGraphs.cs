namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds a Smallest Integer Divisible by K scenario (LC 1015): every remainder mod
// k as a node in a k-node functional graph, one outgoing edge per node
// (r -> (r*10+1) % k). K is deliberately chosen coprime to 10 (odd, not a multiple
// of 5) so remainder 0 is genuinely reachable, forcing both benchmarked approaches
// through real work instead of an immediate "-1" short circuit.
internal static class RemainderGraphs
{
    private const int DecimalDigitBase = 10;

    public static (Dictionary<int, RemainderNode> NodesByRemainder, RemainderNode StartNode) BuildGraph(int k)
    {
        var nodesByRemainder = new Dictionary<int, RemainderNode>();

        for (var remainder = 0; remainder < k; remainder++)
        {
            nodesByRemainder[remainder] = new RemainderNode(remainder);
        }

        foreach (var node in nodesByRemainder.Values)
        {
            node.Neighbors.Add(nodesByRemainder[(node.Remainder * DecimalDigitBase + 1) % k]);
        }

        return (nodesByRemainder, nodesByRemainder[1 % k]);
    }
}
