using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for QueueReconstructionByHeightBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the reconstructed queue itself, which is what
// LeetCode asks for - so a harness whose arms disagree is timing two different problems. Both arms
// return the queue as an int[][], and Assert.Equal on a jagged array falls back to reference
// equality for the inner arrays, so AnswerText.Of is what actually compares them. Setup draws the
// (height, k) pairs from one fixed seed, so the same Length must rebuild the same pairs.
public sealed partial class QueueReconstructionByHeightBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ArraySortListInsert()),
            AnswerText.Of(BuildHarness().ArraySortListInsert()));

    [Fact]
    public void ArraySortListInsert_SeededPeopleBatch_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.ArraySortListInsert()),
            AnswerText.Of(harness.MergeSortDynamicArrayInsert()));
    }

    [Fact]
    public void MergeSortDynamicArrayInsert_SeededPeopleBatch_AgreesWithTheArraySortArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MergeSortDynamicArrayInsert()),
            AnswerText.Of(harness.ArraySortListInsert()));
    }

    private static QueueReconstructionByHeightBenchmarks BuildHarness()
    {
        var harness = new QueueReconstructionByHeightBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
