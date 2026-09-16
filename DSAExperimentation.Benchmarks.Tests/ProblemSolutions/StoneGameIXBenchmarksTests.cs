using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameIXBenchmarks (ARCHITECTURE 17.9): both arms answer the same
// question - whether Alice wins LC 2029 on one stones array - one by a memoized negamax over
// the remaining (count0, count1, count2, runningSumMod3) state, one by a closed-form parity
// rule over the same value remainders, so a harness whose arms disagree is timing two
// different problems. Setup's stones are seeded, so the same length must rebuild the same
// workload.
//
// The arms' shared return type is a lone bool, so agreement here witnesses that both
// strategies reach the same verdict - it cannot witness how they got there, and for a
// fixture whose verdict is a single bit that is the strongest check the arm shapes allow.
public sealed partial class StoneGameIXBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CanAliceWinByClosedFormCounting()),
            AnswerText.Of(BuildHarness().CanAliceWinByClosedFormCounting()));

    [Fact]
    public void CanAliceWinByGameTreeMinimax_AgreesWithCanAliceWinByClosedFormCounting()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanAliceWinByGameTreeMinimax(),
            harness.CanAliceWinByClosedFormCounting());
    }

    [Fact]
    public void CanAliceWinByClosedFormCounting_AgreesWithCanAliceWinByGameTreeMinimax()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanAliceWinByClosedFormCounting(),
            harness.CanAliceWinByGameTreeMinimax());
    }

    private static StoneGameIXBenchmarks BuildHarness()
    {
        var harness = new StoneGameIXBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
