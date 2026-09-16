using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LongestSubstringWithAtLeastKRepeatingCharactersWorkloads (ARCHITECTURE 17.7).
// The reading depends on LC 395's string being drawn from a four-letter alphabet, so most characters
// clear K's threshold and the divide-and-conquer strategy actually gets to recurse.
public sealed partial class LongestSubstringWithAtLeastKRepeatingCharactersWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 395; // LC problem number
    private const int AlphabetSize = 4;
    private const char FirstLetter = 'a';

    [Fact]
    public void BuildString_Length_ReturnsAStringOfExactlyThatLength() =>
        Assert.Equal(
            Length,
            LongestSubstringWithAtLeastKRepeatingCharactersWorkloads.BuildString(Length, Seed).Length);

    [Fact]
    public void BuildString_EveryCharacter_StaysOnTheFourLetterAlphabet()
    {
        var text = LongestSubstringWithAtLeastKRepeatingCharactersWorkloads.BuildString(Length, Seed);

        Assert.All(text, character => Assert.InRange(character, FirstLetter, (char)(FirstLetter + AlphabetSize - 1)));
    }

    [Fact]
    public void BuildString_TheWholeAlphabet_Appears()
    {
        var text = LongestSubstringWithAtLeastKRepeatingCharactersWorkloads.BuildString(Length, Seed);

        Assert.Equal(AlphabetSize, text.Distinct().Count());
    }

    [Fact]
    public void BuildString_SameSeed_ReturnsTheSameString() =>
        Assert.Equal(
            LongestSubstringWithAtLeastKRepeatingCharactersWorkloads.BuildString(Length, Seed),
            LongestSubstringWithAtLeastKRepeatingCharactersWorkloads.BuildString(Length, Seed));
}
