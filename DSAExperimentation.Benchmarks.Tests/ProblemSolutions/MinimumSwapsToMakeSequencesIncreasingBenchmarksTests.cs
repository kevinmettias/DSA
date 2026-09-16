using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumSwapsToMakeSequencesIncreasingBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the fewest same-index exchanges that make both
// sequences strictly increasing - so a harness whose arms disagree is timing two different problems.
// The fixture puts a {2i, 2i+1} pair at every index, which keeps both the keep and swap transitions
// legal at every step, so neither strategy gets a branch pruned away and the agreement has to hold
// across the full recurrence. Both arms read the one pair of arrays [GlobalSetup] built from a fixed
// seed, so the same Length must rebuild the same pair.
public sealed partial class MinimumSwapsToMakeSequencesIncreasingBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().Tabulation(), BuildHarness().Tabulation());

    [Fact]
    public void MemoizedTwoState_SamePairRun_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.MemoizedTwoState());
    }

    [Fact]
    public void Tabulation_SamePairRun_AgreesWithMemoizedTwoState()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedTwoState(), harness.Tabulation());
    }

    private static MinimumSwapsToMakeSequencesIncreasingBenchmarks BuildHarness()
    {
        var harness = new MinimumSwapsToMakeSequencesIncreasingBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
