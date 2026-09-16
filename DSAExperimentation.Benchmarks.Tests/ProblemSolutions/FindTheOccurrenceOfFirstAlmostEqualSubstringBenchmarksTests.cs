using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheOccurrenceOfFirstAlmostEqualSubstringBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for one question - rescanning every window for its
// mismatches against one Z-function pass over text#pattern - so a harness whose arms disagree is
// timing two different problems. Setup builds text and pattern from Length, so the same Length
// must rebuild the same pair. Both answers are an index, and the workload is built so every window
// mismatches twice, which pins them both at -1.
public sealed partial class FindTheOccurrenceOfFirstAlmostEqualSubstringBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithZFunction()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ZFunction(), harness.BruteForce());
    }

    [Fact]
    public void ZFunction_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.ZFunction());
    }

    private static FindTheOccurrenceOfFirstAlmostEqualSubstringBenchmarks BuildHarness()
    {
        var harness = new FindTheOccurrenceOfFirstAlmostEqualSubstringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
