using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfStringsWhichCanBeRearrangedToContainSubstringBenchmarks (ARCHITECTURE
// 17.9): both arms return the same count of rearrangeable strings for one length - the O(n) state DP
// against the closed form through modular exponentiation - so a harness whose arms disagree is timing
// two different questions. The count modulo 1e9+7 is the problem's whole answer rather than a proxy.
// This class has no Setup: its whole workload is the [Params] axis, so the same StringLength is the
// whole of what a rebuild can vary.
public sealed partial class NumberOfStringsWhichCanBeRearrangedToContainSubstringBenchmarksTests
{
    private const int SmallestStringLength = 1_000;

    [Fact]
    public void StateDp_AgreesWithInclusionExclusion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InclusionExclusion(), harness.StateDp());
    }

    [Fact]
    public void InclusionExclusion_AgreesWithStateDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StateDp(), harness.InclusionExclusion());
    }

    private static NumberOfStringsWhichCanBeRearrangedToContainSubstringBenchmarks BuildHarness() =>
        new() { StringLength = SmallestStringLength };
}
