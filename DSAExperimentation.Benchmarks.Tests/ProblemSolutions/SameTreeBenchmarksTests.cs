using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SameTreeBenchmarks (ARCHITECTURE 17.9): the class has two arms - the recursive
// compare and the iterative stack compare - so the pair must agree as well as each matching the
// decisive answer for the built trees. It also carries no [Params]; the whole workload is the pair of
// trees [GlobalSetup] builds, so the only thing to construct is the harness and then call Setup.
//
// [GlobalSetup] builds both trees with the same private helper, so the two are structurally equal
// with equal values at every node - a pair the answer is decisively true for, which is asserted here
// rather than read back out of either arm. Neither tree is mutated by the comparison, so one harness
// is safe to call any number of times.
public sealed partial class SameTreeBenchmarksTests
{
    [Fact]
    public void Setup_SameTrees_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsSameByRecursiveCompare(),
            BuildHarness().IsSameByRecursiveCompare());

    [Fact]
    public void IsSameByRecursiveCompare_IdenticallyBuiltTrees_ReportsThemTheSame() =>
        Assert.True(BuildHarness().IsSameByRecursiveCompare());

    [Fact]
    public void IterativeStackCompare_IdenticallyBuiltTrees_ReportsThemTheSame() =>
        Assert.True(BuildHarness().IterativeStackCompare());

    [Fact]
    public void IterativeStackCompare_AgreesWithIsSameByRecursiveCompare()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsSameByRecursiveCompare(), harness.IterativeStackCompare());
    }

    private static SameTreeBenchmarks BuildHarness()
    {
        var harness = new SameTreeBenchmarks();
        harness.Setup();

        return harness;
    }
}
