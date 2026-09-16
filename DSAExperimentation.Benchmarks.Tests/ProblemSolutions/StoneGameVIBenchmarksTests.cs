using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameVIBenchmarks (ARCHITECTURE 17.9): both arms run LC 1686's
// identical greedy and differ only in the sort primitive - the BCL's Array.Sort against this
// repo's own MergeSort - so a harness whose arms disagree is timing two different problems
// rather than two sorts of one. Setup's value arrays are seeded, so the same length must
// rebuild the same workload.
public sealed partial class StoneGameVIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ArraySortGreedy()),
            AnswerText.Of(BuildHarness().ArraySortGreedy()));

    [Fact]
    public void ArraySortGreedy_AgreesWithMergeSortGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArraySortGreedy(), harness.MergeSortGreedy());
    }

    [Fact]
    public void MergeSortGreedy_AgreesWithArraySortGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortGreedy(), harness.ArraySortGreedy());
    }

    private static StoneGameVIBenchmarks BuildHarness()
    {
        var harness = new StoneGameVIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
