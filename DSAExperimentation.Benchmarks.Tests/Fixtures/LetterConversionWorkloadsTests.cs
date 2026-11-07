using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LetterConversionWorkloads (ARCHITECTURE 17.7). The reading depends on the
// conversion ring letting every character reach every other one - so both LC 2976 strategies stay on
// their full-cost path instead of short-circuiting on the first unreachable character - and on the
// two strings being independent draws of the requested length.
public sealed partial class LetterConversionWorkloadsTests
{
    private const int Seed = 2976; // LC problem number
    private const int Length = 256;
    private const int MinEdgeCost = 1;
    private const int MaxEdgeCostExclusive = 1000;
    private const char FirstLetter = 'a';

    [Fact]
    public void BuildRing_AlphabetSize_ReturnsOneLetterAndOneCostPerRingPosition()
    {
        var (original, changed, cost) = LetterConversionWorkloads.BuildRing(Seed);

        Assert.Equal(EnglishLetters.Size, original.Length);
        Assert.Equal(EnglishLetters.Size, changed.Length);
        Assert.Equal(EnglishLetters.Size, cost.Length);
    }

    // a -> b -> ... -> z -> a is the ring, which is what makes every character reachable from every
    // other one; the wrap at the last position is the part of it that is easy to lose.
    [Fact]
    public void BuildRing_OriginalAndChanged_LayOutTheWrappingSuccessorRing()
    {
        var (original, changed, _) = LetterConversionWorkloads.BuildRing(Seed);

        foreach (var position in Enumerable.Range(0, EnglishLetters.Size))
        {
            Assert.Equal((char)(FirstLetter + position), original[position]);
            Assert.Equal(original[(position + 1) % EnglishLetters.Size], changed[position]);
        }
    }

    [Fact]
    public void BuildRing_EveryLetter_AppearsExactlyOnceOnEachSide()
    {
        var (original, changed, _) = LetterConversionWorkloads.BuildRing(Seed);

        Assert.Equal(EnglishLetters.Size, original.Distinct().Count());
        Assert.Equal(EnglishLetters.Size, changed.Distinct().Count());
    }

    [Fact]
    public void BuildRing_EveryCost_StaysWithinTheEdgeCostBand() =>
        Assert.All(
            LetterConversionWorkloads.BuildRing(Seed).Cost,
            amount => Assert.InRange(amount, MinEdgeCost, MaxEdgeCostExclusive - 1));

    [Fact]
    public void BuildRing_SameSeed_ReturnsTheSameRing()
    {
        var (original, changed, cost) = LetterConversionWorkloads.BuildRing(Seed);
        var (repeatOriginal, repeatChanged, repeatCost) = LetterConversionWorkloads.BuildRing(Seed);

        Assert.Equal(original, repeatOriginal);
        Assert.Equal(changed, repeatChanged);
        Assert.Equal(cost, repeatCost);
    }

    [Fact]
    public void BuildStrings_Length_ReturnsTwoOneCharacterPerPositionStrings()
    {
        var (source, target) = LetterConversionWorkloads.BuildStrings(Length, Seed);

        Assert.Equal(Length, source.Length);
        Assert.Equal(Length, target.Length);
    }

    [Fact]
    public void BuildStrings_EveryCharacter_StaysOnTheConversionAlphabet()
    {
        var (source, target) = LetterConversionWorkloads.BuildStrings(Length, Seed);

        Assert.All(source, character => Assert.InRange(character, FirstLetter, (char)(FirstLetter + EnglishLetters.Size - 1)));
        Assert.All(target, character => Assert.InRange(character, FirstLetter, (char)(FirstLetter + EnglishLetters.Size - 1)));
    }

    [Fact]
    public void BuildStrings_SameSeed_ReturnsTheSamePair()
    {
        var (source, target) = LetterConversionWorkloads.BuildStrings(Length, Seed);
        var (repeatSource, repeatTarget) = LetterConversionWorkloads.BuildStrings(Length, Seed);

        Assert.Equal(source, repeatSource);
        Assert.Equal(target, repeatTarget);
    }
}
