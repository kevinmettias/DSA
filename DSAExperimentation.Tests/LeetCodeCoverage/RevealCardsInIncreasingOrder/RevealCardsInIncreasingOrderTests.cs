using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RevealCardsInIncreasingOrder;

// LeetCode 950. Reveal Cards In Increasing Order: sort the deck, then replay the
// reveal-then-move-to-bottom process backwards - simulate it forwards over the ORIGINAL
// index positions using this repo's own Queue<int> (FIFO front-dequeue, back-enqueue is
// exactly the "move next card to the bottom" step) to recover which original slot each
// sorted value belongs in.
public sealed partial class RevealCardsInIncreasingOrderTests
{
    [Fact]
    public void DeckRevealedIncreasing_ClassicExample_ReturnsOrderThatRevealsSorted()
    {
        int[] deck = [17, 13, 11, 2, 3, 5, 7];

        var ordered = DeckRevealedIncreasing(deck);

        Assert.Equal([2, 13, 3, 11, 5, 17, 7], ordered);
    }

    [Fact]
    public void DeckRevealedIncreasing_SingleCard_ReturnsSameCard()
    {
        int[] deck = [11];

        var ordered = DeckRevealedIncreasing(deck);

        Assert.Equal([11], ordered);
    }

    private static int[] DeckRevealedIncreasing(int[] deck)
    {
        var sorted = (int[])deck.Clone();
        Array.Sort(sorted);

        var indices = new RepoQueue();

        for (var i = 0; i < deck.Length; i++)
        {
            indices.Enqueue(i);
        }

        var result = new int[deck.Length];

        foreach (var value in sorted)
        {
            indices.TryDequeue(out var revealIndex);
            result[revealIndex] = value;

            if (indices.TryDequeue(out var moveToBottom))
            {
                indices.Enqueue(moveToBottom);
            }
        }

        return result;
    }
}
