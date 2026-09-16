using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestPalindromicRearrangementIIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - walk the arrangements until the
// requested rank against count the arrangements combinatorially and build that rank directly -
// so a harness whose arms disagree is ranking two different arrangement orders. Setup builds
// the palindrome from one fixed seed over a four-letter half alphabet, so the same Length must
// rebuild the same text; otherwise two published numbers were never comparable.
//
// The half's arrangement count stays well above the harness's own rank at both tunings, so an
// answer always exists and neither arm may report the problem's "no such arrangement" sentinel.
// Beyond agreeing with each other, both arms answer a question with a property checkable from
// the answer alone: the rearrangement is as long as the text it came from and is a palindrome.
public sealed partial class SmallestPalindromicRearrangementIIBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePalindrome() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CountingGreedy()),
            AnswerText.Of(BuildHarness().CountingGreedy()));

    [Fact]
    public void BacktrackingRank_TwentyCharacterPalindrome_AgreesWithCountingGreedy()
    {
        var harness = BuildHarness();
        var answer = harness.BacktrackingRank();

        Assert.Equal(AnswerText.Of(harness.CountingGreedy()), AnswerText.Of(answer));
        Assert.Equal(SmallestLength, answer.Length);
        Assert.Equal(answer, Reversed(answer));
    }

    [Fact]
    public void CountingGreedy_TwentyCharacterPalindrome_AgreesWithBacktrackingRank()
    {
        var harness = BuildHarness();
        var answer = harness.CountingGreedy();

        Assert.Equal(AnswerText.Of(harness.BacktrackingRank()), AnswerText.Of(answer));
        Assert.Equal(SmallestLength, answer.Length);
        Assert.Equal(answer, Reversed(answer));
    }

    private static string Reversed(string text) => new([.. text.Reverse()]);

    private static SmallestPalindromicRearrangementIIBenchmarks BuildHarness()
    {
        var harness = new SmallestPalindromicRearrangementIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
