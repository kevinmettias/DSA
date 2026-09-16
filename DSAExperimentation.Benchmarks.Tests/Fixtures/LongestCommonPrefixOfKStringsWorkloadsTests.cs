using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LongestCommonPrefixOfKStringsWorkloads (ARCHITECTURE 17.7). The reading depends
// on LC 3485's words being drawn over a deliberately small alphabet, so candidate pairs share long
// prefixes instead of diverging on their first character and leaving nothing for either strategy to
// walk.
public sealed partial class LongestCommonPrefixOfKStringsWorkloadsTests
{
    private const int Count = 64;
    private const int MaxWordLength = 12;
    private const int Seed = 3485; // LC problem number
    private const char FirstAlphabetLetter = 'a';
    private const char LastAlphabetLetter = 'c';
    private const int MinWordLength = 1;

    [Fact]
    public void BuildWords_Count_ReturnsOneWordPerRequestedPosition() =>
        Assert.Equal(
            Count,
            LongestCommonPrefixOfKStringsWorkloads.BuildWords(Count, MaxWordLength, Seed).Length);

    [Fact]
    public void BuildWords_EveryWord_IsANonEmptyRunOverTheSmallAlphabet()
    {
        var words = LongestCommonPrefixOfKStringsWorkloads.BuildWords(Count, MaxWordLength, Seed);

        Assert.All(words, word => Assert.InRange(word.Length, MinWordLength, MaxWordLength));
        Assert.All(
            words,
            word => Assert.All(
                word,
                character => Assert.InRange(character, FirstAlphabetLetter, LastAlphabetLetter)));
    }

    [Fact]
    public void BuildWords_SameSeed_ReturnsTheSameWords() =>
        Assert.Equal(
            LongestCommonPrefixOfKStringsWorkloads.BuildWords(Count, MaxWordLength, Seed),
            LongestCommonPrefixOfKStringsWorkloads.BuildWords(Count, MaxWordLength, Seed));
}
