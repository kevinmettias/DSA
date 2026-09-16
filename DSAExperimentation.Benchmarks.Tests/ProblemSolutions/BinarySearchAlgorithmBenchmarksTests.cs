using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BinarySearchAlgorithmBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the linear scan against the halving search - so a
// harness whose arms disagree is timing two different problems. The workload fixture builds a sorted
// even-valued array and puts the target at the far end, so the scan arm cannot look fast by exiting
// early; Setup holds both the array and the target, so the same Length must rebuild both.
public sealed partial class BinarySearchAlgorithmBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_FarthestTargetSeededArray_AgreesWithBinarySearchFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchFind(), harness.LinearScan());
    }

    [Fact]
    public void BinarySearchFind_FarthestTargetSeededArray_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.BinarySearchFind());
    }

    private static BinarySearchAlgorithmBenchmarks BuildHarness()
    {
        var harness = new BinarySearchAlgorithmBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
