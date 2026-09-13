using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.RevealCardsInIncreasingOrder;

// LeetCode 950. Reveal Cards In Increasing Order: order the deck so that repeatedly
// "reveal the top card, then move the next card to the bottom" reveals the cards in
// increasing order.
//
// Both strategies run the same inversion: sort the deck, then simulate the
// reveal/move-to-bottom process forwards over the ORIGINAL index positions, handing
// each sorted value to whichever slot the simulation reveals next. They differ only in
// the container holding those pending indices, which is where the whole cost sits:
//
//   ListRemoveAt is the naive simulation most people reach for first - a BCL
//   List<int> with RemoveAt(0) standing in for "pop the front", O(n) per removal and
//   O(n^2) overall.
//
//   QueueRotation drives the identical simulation with this repo's own Queue<int>
//   (Deque-backed, O(1) amortized at both ends), where front-dequeue/back-enqueue is
//   literally the "move the next card to the bottom" step, for O(n) overall.
//
// Both pay the same O(n log n) sort up front, so the asymmetry is entirely in the
// simulation loop.
internal static class RevealCardsInIncreasingOrderSolution
{
    // The textbook arm: BCL List<int> as the pending-index buffer, shifting the whole
    // tail on every front removal.
    public static int[] DeckRevealedIncreasingByListRemoveAt(int[] deck)
    {
        var sorted = SortedCopy(deck);
        var indices = BuildIndexList(deck.Length);
        var result = new int[deck.Length];

        foreach (var value in sorted)
        {
            RevealAndRotate(indices, result, value);
        }

        return result;
    }

    // The composed arm: this repo's Queue<int>, whose FIFO front-dequeue and
    // back-enqueue are exactly the reveal and the move-to-bottom.
    public static int[] DeckRevealedIncreasingByQueueRotation(int[] deck)
    {
        var sorted = SortedCopy(deck);
        var indices = BuildIndexQueue(deck.Length);
        var result = new int[deck.Length];

        foreach (var value in sorted)
        {
            RevealAndRotate(indices, result, value);
        }

        return result;
    }

    private static int[] SortedCopy(int[] deck)
    {
        var sorted = (int[])deck.Clone();
        Array.Sort(sorted);
        return sorted;
    }

    private static List<int> BuildIndexList(int count)
    {
        var indices = new List<int>(count);

        for (var i = 0; i < count; i++)
        {
            indices.Add(i);
        }

        return indices;
    }

    private static RepoQueue BuildIndexQueue(int count)
    {
        var indices = new RepoQueue();

        for (var i = 0; i < count; i++)
        {
            indices.Enqueue(i);
        }

        return indices;
    }

    private static void RevealAndRotate(List<int> indices, int[] result, int value)
    {
        var revealIndex = indices[0];
        indices.RemoveAt(0);
        result[revealIndex] = value;

        if (indices.Count > 0)
        {
            var moveToBottom = indices[0];
            indices.RemoveAt(0);
            indices.Add(moveToBottom);
        }
    }

    private static void RevealAndRotate(RepoQueue indices, int[] result, int value)
    {
        indices.TryDequeue(out var revealIndex);
        result[revealIndex] = value;

        if (indices.TryDequeue(out var moveToBottom))
        {
            indices.Enqueue(moveToBottom);
        }
    }
}
