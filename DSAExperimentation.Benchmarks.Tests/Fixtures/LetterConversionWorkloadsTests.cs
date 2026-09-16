using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LetterConversionWorkloads (ARCHITECTURE 17.7). The reading depends on the
// conversion ring letting every character reach every other one - so both LC 2976 strategies stay on
// their full-cost path instead of short-circuiting on the first unreachable character - and on the
// two strings being independent draws of the requested length.
public sealed partial class LetterConversionWorkloadsTests
{
    private const int AlphabetSize = 26;
    private const int Seed = 2976; // LC problem number
    private const int Length = 256;
    private const int MinEdgeCost = 1;
    private const int MaxEdgeCostExclusive = 1000;
    private const char FirstLetter = 'a';

    [Fact]
    public void BuildRing_AlphabetSize_ReturnsOneLetterAndOneCostPerRingPosition()
    {
        var (original, changed, cost) = LetterConversionWorkloads.BuildRing(Seed);

        Assert.Equal(AlphabetSize, original.Length);
        Assert.Equal(AlphabetSize, changed.Length);
        Assert.Equal(AlphabetSize, cost.Length);
    }

    // a -> b -> ... -> z -> a is the ring, which is what makes every character reachable from every
    // other one; the wrap at the last position is the part of it that is easy to lose.
    [Fact]
    public void BuildRing_OriginalAndChanged_LayOutTheWrappingSuccessorRing()
    {
        var (original, changed, _) = LetterConversionWorkloads.BuildRing(Seed);

        foreach (var position in Enumerable.Range(0, AlphabetSize))
        {
            Assert.Equal((char)(FirstLetter + position), original[position]);
            Assert.Equal(original[(position + 1) % AlphabetSize], changed[position]);
        }
    }

    [Fact]
    public void BuildRing_EveryLetter_AppearsExactlyOnceOnEachSide()
    {
        var (original, changed, _) = LetterConversionWorkloads.BuildRing(Seed);

        Assert.Equal(AlphabetSize, original.Distinct().Count());
        Assert.Equal(AlphabetSize, changed.Distinct().Count());
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

        Assert.All(source, character => Assert.InRange(character, FirstLetter, (char)(FirstLetter + AlphabetSize - 1)));
        Assert.All(target, character => Assert.InRange(character, FirstLetter, (char)(FirstLetter + AlphabetSize - 1)));
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
