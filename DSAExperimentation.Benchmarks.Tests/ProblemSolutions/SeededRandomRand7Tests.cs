using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementRand10UsingRand7Benchmarks's nested SeededRandomRand7.
//
// SeededRandomRand7 is a private nested type, so no test class can name it in code. Coverage is
// attributed by TYPE, so the nested unit needs a class named after it, and the only surface that
// runs SeededRandomRand7.Draw is the outer benchmark's arm, which passes the Setup-built Rand7 into
// the solution. These tests drive that arm and assert what Draw must be doing.
//
// The benchmark's two arms consume Draw's stream differently - the recycled sampler draws fewer times
// per value - so this class does not assert that they agree. It uses the textbook arm,
// RejectionSampling, purely as the way to observe Draw: one arm call returns one Draw-derived value
// per call it makes, so it reads out Draw's stream. Both facts below are properties of Draw rather
// than of the arm - a Draw that returned 0 would fold a value to 0, outside 1..10, and Draw's seeded
// stream must keep covering all seven values rather than collapsing onto a subset.
public sealed partial class SeededRandomRand7Tests
{
    private const int SmallestCallCount = 1_000;
    private const int LowestRand10 = 1;
    private const int HighestRand10 = 10;

    [Fact]
    public void Draw_OneThousandObservedCalls_KeepsEveryValueInsideTheRand10Contract()
    {
        var observed = BuildHarness(SmallestCallCount).RejectionSampling();

        Assert.All(observed, value => Assert.InRange(value, LowestRand10, HighestRand10));
    }

    [Fact]
    public void Draw_OneThousandObservedCalls_ReachesEveryRand10Value()
    {
        var observed = BuildHarness(SmallestCallCount).RejectionSampling();

        Assert.Equal(Enumerable.Range(LowestRand10, HighestRand10), observed.Distinct().Order());
    }

    [Fact]
    public void Draw_TwoFreshHarnessesAtTheSameSeed_ReplayTheSameStream() =>
        Assert.Equal(BuildHarness(SmallestCallCount).RejectionSampling(), BuildHarness(SmallestCallCount).RejectionSampling());

    private static ImplementRand10UsingRand7Benchmarks BuildHarness(int calls)
    {
        var harness = new ImplementRand10UsingRand7Benchmarks { Calls = calls };
        harness.Setup();

        return harness;
    }
}
