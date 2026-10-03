using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ValidPalindromeWorkloads (ARCHITECTURE 17.7). LC 125's benchmark pins both
// arms to true and relies on the noise being real, so this checks what the fixture's comment
// promises: the string is printable ASCII of the asked length, it carries both punctuation and mixed
// case, and it reads the same backwards once only its lowercased letters and digits are kept.
public sealed partial class ValidPalindromeWorkloadsTests
{
    private const int Length = 2_000;
    private const int Seed = 125; // LC problem number
    private const char FirstPrintable = ' ';
    private const char LastPrintable = '~';

    [Fact]
    public void BuildNoisyPalindrome_Length_ReturnsThatManyPrintableCharacters()
    {
        var text = Build();

        Assert.Equal(Length, text.Length);
        Assert.All(text, character => Assert.InRange(character, FirstPrintable, LastPrintable));
    }

    [Fact]
    public void BuildNoisyPalindrome_LettersAndDigits_ReadTheSameBackwards()
    {
        var kept = Build().Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray();

        Assert.Equal(kept, kept.Reverse());
    }

    [Fact]
    public void BuildNoisyPalindrome_Text_CarriesPunctuationAndBothCases()
    {
        var text = Build();

        Assert.Contains(text, character => !char.IsLetterOrDigit(character));
        Assert.Contains(text, char.IsUpper);
        Assert.Contains(text, char.IsLower);
    }

    [Fact]
    public void BuildNoisyPalindrome_SameSeed_ReturnsTheSameText() =>
        Assert.Equal(Build(), Build());

    private static string Build() => ValidPalindromeWorkloads.BuildNoisyPalindrome(Length, new Random(Seed));
}
