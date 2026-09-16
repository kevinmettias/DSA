using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for Finding3DigitEvenNumbersBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. Setup draws the digit array from a fixed seed, so the same Length must rebuild
// the same workload.
//
// LeetCode 2094 asks for the numbers in ascending order with duplicates removed, and both arms sort
// before returning, so the order of the returned array is pinned by the problem and
// AnswerText.Of's order-sensitive rendering is the right comparison.
public sealed partial class Finding3DigitEvenNumbersBenchmarksTests
{
    private const int SmallestLength = 30;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ListContainsScanDedupe()),
            AnswerText.Of(BuildHarness().ListContainsScanDedupe()));

    [Fact]
    public void ListContainsScanDedupe_AgreesWithSetDedupeThenMergeSort()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SetDedupeThenMergeSort()),
            AnswerText.Of(harness.ListContainsScanDedupe()));
    }

    [Fact]
    public void SetDedupeThenMergeSort_AgreesWithListContainsScanDedupe()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.ListContainsScanDedupe()),
            AnswerText.Of(harness.SetDedupeThenMergeSort()));
    }

    private static Finding3DigitEvenNumbersBenchmarks BuildHarness()
    {
        var harness = new Finding3DigitEvenNumbersBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
