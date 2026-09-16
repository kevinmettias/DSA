using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestIncreasingSubsequenceBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the textbook O(n^2) dynamic program against
// patience sorting with a binary search over the tails - so a harness whose arms disagree is
// timing two different problems. Both arms return the subsequence length, a scalar compared
// directly. Setup draws values from a fixed seed, so the same Length must rebuild the same array;
// the length itself depends on those draws, so the arms are compared against each other alone.
public sealed partial class LongestIncreasingSubsequenceBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().PatienceSortingBinarySearch(),
            BuildHarness().PatienceSortingBinarySearch());

    [Fact]
    public void DynamicProgramming_SmallestLength_AgreesWithPatienceSortingBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PatienceSortingBinarySearch(), harness.DynamicProgramming());
    }

    [Fact]
    public void PatienceSortingBinarySearch_SmallestLength_AgreesWithDynamicProgramming()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DynamicProgramming(), harness.PatienceSortingBinarySearch());
    }

    private static LongestIncreasingSubsequenceBenchmarks BuildHarness()
    {
        var harness = new LongestIncreasingSubsequenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
