namespace DSAExperimentation.LeetCode.Solutions.ValidArrangementOfPairs;

// LC 2097 accepts any arrangement of the pairs in which each pair starts where the one before it ends.
// So an answer stands when it holds exactly the given pairs, each as often as it is given, chained end to
// start. The recorded arrangement is one of these, so it does not take part. The two strategies visit a
// node's outgoing pairs in different orders, so their Eulerian paths part where a node has several.
internal static class ValidArrangementOfPairsAnswerCheck
{
    public static bool IsAccepted(int[][] pairs, int[][] answer, int[][] recorded)
        => IsSamePairs(pairs, answer) && answer.Zip(answer.Skip(1)).All(link => link.First[1] == link.Second[0]);

    // The same pairs, each as many times, in any order.
    private static bool IsSamePairs(int[][] pairs, int[][] answer)
        => answer.Select(pair => (pair[0], pair[1])).Order().SequenceEqual(pairs.Select(pair => (pair[0], pair[1])).Order());
}
