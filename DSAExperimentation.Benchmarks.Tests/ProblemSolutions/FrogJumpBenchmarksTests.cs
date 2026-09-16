using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FrogJumpBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question, so a harness whose arms disagree is timing two different problems. Setup
// derives the stone array from StoneCount alone, so the same StoneCount must rebuild the same
// workload.
//
// The agreement is weak by construction and the assertions say only what the fixture supports: the
// stones are the consecutive run 0..StoneCount - 2 with one unreachable stone appended a fixed gap
// beyond it, so nothing can ever land on the last stone and both arms answer false. The comparison
// catches an arm that ever says true, but both arms reaching the same false is not evidence that
// either walked the whole exponential search the workload exists to force - the false verdict
// assertions are what pin them to the verdict the fixture guarantees.
public sealed partial class FrogJumpBenchmarksTests
{
    private const int SmallestStoneCount = 10;
    private const bool ExpectedCrossingVerdict = false;

    [Fact]
    public void Setup_SameStoneCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CanCrossByRecursiveBruteForce()),
            AnswerText.Of(BuildHarness().CanCrossByRecursiveBruteForce()));

    [Fact]
    public void CanCrossByRecursiveBruteForce_AgreesWithCanCrossByHashMapDynamicProgramming()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedCrossingVerdict, harness.CanCrossByRecursiveBruteForce());
        Assert.Equal(harness.CanCrossByHashMapDynamicProgramming(), harness.CanCrossByRecursiveBruteForce());
    }

    [Fact]
    public void CanCrossByHashMapDynamicProgramming_AgreesWithCanCrossByRecursiveBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedCrossingVerdict, harness.CanCrossByHashMapDynamicProgramming());
        Assert.Equal(harness.CanCrossByRecursiveBruteForce(), harness.CanCrossByHashMapDynamicProgramming());
    }

    private static FrogJumpBenchmarks BuildHarness()
    {
        var harness = new FrogJumpBenchmarks { StoneCount = SmallestStoneCount };
        harness.Setup();

        return harness;
    }
}
