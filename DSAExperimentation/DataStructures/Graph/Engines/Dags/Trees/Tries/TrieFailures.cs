namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The way an ordered child store refuses a rank, worded once for every store: a rank at or past the
// node's child count. Thrown from here, so the store's own lookup keeps no throw in its body.
internal static class TrieFailures
{
    private const string NotAChildRankMessage = "Expected a rank below the node's child count.";

    public static void ThrowNotAChildRank(int rank) =>
        throw new ArgumentOutOfRangeException(nameof(rank), rank, NotAChildRankMessage);
}
