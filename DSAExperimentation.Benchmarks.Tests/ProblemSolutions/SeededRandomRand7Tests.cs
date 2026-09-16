using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementRand10UsingRand7Benchmarks's nested SeededRandomRand7.
//
// SeededRandomRand7 is a private nested type, so no test class can name it in code. Coverage is
// attributed by TYPE, so the nested unit needs a class named after it, and the only surface that
// runs SeededRandomRand7.Draw is the outer benchmark's arm, which passes the Setup-built Rand7 into
// the solution. These tests drive that arm and assert what Draw must be doing.
//
// The benchmark's own comment says its arms are not competing strategies - NaiveModuloFold is the
// "fast but wrong" contrast, not a correctness baseline - so this class does not assert that they
// agree. It uses the accepted arm, RejectionSampling, purely as the way to observe Draw: with Calls
// set to one, an arm call returns exactly one Draw-derived value, so repeated calls read out Draw's
// stream. Both facts below are properties of Draw rather than of the arm - a Draw that returned a
// value outside 1..7 would push the fold outside 1..10, and Draw's seeded stream must keep covering
// all seven values rather than collapsing onto a subset.
public sealed partial class SeededRandomRand7Tests
{
    private const int OneDrawPerCall = 1;
    private const int SmallestCallCount = 1_000;
    private const int ObservedDrawCount = 1_000;
    private const int LowestRand10 = 1;
    private const int HighestRand10 = 10;

    [Fact]
    public void Draw_OneThousandObservedCalls_KeepsEveryValueInsideTheRand10Contract()
    {
        var observed = Observe(BuildHarness(OneDrawPerCall), ObservedDrawCount);

        Assert.All(observed, value => Assert.InRange(value, LowestRand10, HighestRand10));
    }

    [Fact]
    public void Draw_OneThousandObservedCalls_ReachesEveryRand10Value()
    {
        var observed = Observe(BuildHarness(OneDrawPerCall), ObservedDrawCount);

        Assert.Equal(Enumerable.Range(LowestRand10, HighestRand10), observed.Distinct().Order());
    }

    [Fact]
    public void Draw_TwoFreshHarnessesAtTheSameSeed_ReplayTheSameStream() =>
        Assert.Equal(BuildHarness(SmallestCallCount).RejectionSampling(), BuildHarness(SmallestCallCount).RejectionSampling());

    private static List<int> Observe(ImplementRand10UsingRand7Benchmarks harness, int count)
    {
        var observed = new List<int>();

        for (var i = 0; i < count; i++)
        {
            observed.Add(harness.RejectionSampling());
        }

        return observed;
    }

    private static ImplementRand10UsingRand7Benchmarks BuildHarness(int calls)
    {
        var harness = new ImplementRand10UsingRand7Benchmarks { Calls = calls };
        harness.Setup();

        return harness;
    }
}
