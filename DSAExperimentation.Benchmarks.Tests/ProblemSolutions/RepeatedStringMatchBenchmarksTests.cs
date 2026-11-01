using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RepeatedStringMatchBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the answer the workload is built to have. The repeated unit is a
// run of 'a' ending in 'b' and the target is a run of 'a' ending in 'c', so no repetition of the unit
// ever contains the target, every candidate length is exhausted, and the answer is the "no repeat
// count" this problem reserves for a target that never occurs.
public sealed partial class RepeatedStringMatchBenchmarksTests
{
    private const int SmallestLength = 200;

    // LeetCode 686 has no repeat count for a target that never occurs.
    private const int ExpectedNoRepeatCount = -1;

    [Fact]
    public void StringContains_TargetNeverOccurs_ReturnsNoRepeatCount() =>
        Assert.Equal(ExpectedNoRepeatCount, BuildHarness().StringContains());

    private static RepeatedStringMatchBenchmarks BuildHarness()
    {
        var harness = new RepeatedStringMatchBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
