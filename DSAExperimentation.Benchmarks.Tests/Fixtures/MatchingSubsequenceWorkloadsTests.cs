using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MatchingSubsequenceWorkloads (ARCHITECTURE 17.7). The reading depends on LC
// 792's words being unmatchable by construction - the alphabet is narrowed to 25 letters and every
// word ends on the excluded 26th - so the per-word two-pointer arm is forced through its full
// O(words * s.Length) worst case instead of short-circuiting on an early match.
public sealed partial class MatchingSubsequenceWorkloadsTests
{
    private const int TextLength = 256;
    private const int WordLength = 4;
    private const int WordCount = 64;
    private const int Seed = 792; // LC problem number
    private const int AlphabetSize = 25;
    private const char FirstLetter = 'a';
    private const char UnreachableLetter = 'z';

    [Fact]
    public void BuildText_Length_ReturnsAStringOfExactlyThatLength() =>
        Assert.Equal(TextLength, MatchingSubsequenceWorkloads.BuildText(new Random(Seed), TextLength).Length);

    // The text never draws the letter the words append, which is what keeps every word unmatchable
    // however the two draws happen to fall.
    [Fact]
    public void BuildText_EveryCharacter_StaysBelowTheExcludedLetter()
    {
        var text = MatchingSubsequenceWorkloads.BuildText(new Random(Seed), TextLength);
        var highestReachableLetter = (char)(FirstLetter + AlphabetSize - 1);

        Assert.All(text, character => Assert.InRange(character, FirstLetter, highestReachableLetter));
        Assert.DoesNotContain(UnreachableLetter, text);
    }

    [Fact]
    public void BuildText_TwoRandomsFromTheSameSeed_ReturnTheSameText() =>
        Assert.Equal(
            MatchingSubsequenceWorkloads.BuildText(new Random(Seed), TextLength),
            MatchingSubsequenceWorkloads.BuildText(new Random(Seed), TextLength));

    [Fact]
    public void BuildUnmatchableWords_WordCount_ReturnsOneWordPerRequestedPosition() =>
        Assert.Equal(
            WordCount,
            MatchingSubsequenceWorkloads.BuildUnmatchableWords(new Random(Seed), WordCount).Length);

    [Fact]
    public void BuildUnmatchableWords_EveryWord_IsAlphabetTextFollowedByTheExcludedLetter()
    {
        var words = MatchingSubsequenceWorkloads.BuildUnmatchableWords(new Random(Seed), WordCount);
        var highestReachableLetter = (char)(FirstLetter + AlphabetSize - 1);

        Assert.All(words, word => Assert.Equal(WordLength + 1, word.Length));
        Assert.All(words, word => Assert.Equal(UnreachableLetter, word[^1]));
        Assert.All(
            words,
            word => Assert.All(
                word[..WordLength],
                character => Assert.InRange(character, FirstLetter, highestReachableLetter)));
    }

    [Fact]
    public void BuildUnmatchableWords_TwoRandomsFromTheSameSeed_ReturnTheSameWords() =>
        Assert.Equal(
            MatchingSubsequenceWorkloads.BuildUnmatchableWords(new Random(Seed), WordCount),
            MatchingSubsequenceWorkloads.BuildUnmatchableWords(new Random(Seed), WordCount));
}
