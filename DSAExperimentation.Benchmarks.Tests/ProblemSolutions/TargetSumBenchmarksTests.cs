using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TargetSumBenchmarks (ARCHITECTURE 17.9): both arms are TargetSumSolution's - the
// 2^ElementCount un-memoized recursion against the same recursion behind this repo's Memoizer - so a
// harness whose arms disagree is timing two different questions. Both answer with a bare int, and
// [GlobalSetup] derives the target from the drawn values, so a rebuild at the same ElementCount has to
// produce the same draw, the same target, and therefore the same way count.
public sealed partial class TargetSumBenchmarksTests
{
    private const int SmallestElementCount = 18;

    [Fact]
    public void Setup_SameElementCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().UnmemoizedRecursion(),
            BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_SeededValuesAndTarget_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_SeededValuesAndTarget_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static TargetSumBenchmarks BuildHarness()
    {
        var harness = new TargetSumBenchmarks { ElementCount = SmallestElementCount };
        harness.Setup();

        return harness;
    }
}
