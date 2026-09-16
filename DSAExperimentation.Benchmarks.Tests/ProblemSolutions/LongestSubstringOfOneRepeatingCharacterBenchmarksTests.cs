using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestSubstringOfOneRepeatingCharacterBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - rescanning the whole text after every
// point update against this repo's run segment tree - so a harness whose arms disagree is timing
// two different problems. Both arms return one answer per query, and the query each answer
// belongs to is part of it, so AnswerText.Of is the comparison. Setup generates the base text and
// the query stream from one fixed seed, so the same Length must rebuild both streams.
public sealed partial class LongestSubstringOfOneRepeatingCharacterBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().SegmentTreeRunAggregate()),
            AnswerText.Of(BuildHarness().SegmentTreeRunAggregate()));

    [Fact]
    public void LinearRescanAfterEachUpdate_SmallestLength_AgreesWithSegmentTreeRunAggregate()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SegmentTreeRunAggregate()),
            AnswerText.Of(harness.LinearRescanAfterEachUpdate()));
    }

    [Fact]
    public void SegmentTreeRunAggregate_SmallestLength_AgreesWithLinearRescanAfterEachUpdate()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LinearRescanAfterEachUpdate()),
            AnswerText.Of(harness.SegmentTreeRunAggregate()));
    }

    private static LongestSubstringOfOneRepeatingCharacterBenchmarks BuildHarness()
    {
        var harness = new LongestSubstringOfOneRepeatingCharacterBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
