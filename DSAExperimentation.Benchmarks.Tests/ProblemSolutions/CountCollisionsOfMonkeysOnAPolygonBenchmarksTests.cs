using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountCollisionsOfMonkeysOnAPolygonBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - raising the configuration count once per
// monkey against raising it by squaring - so a harness whose arms disagree is timing two different
// problems. Both arms return an int, so they are compared directly. The class carries no
// [GlobalSetup] - the monkey count is the whole workload - so the closed form the class comment
// states, (2^n - 2) mod 1e9+7, is asserted from the arm tests at the smallest [Params] count,
// computed here independently of both arms.
public sealed partial class CountCollisionsOfMonkeysOnAPolygonBenchmarksTests
{
    private const int SmallestMonkeyCount = 1_000;
    private const int WaysForOneThousandMonkeys = 688423208;

    [Fact]
    public void RepeatedMultiplication_OneThousandMonkeys_AgreesWithExponentiationBySquaringAndTheClosedForm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ExponentiationBySquaring(), harness.RepeatedMultiplication());
        Assert.Equal(WaysForOneThousandMonkeys, harness.RepeatedMultiplication());
    }

    [Fact]
    public void ExponentiationBySquaring_OneThousandMonkeys_AgreesWithRepeatedMultiplicationAndTheClosedForm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedMultiplication(), harness.ExponentiationBySquaring());
        Assert.Equal(WaysForOneThousandMonkeys, harness.ExponentiationBySquaring());
    }

    private static CountCollisionsOfMonkeysOnAPolygonBenchmarks BuildHarness() =>
        new() { MonkeyCount = SmallestMonkeyCount };
}
