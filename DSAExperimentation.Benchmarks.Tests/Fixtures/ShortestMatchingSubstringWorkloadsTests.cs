using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ShortestMatchingSubstringWorkloads (ARCHITECTURE 17.7). The LC 3455 reading
// depends on the alphabet being small enough that the fixed pattern's literal parts each turn up
// many times, so the search has real candidates to compare instead of falling through to -1 on
// every run - which is why the alphabet has to cover every letter those parts use.
public sealed partial class ShortestMatchingSubstringWorkloadsTests
{
    private const int TextLength = 500;
    private const int Seed = 3455; // LC problem number
    private const char FirstAlphabetLetter = 'a';
    private const char LastAlphabetLetter = 'f';
    private const string FirstLiteralPart = "ab";
    private const string SecondLiteralPart = "cd";
    private const string ThirdLiteralPart = "ef";

    [Fact]
    public void BuildText_Length_ReturnsOneCharacterPerPosition() =>
        Assert.Equal(
            TextLength,
            ShortestMatchingSubstringWorkloads.BuildText(TextLength, Seed).Length);

    [Fact]
    public void BuildText_EveryCharacter_ComesFromTheSixLetterAlphabet() =>
        Assert.All(
            ShortestMatchingSubstringWorkloads.BuildText(TextLength, Seed),
            character => Assert.InRange(character, FirstAlphabetLetter, LastAlphabetLetter));

    // Both arms search for the same fixed pattern, so a text its literal parts never occur in would
    // measure only the -1 fast path. This is the claim the alphabet's size exists to make true, and
    // it fails loudly if the alphabet ever shrinks back to four letters, where "ef" could not occur.
    [Fact]
    public void BuildText_Text_ContainsEveryLiteralPartOfTheFixedPattern()
    {
        var text = ShortestMatchingSubstringWorkloads.BuildText(TextLength, Seed);

        Assert.Contains(FirstLiteralPart, text, StringComparison.Ordinal);
        Assert.Contains(SecondLiteralPart, text, StringComparison.Ordinal);
        Assert.Contains(ThirdLiteralPart, text, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildText_SameSeed_ReturnsTheSameText() =>
        Assert.Equal(
            ShortestMatchingSubstringWorkloads.BuildText(TextLength, Seed),
            ShortestMatchingSubstringWorkloads.BuildText(TextLength, Seed));
}
