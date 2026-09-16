using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MinimumOperationsToEqualizeBinaryStringWorkloads (ARCHITECTURE 17.7). The reading
// depends on LC 3666's K sitting near half the string's length, so a single operation's reachable
// zero-count range is wide and both strategies explore a nontrivial slice of the 0..n state graph.
public sealed partial class MinimumOperationsToEqualizeBinaryStringWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 3666; // LC problem number
    private const int LengthBelowTheDivisor = 1;
    private const int KDivisor = 2;
    private const int MinimumK = 1;
    private const char Zero = '0';
    private const char One = '1';

    [Fact]
    public void Build_Length_ReturnsAStringOfThatLengthWithHalfOfItAsK()
    {
        var (text, k) = MinimumOperationsToEqualizeBinaryStringWorkloads.Build(Length, Seed);

        Assert.Equal(Length, text.Length);
        Assert.Equal(Length / KDivisor, k);
    }

    [Fact]
    public void Build_LengthBelowTheDivisor_ClampsKToTheMinimum()
    {
        var (_, k) = MinimumOperationsToEqualizeBinaryStringWorkloads.Build(LengthBelowTheDivisor, Seed);

        Assert.Equal(MinimumK, k);
    }

    [Fact]
    public void Build_EveryCharacter_IsABinaryDigit()
    {
        var (text, _) = MinimumOperationsToEqualizeBinaryStringWorkloads.Build(Length, Seed);

        Assert.All(text, character => Assert.Contains(character, new[] { Zero, One }));
    }

    [Fact]
    public void Build_Text_ContainsBothDigits()
    {
        var (text, _) = MinimumOperationsToEqualizeBinaryStringWorkloads.Build(Length, Seed);

        Assert.Contains(Zero, text);
        Assert.Contains(One, text);
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameTextAndK()
    {
        var (text, k) = MinimumOperationsToEqualizeBinaryStringWorkloads.Build(Length, Seed);
        var (repeatText, repeatK) = MinimumOperationsToEqualizeBinaryStringWorkloads.Build(Length, Seed);

        Assert.Equal(text, repeatText);
        Assert.Equal(k, repeatK);
    }
}
