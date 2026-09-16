using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameVIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the LC 1696 maximum score under a window of K - so a harness
// whose arms disagree is timing two different problems. The values are drawn over a wide signed
// range on purpose, so the window's running maximum keeps changing; agreement between the rescan
// and the deque is therefore agreement on a workload neither can settle on one dominant value.
public sealed partial class JumpGameVIBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameSignedValues() =>
        Assert.Equal(
            BuildHarness().RescanWindowEachPosition(),
            BuildHarness().RescanWindowEachPosition());

    [Fact]
    public void RescanWindowEachPosition_SeededSignedValues_AgreesWithMonotonicDequeDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicDequeDp(), harness.RescanWindowEachPosition());
    }

    [Fact]
    public void MonotonicDequeDp_SeededSignedValues_AgreesWithRescanWindowEachPosition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RescanWindowEachPosition(), harness.MonotonicDequeDp());
    }

    private static JumpGameVIBenchmarks BuildHarness()
    {
        var harness = new JumpGameVIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
