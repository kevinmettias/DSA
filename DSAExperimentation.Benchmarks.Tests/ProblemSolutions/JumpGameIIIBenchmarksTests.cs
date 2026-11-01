using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameIIIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds
// the arms to agreeing on the smallest workload, so this asserts only the verdict the fixture fixes
// independently of either arm.
//
// Setup draws every value from [1, Length), so no index holds 0 and LC 1306's only success
// condition is never met: the verdict is decisively false, and the class comment's "force the
// real worst case" intent means neither arm may short-circuit on an early hit - both must exhaust
// the whole reachable component before answering.
public sealed partial class JumpGameIIIBenchmarksTests
{
    private const int SmallestLength = 200;
    private const bool ExpectedVerdict = false;

    [Fact]
    public void CanReachByStackWalk_ZeroFreeArray_FindsNoZero() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().CanReachByStackWalk());

    private static JumpGameIIIBenchmarks BuildHarness()
    {
        var harness = new JumpGameIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
