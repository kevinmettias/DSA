using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RevealCardsInIncreasingOrderBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; the class's own problem statement is the independent oracle the baseline's
// answer is checked against: running the reveal/move-to-bottom process over an answer must produce
// the deck's values in non-decreasing order, which no arm can satisfy by agreeing with the other
// about a wrong ordering.
public sealed partial class RevealCardsInIncreasingOrderBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void ListRemoveAtSimulation_SeededDeck_RevealsInIncreasingOrder() =>
        Assert.True(IsRevealedInIncreasingOrder(BuildHarness().ListRemoveAtSimulation()));

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
