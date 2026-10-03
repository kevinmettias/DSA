using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for StoneGameWorkloads (ARCHITECTURE 17.7). The reading depends on the piles being
// a game LC 877 could pose: an even number of piles, each of 1 to 500 stones, and - the guarantee the
// extra stone exists for - an odd total, so the game cannot tie. Both of StoneGameBenchmarks' pile
// counts are checked at its own seed, since whether the extra stone is needed depends on the draw.
public sealed partial class StoneGameWorkloadsTests
{
    // Mirrors StoneGameBenchmarks' own private RandomSeed and its two pile counts.
    private const int Seed = 877;
    private const int SmallestPileCount = 22;
    private const int LargestPileCount = 26;

    private const int MinPile = 1;
    private const int MaxPile = 500;
    private const int TotalParityDivisor = 2;

    public static TheoryData<int> PileCounts => new([SmallestPileCount, LargestPileCount]);

    [Theory]
    [MemberData(nameof(PileCounts))]
    public void BuildPiles_BenchmarkPileCounts_TotalAnOddNumberOfStones(int pileCount) =>
        Assert.NotEqual(0, StoneGameWorkloads.BuildPiles(pileCount, Seed).Sum() % TotalParityDivisor);

    [Theory]
    [MemberData(nameof(PileCounts))]
    public void BuildPiles_BenchmarkPileCounts_AreThatManyPilesInsideThePileRange(int pileCount)
    {
        var piles = StoneGameWorkloads.BuildPiles(pileCount, Seed);

        Assert.Equal(pileCount, piles.Length);
        Assert.All(piles, pile => Assert.InRange(pile, MinPile, MaxPile));
    }

    [Fact]
    public void BuildPiles_SameSeed_ReturnsTheSamePiles() =>
        Assert.Equal(
            StoneGameWorkloads.BuildPiles(SmallestPileCount, Seed),
            StoneGameWorkloads.BuildPiles(SmallestPileCount, Seed));
}
