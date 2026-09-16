using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortedGcdPairQueriesBenchmarks (ARCHITECTURE 17.9): both arms answer the
// same query array over the same values - one by materializing and sorting every pair's gcd, the
// other from the sieve index Setup prebuilt - so a harness whose arms disagree is timing two
// different problems. Setup is a pure function of Length and its own fixed seed, so the same
// Length must rebuild the same values, the same queries and the same index.
//
// Each arm returns one answer per query in query order, and the problem pins no order of its own
// beyond that, so the order-sensitive rendering compares the whole answer.
public sealed partial class SortedGcdPairQueriesBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_FiftyValues_AgreesWithGcdCountingSieve()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.GcdCountingSieve()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void GcdCountingSieve_FiftyValues_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.GcdCountingSieve()));
    }

    private static SortedGcdPairQueriesBenchmarks BuildHarness()
    {
        var harness = new SortedGcdPairQueriesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
