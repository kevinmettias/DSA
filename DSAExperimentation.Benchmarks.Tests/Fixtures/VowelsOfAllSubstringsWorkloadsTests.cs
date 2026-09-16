using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for VowelsOfAllSubstringsWorkloads (ARCHITECTURE 17.7): LC 2063 counts each vowel's
// contribution across all substrings, so the word has to be long enough for that count to be worth
// measuring and must not degenerate into an all-vowel or vowel-free word. Letters are drawn uniformly over
// the whole lowercase alphabet, which is what the two presence assertions below witness.
public sealed partial class VowelsOfAllSubstringsWorkloadsTests
{
    private const int Length = 64;
    private const int Seed = 2063; // LC problem number
    private const int FewestVowels = 1;
    private const char FirstLowercaseLetter = 'a';
    private const char LastLowercaseLetter = 'z';
    private const string Vowels = "aeiou";

    [Fact]
    public void BuildRandomLowercaseWord_Length_ReturnsOneLetterPerPosition()
    {
        var word = VowelsOfAllSubstringsWorkloads.BuildRandomLowercaseWord(Length, Seed);

        Assert.Equal(Length, word.Length);
    }

    [Fact]
    public void BuildRandomLowercaseWord_EveryLetter_IsALowercaseLetter() =>
        Assert.All(
            VowelsOfAllSubstringsWorkloads.BuildRandomLowercaseWord(Length, Seed),
            letter => Assert.InRange(letter, FirstLowercaseLetter, LastLowercaseLetter));

    // Neither strategy should meet a degenerate word: a uniform draw over the alphabet leaves both vowels
    // and non-vowels present, so the contribution count has both zero and nonzero terms to add.
    [Fact]
    public void BuildRandomLowercaseWord_SeededWord_HoldsBothVowelsAndNonVowels()
    {
        var word = VowelsOfAllSubstringsWorkloads.BuildRandomLowercaseWord(Length, Seed);

        Assert.True(word.Count(Vowels.Contains) >= FewestVowels);
        Assert.True(word.Count(letter => !Vowels.Contains(letter)) >= FewestVowels);
    }

    [Fact]
    public void BuildRandomLowercaseWord_SameSeed_ReturnsTheSameWord() =>
        Assert.Equal(
            VowelsOfAllSubstringsWorkloads.BuildRandomLowercaseWord(Length, Seed),
            VowelsOfAllSubstringsWorkloads.BuildRandomLowercaseWord(Length, Seed));
}
