using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeToMakeArraySumAtMostXBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the fewest seconds until the growing sum drops to the
// target - so a harness whose arms disagree is timing two different problems. Both arms run the same
// knapsack recurrence over the pairs sorted ascending by nums2 and differ only in the DP's footprint,
// so agreement proves the collapsed rolling row still holds the value the dense table's final row
// does. Note the target is deliberately set below any reachable sum, which forces the full recurrence
// but also means both arms are expected to answer "no schedule": that agreement is weak by
// construction, since an arm that got the recurrence wrong could still fail to find a schedule. Setup
// draws both arrays from one fixed seed, so the same Length must rebuild the same pair.
public sealed partial class MinimumTimeToMakeArraySumAtMostXBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RollingKnapsack(), BuildHarness().RollingKnapsack());

    [Fact]
    public void DenseTable_SameArrayPairs_AgreesWithRollingKnapsack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RollingKnapsack(), harness.DenseTable());
    }

    [Fact]
    public void RollingKnapsack_SameArrayPairs_AgreesWithDenseTable()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DenseTable(), harness.RollingKnapsack());
    }

    private static MinimumTimeToMakeArraySumAtMostXBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeToMakeArraySumAtMostXBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
