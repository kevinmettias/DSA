using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PeaksInArrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// PeaksInArraySolution's, competing answerers for the same query script - a rescan of the queried
// range per count query against a Fenwick tree of peak indicators - so a harness whose arms
// disagree is timing two different problems. Setup builds both nums and the query script from one
// fixed seed, so the same Length must rebuild both, and both arms replay the same script in the
// same order, updating the same positions.
public sealed partial class PeaksInArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(AnswerText.Of(BuildHarness().BruteForce()), AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AlternatingQueryScript_AgreesWithFenwickTree()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.FenwickTree()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void FenwickTree_AlternatingQueryScript_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.FenwickTree()));
    }

    private static PeaksInArrayBenchmarks BuildHarness()
    {
        var harness = new PeaksInArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
