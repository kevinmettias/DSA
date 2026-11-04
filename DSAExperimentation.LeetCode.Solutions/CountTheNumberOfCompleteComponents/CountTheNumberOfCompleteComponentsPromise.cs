namespace DSAExperimentation.LeetCode.Solutions.CountTheNumberOfCompleteComponents;

// LC 2685's promise about its input: no edge joins a node to itself, and none is listed twice, in
// either direction. LeetCode's judge never poses an input that breaks it. The adjacency count
// counts a repeated edge twice, and the disjoint-set tally once. Generated cases do not always keep
// the promise, so a harness that runs them asks this first.
internal static class CountTheNumberOfCompleteComponentsPromise
{
    public static bool IsKeptBy(int nodeCount, int[][] edges)
        => edges.All(edge => edge[0] != edge[1]) && IsEachListedOnce(edges);

    // Each edge read as the pair of nodes it joins, smaller first, so either direction is one pair.
    private static bool IsEachListedOnce(int[][] edges)
        => edges.Select(edge => (Math.Min(edge[0], edge[1]), Math.Max(edge[0], edge[1]))).Distinct().Count() == edges.Length;
}
