using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MakeArrayEmptyBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the O(n) present-count scan against this repo's own
// FenwickTree range query over the surviving positions - so a harness whose arms disagree is timing
// two different problems. Both arms return the operation count as a long. Setup shuffles a distinct
// value run from one fixed seed, so the same Length must rebuild the same permutation and the same
// count.
public sealed partial class MakeArrayEmptyBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
        Assert.Equal(BuildHarness().FenwickTreeSweep(), BuildHarness().FenwickTreeSweep());
    }

    [Fact]
    public void LinearScan_ShuffledDistinctRun_AgreesWithFenwickTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeSweep(), harness.LinearScan());
    }

    [Fact]
    public void FenwickTreeSweep_ShuffledDistinctRun_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.FenwickTreeSweep());
    }

    private static MakeArrayEmptyBenchmarks BuildHarness()
    {
        var harness = new MakeArrayEmptyBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
