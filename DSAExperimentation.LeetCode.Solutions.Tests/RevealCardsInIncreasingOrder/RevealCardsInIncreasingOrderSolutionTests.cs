using DSAExperimentation.LeetCode.Solutions.RevealCardsInIncreasingOrder;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RevealCardsInIncreasingOrder;

// Harness only: both strategies live in RevealCardsInIncreasingOrderSolution and are
// asserted against the same cases, among them the single-card deck where the
// move-to-bottom step never runs and the two-card deck where it runs exactly once.
public sealed partial class RevealCardsInIncreasingOrderSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [11], [11] },
            { [1, 2, 3, 4], [1, 3, 2, 4] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void DeckRevealedIncreasingByListRemoveAt_Cases_ReturnsOrderThatRevealsSorted(
        int[] deck, int[] expected) =>
        Assert.Equal(expected, RevealCardsInIncreasingOrderSolution.DeckRevealedIncreasingByListRemoveAt(deck));

    [Theory]
    [MemberData(nameof(Cases))]
    public void DeckRevealedIncreasingByQueueRotation_Cases_ReturnsOrderThatRevealsSorted(
        int[] deck, int[] expected) =>
        Assert.Equal(expected, RevealCardsInIncreasingOrderSolution.DeckRevealedIncreasingByQueueRotation(deck));
}
