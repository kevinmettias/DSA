using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumAndMinimumSumsOfAtMostSizeKSubsequencesBenchmarks (ARCHITECTURE 17.9):
// both arms are competing strategies for one question - the summed maximum and minimum over every
// subsequence of at most k elements - so a harness whose arms disagree is timing two different
// problems. Setup draws the array from a fixed seed, so the same length must rebuild the same
// workload; neither arm mutates it, and k is the class's own fixed bound.
public sealed partial class MaximumAndMinimumSumsOfAtMostSizeKSubsequencesBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().PascalTriangle(), BuildHarness().PascalTriangle());

    [Fact]
    public void PascalTriangle_AgreesWithFactorialCombinatorics()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PascalTriangle(), harness.FactorialCombinatorics());
    }

    [Fact]
    public void FactorialCombinatorics_AgreesWithPascalTriangle()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FactorialCombinatorics(), harness.PascalTriangle());
    }

    private static MaximumAndMinimumSumsOfAtMostSizeKSubsequencesBenchmarks BuildHarness()
    {
        var harness = new MaximumAndMinimumSumsOfAtMostSizeKSubsequencesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
