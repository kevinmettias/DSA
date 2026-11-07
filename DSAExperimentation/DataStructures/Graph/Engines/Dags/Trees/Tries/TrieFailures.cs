namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The way an ordered child store refuses a rank, worded once for every store: a rank at or past the
// node's child count. And the way CountedBitTrie refuses a copy past int.MaxValue, the
// OverflowException IXorMultiset names. Thrown from here, so the store's own lookup and the trie's
// insert walk keep no throw in their bodies.
internal static class TrieFailures
{
    private const string NotAChildRankMessage = "Expected a rank below the node's child count.";
    private const string CountFullMessage =
        "Expected fewer than int.MaxValue copies held before adding one.";

    public static void ThrowNotAChildRank(int rank) =>
        throw new ArgumentOutOfRangeException(nameof(rank), rank, NotAChildRankMessage);

    public static void ThrowCountFull() => throw new OverflowException(CountFullMessage);
}
