namespace DSAExperimentation.LeetCode.Solutions.PathWithMaximumProbability;

// LC 1514's promise about its input: each edge joins two of the nodes 0 to n - 1 and has its own success
// probability, and the start and end are nodes of the graph. LeetCode's judge never poses an input that
// breaks it. Both strategies index a node table by node and the probabilities by edge. Generated cases do
// not always keep the promise, so a harness that runs them asks this first.
internal static class PathWithMaximumProbabilityPromise
{
    public static bool IsKeptBy(int nodeCount, int[][] edges, double[] successProbabilities, (int Start, int End) endpoints)
    {
        var isNode = (int node) => node >= 0 && node < nodeCount;
        return edges.Length == successProbabilities.Length && edges.All(edge => edge.All(node => isNode(node))) && isNode(endpoints.Start) && isNode(endpoints.End);
    }
}
