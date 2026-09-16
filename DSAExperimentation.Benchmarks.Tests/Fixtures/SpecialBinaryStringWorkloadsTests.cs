using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SpecialBinaryStringWorkloads (ARCHITECTURE 17.7). LC 761's recursion is only
// well-founded on genuinely special strings, so the reading depends on the generated input being
// one: pairCount 1/0 pairs, every prefix never closing more than it opened, and the recursion's own
// closure under wrapping and concatenation is what makes that hold for every draw.
public sealed partial class SpecialBinaryStringWorkloadsTests
{
    private const int PairCount = 50;
    private const int Seed = 761; // LC problem number
    private const int OneBitCountPerPair = 1;
    private const int CharsPerPair = 2;
    private const char OneBit = '1';
    private const char ZeroBit = '0';
    private const int EmptyDepth = 0;

    [Fact]
    public void GenerateSpecial_PairCount_ReturnsTwoCharactersPerPair() =>
        Assert.Equal(
            PairCount * CharsPerPair,
            SpecialBinaryStringWorkloads.GenerateSpecial(PairCount, new Random(Seed)).Length);

    [Fact]
    public void GenerateSpecial_PairCount_ReturnsExactlyOneOneBitPerPair() =>
        Assert.Equal(
            PairCount * OneBitCountPerPair,
            SpecialBinaryStringWorkloads.GenerateSpecial(PairCount, new Random(Seed)).Count(bit => bit == OneBit));

    [Fact]
    public void GenerateSpecial_EveryPrefix_NeverClosesMoreThanItOpened()
    {
        var depth = 0;

        foreach (var bit in SpecialBinaryStringWorkloads.GenerateSpecial(PairCount, new Random(Seed)))
        {
            depth += bit == OneBit ? 1 : -1;
            Assert.True(depth >= EmptyDepth, "A special binary string never closes before it opens.");
        }

        Assert.Equal(EmptyDepth, depth);
    }

    [Fact]
    public void GenerateSpecial_String_StartsWithAOneBitAndEndsWithAZeroBit()
    {
        var special = SpecialBinaryStringWorkloads.GenerateSpecial(PairCount, new Random(Seed));

        Assert.Equal(OneBit, special[0]);
        Assert.Equal(ZeroBit, special[^1]);
    }

    [Fact]
    public void GenerateSpecial_SameSeed_ReturnsTheSameString() =>
        Assert.Equal(
            SpecialBinaryStringWorkloads.GenerateSpecial(PairCount, new Random(Seed)),
            SpecialBinaryStringWorkloads.GenerateSpecial(PairCount, new Random(Seed)));
}
