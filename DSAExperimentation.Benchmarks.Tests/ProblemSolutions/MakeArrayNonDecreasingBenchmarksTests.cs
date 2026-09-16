using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MakeArrayNonDecreasingBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the memoized prefix DP that tries every split point
// against the greedy scan that keeps the longest run of values it can extend - so a harness whose
// arms disagree is timing two different problems. Both arms return the largest achievable size.
// Setup draws the values from one fixed seed, so the same Length must rebuild the same array and
// the same size.
public sealed partial class MakeArrayNonDecreasingBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().PrefixDynamicProgramming(), BuildHarness().PrefixDynamicProgramming());
        Assert.Equal(BuildHarness().GreedyScan(), BuildHarness().GreedyScan());
    }

    [Fact]
    public void PrefixDynamicProgramming_SeededValueRun_AgreesWithGreedyScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GreedyScan(), harness.PrefixDynamicProgramming());
    }

    [Fact]
    public void GreedyScan_SeededValueRun_AgreesWithPrefixDynamicProgramming()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrefixDynamicProgramming(), harness.GreedyScan());
    }

    private static MakeArrayNonDecreasingBenchmarks BuildHarness()
    {
        var harness = new MakeArrayNonDecreasingBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
