using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSumOfMNonOverlappingSubarraysIIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the length-window DP against the Lagrangian
// relaxation that binary-searches the per-subarray penalty - so a harness whose arms disagree is
// timing two different problems. Setup draws the values from one fixed seed and fixes the subarray
// budget and window bounds, so the same Length must rebuild the same workload; otherwise two
// published numbers were never comparable in the first place.
public sealed partial class MaximumSumOfMNonOverlappingSubarraysIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DynamicProgramming(), BuildHarness().DynamicProgramming());

    [Fact]
    public void DynamicProgramming_QuarterOfLengthAsSubarrayBudget_AgreesWithLagrangianRelaxation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LagrangianRelaxation(), harness.DynamicProgramming());
    }

    [Fact]
    public void LagrangianRelaxation_QuarterOfLengthAsSubarrayBudget_AgreesWithDynamicProgramming()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DynamicProgramming(), harness.LagrangianRelaxation());
    }

    private static MaximumSumOfMNonOverlappingSubarraysIIBenchmarks BuildHarness()
    {
        var harness = new MaximumSumOfMNonOverlappingSubarraysIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
