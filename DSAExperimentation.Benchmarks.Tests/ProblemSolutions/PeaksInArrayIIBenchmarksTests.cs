using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PeaksInArrayIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// PeaksInArrayIISolution's, competing answerers for the same query script - enumerating every
// candidate subarray and rescanning its interior against a segment tree of peak-gap structure - so
// a harness whose arms disagree is timing two different problems. Setup builds both nums and the
// query script from one fixed seed, so the same Length must rebuild both, and both arms replay the
// same script in the same order, updating the same positions.
public sealed partial class PeaksInArrayIIBenchmarksTests
{
    private const int SmallestLength = 30;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(AnswerText.Of(BuildHarness().BruteForce()), AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AlternatingQueryScript_AgreesWithSegmentTree()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.SegmentTree()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void SegmentTree_AlternatingQueryScript_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.SegmentTree()));
    }

    private static PeaksInArrayIIBenchmarks BuildHarness()
    {
        var harness = new PeaksInArrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
