using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameVIIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds
// the arms to agreeing on the smallest workload, so this asserts only the verdict the fixture fixes
// independently of either arm.
//
// Setup fills the string with '0' except for a blocked '1' in the final position, and LC 1871
// only allows landing on a '0'. The last index is therefore unreachable and the verdict is
// decisively false - which is exactly the class comment's point: neither arm can short-circuit on
// an early success, so both must exhaust every reachable index before answering, and the
// unmemoized recursion's call-count blowup is real work rather than a bailout.
public sealed partial class JumpGameVIIBenchmarksTests
{
    private const int SmallestStringLength = 28;
    private const bool ExpectedVerdict = false;

    [Fact]
    public void CanReachByUnmemoizedRecursion_BlockedFinalCharacter_CannotReachTheEnd() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().CanReachByUnmemoizedRecursion());

    private static JumpGameVIIBenchmarks BuildHarness()
    {
        var harness = new JumpGameVIIBenchmarks { StringLength = SmallestStringLength };
        harness.Setup();

        return harness;
    }
}
