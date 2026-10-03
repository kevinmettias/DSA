using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PeekingIteratorBenchmarks (ARCHITECTURE 17.9): its two arms are
// PeekingIteratorSolution's, competing backings for the same peek-peek-next contract - a raw index
// plus buffered value against the repo's own queue, whose peek-and-dequeue pair is already the
// contract - so a harness whose arms disagree is timing two different problems. Each arm builds its
// own iterator inside the call, so one harness is safe to call in either order. Setup builds the
// source sequence from Length alone, and every Peek and Next answer the drain returns is fixed by
// that sequence, so the tests pin the answers themselves.
public sealed partial class PeekingIteratorBenchmarksTests
{
    private const int SmallestLength = 200;

    // Drain peeks then takes every element, so each of 1..Length is answered twice in a row.
    private const int AnswersPerElement = 2;

    // The sum of those answers: each of 1..Length twice.
    private const long ExpectedDrainedTotal = (long)SmallestLength * (SmallestLength + 1);

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().IndexTracked(), BuildHarness().IndexTracked());

    [Fact]
    public void IndexTracked_SequentialValues_PeeksThenTakesEveryElement()
    {
        var answers = BuildHarness().IndexTracked();

        Assert.Equal(ExpectedAnswers(), answers);
        Assert.Equal(ExpectedDrainedTotal, answers.Sum(answer => (long)answer));
    }

    [Fact]
    public void QueuePrimitive_SequentialValues_PeeksThenTakesEveryElement()
    {
        var answers = BuildHarness().QueuePrimitive();

        Assert.Equal(ExpectedAnswers(), answers);
        Assert.Equal(ExpectedDrainedTotal, answers.Sum(answer => (long)answer));
    }

    private static int[] ExpectedAnswers() =>
        [.. Enumerable.Range(1, SmallestLength).SelectMany(value => Enumerable.Repeat(value, AnswersPerElement))];

    private static PeekingIteratorBenchmarks BuildHarness()
    {
        var harness = new PeekingIteratorBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
