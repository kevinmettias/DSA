using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RotateStringBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the verdict the workload is built to have. The source is a run of 'a' ending in 'b' and
// the goal is a run of 'a' ending in 'c'. A rotation of the source is a cyclic shift of it, so every
// rotation still holds exactly one 'b' and no 'c' at all - the goal is not a rotation of the source,
// which is the decisive answer asserted here rather than read back out of an arm.
public sealed partial class RotateStringBenchmarksTests
{
    private const int SmallestLength = 10;

    [Fact]
    public void CanRotateByNaiveScan_GoalOutsideTheRotationSet_ReportsNoRotation() =>
        Assert.False(BuildHarness().CanRotateByNaiveScan());

    private static RotateStringBenchmarks BuildHarness()
    {
        var harness = new RotateStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
