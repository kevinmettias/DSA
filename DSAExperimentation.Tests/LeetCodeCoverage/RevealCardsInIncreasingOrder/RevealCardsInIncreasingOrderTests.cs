using DSAExperimentation.LeetCode.RevealCardsInIncreasingOrder;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RevealCardsInIncreasingOrder;

// Harness only: both strategies live in RevealCardsInIncreasingOrderSolution and are
// asserted against the same examples, including the single-card deck where the
// move-to-bottom step never runs and the two-card deck where it runs exactly once.
public sealed class RevealCardsInIncreasingOrderTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [17, 13, 11, 2, 3, 5, 7], [2, 13, 3, 11, 5, 17, 7] },
            { [1, 1000], [1, 1000] },
            { [11], [11] },
            { [1, 2, 3, 4], [1, 3, 2, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeckRevealedIncreasingByListRemoveAt_LeetCodeExamples_ReturnsOrderThatRevealsSorted(
        int[] deck, int[] expected) =>
        Assert.Equal(expected, RevealCardsInIncreasingOrderSolution.DeckRevealedIncreasingByListRemoveAt(deck));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeckRevealedIncreasingByQueueRotation_LeetCodeExamples_ReturnsOrderThatRevealsSorted(
        int[] deck, int[] expected) =>
        Assert.Equal(expected, RevealCardsInIncreasingOrderSolution.DeckRevealedIncreasingByQueueRotation(deck));
}
