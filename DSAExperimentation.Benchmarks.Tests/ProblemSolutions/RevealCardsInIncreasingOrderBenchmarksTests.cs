using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RevealCardsInIncreasingOrderBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a BCL List<int> with RemoveAt(0) standing in for
// "pop the front" against this repo's Queue<int>, whose front-dequeue/back-enqueue is literally the
// reveal and the move-to-bottom - so a harness whose arms disagree is timing two different problems.
// Setup draws the deck from one fixed seed, so the same Length must rebuild the same deck;
// otherwise two published numbers were never comparable in the first place.
//
// Both arms clone the deck before sorting it, so the hoisted deck is never written to and one
// harness is safe to call twice in either order. The class's own problem statement is the
// independent oracle the answer is checked against: running the reveal/move-to-bottom process over
// an answer must produce the deck's values in non-decreasing order, which no arm can satisfy by
// agreeing with the other about a wrong ordering.
public sealed partial class RevealCardsInIncreasingOrderBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ListRemoveAtSimulation()),
            AnswerText.Of(BuildHarness().ListRemoveAtSimulation()));

    [Fact]
    public void ListRemoveAtSimulation_SeededDeck_AgreesWithQueueSimulation()
    {
        var harness = BuildHarness();
        var ordered = harness.ListRemoveAtSimulation();

        Assert.True(IsRevealedInIncreasingOrder(ordered));
        Assert.Equal(AnswerText.Of(harness.QueueSimulation()), AnswerText.Of(ordered));
    }

    [Fact]
    public void QueueSimulation_SeededDeck_AgreesWithListRemoveAtSimulation()
    {
        var harness = BuildHarness();
        var ordered = harness.QueueSimulation();

        Assert.True(IsRevealedInIncreasingOrder(ordered));
        Assert.Equal(AnswerText.Of(harness.ListRemoveAtSimulation()), AnswerText.Of(ordered));
    }

    // Runs the problem's own process over the candidate ordering: reveal the front, then move the
    // next card to the bottom, until nothing is left. The revealed values must come out
    // non-decreasing for the ordering to be the answer the problem asks for.
    private static bool IsRevealedInIncreasingOrder(int[] ordered)
    {
        var pending = new Queue<int>(ordered);
        var previous = int.MinValue;

        while (pending.Count > 0)
        {
            var revealed = pending.Dequeue();

            if (revealed < previous)
            {
                return false;
            }

            previous = revealed;

            if (pending.Count > 0)
            {
                pending.Enqueue(pending.Dequeue());
            }
        }

        return true;
    }

    private static RevealCardsInIncreasingOrderBenchmarks BuildHarness()
    {
        var harness = new RevealCardsInIncreasingOrderBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
