using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IntegerToEnglishWordsBenchmarks (ARCHITECTURE 17.9). Both arms are competing
// strategies for the same question - repeated string prepending against un-reversing the group
// order through this repo's own Stack<string> - so a harness whose arms disagree is timing two
// different problems. Both arms answer with the spelled number, a string, compared through
// AnswerText so that the two renderings of an equal string are equal by value. This class has no
// [GlobalSetup]: Number is the whole input, so each [Fact] constructs the harness with the smaller
// of the two [Params] values. That value is also the decisive part of the assertion - the two
// [Params] values are a bare single-group number and Int32.MaxValue - so the smallest number's
// spelling is pinned here rather than only compared arm to arm.
public sealed partial class IntegerToEnglishWordsBenchmarksTests
{
    private const int SmallestNumber = 123;

    // One hundred and twenty-three in LC 273's own words.
    private const string ExpectedSpellingForSmallestNumber = "One Hundred Twenty Three";

    [Fact]
    public void Setup_SameNumber_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().StringPrepend(), BuildHarness().StringPrepend());

    [Fact]
    public void StringPrepend_SpelledNumber_AgreesWithWordStack()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.WordStack()), AnswerText.Of(harness.StringPrepend()));
        Assert.Equal(ExpectedSpellingForSmallestNumber, harness.StringPrepend());
    }

    [Fact]
    public void WordStack_SpelledNumber_AgreesWithStringPrepend()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.StringPrepend()), AnswerText.Of(harness.WordStack()));
        Assert.Equal(ExpectedSpellingForSmallestNumber, harness.WordStack());
    }

    private static IntegerToEnglishWordsBenchmarks BuildHarness() =>
        new() { Number = SmallestNumber };
}
