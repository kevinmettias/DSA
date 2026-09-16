using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumWindowSubstringBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - every O(n^2) start/end pair against the sliding-window hash map -
// so a harness whose arms disagree is timing two different problems. Both arms only read the generated
// text, so one harness instance is safe to call twice in either order. Setup builds that text from one
// fixed seed, so the same Length must rebuild the same text; otherwise two published numbers were never
// comparable in the first place.
//
// WEAK AGREEMENT, by construction. The fixture's target is three characters that deliberately do not
// occur in the text, so neither strategy can satisfy "missing == 0" and both answer with the no-window
// answer. Agreement therefore witnesses "both report no window" - it would catch an arm that returned a
// spurious window, but it says nothing about whether the two arms agree on a window they could both
// find, because this workload never has one. That is the workload the benchmark measures, so the honest
// assertion is used rather than a stronger one the fixture cannot support.
public sealed partial class MinimumWindowSubstringBenchmarksTests
{
    private const int SmallestLength = 200;

    // LeetCode 76's answer when no window covers every required character.
    private const string NoWindowAnswer = "";

    [Fact]
    public void Setup_SameLength_RebuildsTheSameText() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_TargetAbsentFromText_ReportsNoWindowAndAgreesWithSlidingWindowHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(NoWindowAnswer, harness.BruteForce());
        Assert.Equal(
            AnswerText.Of(harness.SlidingWindowHashMap()),
            AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void SlidingWindowHashMap_TargetAbsentFromText_ReportsNoWindowAndAgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(NoWindowAnswer, harness.SlidingWindowHashMap());
        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.SlidingWindowHashMap()));
    }

    private static MinimumWindowSubstringBenchmarks BuildHarness()
    {
        var harness = new MinimumWindowSubstringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
