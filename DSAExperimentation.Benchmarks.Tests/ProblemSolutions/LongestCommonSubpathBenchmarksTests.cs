using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestCommonSubpathBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - materializing every window's key and intersecting
// the sets against rolling-hash binary search - so a harness whose arms disagree is timing two
// different problems. Both arms return the longest shared run's length, a scalar compared
// directly. Setup draws every path from a fixed seed over a deliberately tiny alphabet, which is
// what makes long shared runs occur; the same PathLength must rebuild the same paths.
public sealed partial class LongestCommonSubpathBenchmarksTests
{
    private const int SmallestPathLength = 100;

    [Fact]
    public void Setup_SmallestPathLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().RollingHashBinarySearch(),
            BuildHarness().RollingHashBinarySearch());

    [Fact]
    public void NaiveKeyedIntersection_SmallestPathLength_AgreesWithRollingHashBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RollingHashBinarySearch(), harness.NaiveKeyedIntersection());
    }

    [Fact]
    public void RollingHashBinarySearch_SmallestPathLength_AgreesWithNaiveKeyedIntersection()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveKeyedIntersection(), harness.RollingHashBinarySearch());
    }

    private static LongestCommonSubpathBenchmarks BuildHarness()
    {
        var harness = new LongestCommonSubpathBenchmarks { PathLength = SmallestPathLength };
        harness.Setup();

        return harness;
    }
}
