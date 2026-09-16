using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HIndexBenchmarks (ARCHITECTURE 17.9): both arms are HIndexSolution's - the
// O(n^2) trial of every candidate h against this repo's MergeSort followed by one descending scan -
// so a harness whose arms disagree is timing two different questions. Both arms answer with the
// index itself, so they are compared as integers, and neither needs a decisive literal: the
// citation array is drawn at random, so the h-index is a property of the fixture rather than a
// value the class comment names. Setup draws that array off one seed, so the same Length must
// rebuild the same citations and the same index.
public sealed partial class HIndexBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededCitations_AgreesWithMergeSortScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortScan(), harness.BruteForce());
    }

    [Fact]
    public void MergeSortScan_SeededCitations_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MergeSortScan());
    }

    private static HIndexBenchmarks BuildHarness()
    {
        var harness = new HIndexBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
