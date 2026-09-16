using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementRand10UsingRand7Benchmarks (ARCHITECTURE 17.9). This harness's
// two arms are NOT competing strategies for one answer: NaiveModuloFold is the deliberately
// wrong contrast the solution's own comment names (a single draw folded by ten can never leave
// the die's own 1..7 range), while RejectionSampling is the only LeetCode-accepted algorithm. So
// the arms are not asserted to agree; each is asserted against its own declared contract over a
// fixed batch of calls. Those contracts are decisive here: because the fold's expression
// `1 + (draw - 1) % 10` is the identity for every draw a Rand7 can produce, the fold's smallest
// and largest observed values pin the die's range exactly - which is also the only observable
// route to the private stand-in's Draw - and a uniform 1..10 must reach ten, which the fold can
// never do.
public sealed partial class ImplementRand10UsingRand7BenchmarksTests
{
    private const int SmallestCalls = 1_000;

    // Enough calls that the extremes of a correct uniform range are certain to appear: over this
    // many samples a missing 1 or 10 is a one-in-a-billion event. The stream is seeded, so the
    // observed values are fixed from run to run either way.
    private const int SampleCount = 200;

    private const int MinimumDieValue = 1;
    private const int Rand7Maximum = 7;
    private const int Rand10Maximum = 10;

    [Fact]
    public void Setup_SameCalls_RebuildsTheSameDrawStream()
    {
        Assert.Equal(BuildHarness().NaiveModuloFold(), BuildHarness().NaiveModuloFold());
        Assert.Equal(BuildHarness().RejectionSampling(), BuildHarness().RejectionSampling());
    }

    [Fact]
    public void NaiveModuloFold_OneDrawFoldedByTen_StaysInsideTheDieRange()
    {
        var harness = BuildHarness();
        var observed = new SortedSet<int>();

        for (var call = 0; call < SampleCount; call++)
        {
            observed.Add(harness.NaiveModuloFold());
        }

        Assert.Equal(MinimumDieValue, observed.Min);
        Assert.Equal(Rand7Maximum, observed.Max);
    }

    [Fact]
    public void RejectionSampling_UniformOverTen_ReachesValuesTheFoldCannotProduce()
    {
        var harness = BuildHarness();
        var observed = new SortedSet<int>();

        for (var call = 0; call < SampleCount; call++)
        {
            observed.Add(harness.RejectionSampling());
        }

        Assert.Equal(MinimumDieValue, observed.Min);
        Assert.Equal(Rand10Maximum, observed.Max);
    }

    // The stand-in is a private nested type, so Draw is only observable through the two arms that
    // call it - and only the fold's own arithmetic exposes it: `1 + (draw - 1) % 10` returns the
    // draw itself for every value a Rand7 can produce, so the fold's observed floor and ceiling
    // are the die's, and a draw outside 1..7 would move them.
    [Fact]
    public void Draw_SeededRand7StandIn_DrawsOnlyInsideTheDieRange()
    {
        var harness = BuildHarness();
        var draws = new SortedSet<int>();

        for (var call = 0; call < SampleCount; call++)
        {
            draws.Add(harness.NaiveModuloFold());
        }

        Assert.Equal(MinimumDieValue, draws.Min);
        Assert.Equal(Rand7Maximum, draws.Max);
    }

    private static ImplementRand10UsingRand7Benchmarks BuildHarness()
    {
        var harness = new ImplementRand10UsingRand7Benchmarks { Calls = SmallestCalls };
        harness.Setup();

        return harness;
    }
}
