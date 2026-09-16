using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfSubarraysThatMatchAPatternIIBenchmarks (ARCHITECTURE 17.9): both arms
// count the windows of nums whose consecutive signs equal pattern - the O(n*m) scan against the
// O(n + m) ZFunction search over the reduced sign texts - so a harness whose arms disagree is timing
// two different questions. The count is the problem's whole answer rather than a proxy.
//
// The fixture saturates that answer, so agreement alone would be weak: nums is strictly increasing and
// pattern is all 1s, so every window matches and both arms report the largest count the work can have.
// The oracle below is derived from the workload's own layout (a matching window starts at every
// i with i + patternLength < numsLength) rather than from either arm, so the test still fails if an
// arm drops windows - the failure bare agreement could not separate from "both found the same
// saturated count".
public sealed partial class NumberOfSubarraysThatMatchAPatternIIBenchmarksTests
{
    private const int SmallestNumsLength = 1_000;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_EveryWindowMatches_AgreesWithZFunction()
    {
        var harness = BuildHarness();
        var bruteForce = harness.BruteForce();

        Assert.Equal(ExpectedAllMatchingWindowCount(harness), bruteForce);
        Assert.Equal(bruteForce, harness.ZFunction());
    }

    [Fact]
    public void ZFunction_EveryWindowMatches_AgreesWithBruteForce()
    {
        var harness = BuildHarness();
        var zFunction = harness.ZFunction();

        Assert.Equal(ExpectedAllMatchingWindowCount(harness), zFunction);
        Assert.Equal(zFunction, harness.BruteForce());
    }

    private static NumberOfSubarraysThatMatchAPatternIIBenchmarks BuildHarness()
    {
        var harness = new NumberOfSubarraysThatMatchAPatternIIBenchmarks { NumsLength = SmallestNumsLength };
        harness.Setup();

        return harness;
    }

    // A strictly increasing nums makes every consecutive sign 1, and an all-1s pattern of half the
    // length therefore matches at every start i with i + patternLength < numsLength.
    private static int ExpectedAllMatchingWindowCount(NumberOfSubarraysThatMatchAPatternIIBenchmarks harness) =>
        harness.NumsLength - harness.NumsLength / 2;
}
