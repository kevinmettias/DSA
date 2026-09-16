using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestPalindromicRearrangementIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - build the half and mirror it through a
// char array against doing the same through this repo's own Stack<char> - so a harness whose
// arms disagree is rearranging two different texts. Setup builds the text from one fixed seed
// and mirrors each half back onto itself, so the same Length must rebuild the same palindrome;
// otherwise two published numbers were never comparable.
//
// Beyond agreeing with each other, both arms answer a question with a property checkable from
// the answer alone: a rearrangement of the text is the same length as the text and is itself a
// palindrome, so a rearrangement that came back as a sorted half, or truncated, fails below
// even when the two arms fail identically.
public sealed partial class SmallestPalindromicRearrangementIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePalindrome() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CharArrayReverse()),
            AnswerText.Of(BuildHarness().CharArrayReverse()));

    [Fact]
    public void CharArrayReverse_TwoHundredCharacterPalindrome_AgreesWithCharStack()
    {
        var harness = BuildHarness();
        var answer = harness.CharArrayReverse();

        Assert.Equal(AnswerText.Of(harness.CharStack()), AnswerText.Of(answer));
        Assert.Equal(SmallestLength, answer.Length);
        Assert.Equal(answer, Reversed(answer));
    }

    [Fact]
    public void CharStack_TwoHundredCharacterPalindrome_AgreesWithCharArrayReverse()
    {
        var harness = BuildHarness();
        var answer = harness.CharStack();

        Assert.Equal(AnswerText.Of(harness.CharArrayReverse()), AnswerText.Of(answer));
        Assert.Equal(SmallestLength, answer.Length);
        Assert.Equal(answer, Reversed(answer));
    }

    private static string Reversed(string text) => new([.. text.Reverse()]);

    private static SmallestPalindromicRearrangementIBenchmarks BuildHarness()
    {
        var harness = new SmallestPalindromicRearrangementIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
