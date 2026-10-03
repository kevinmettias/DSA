namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 877 - pileCount piles of 1 to 99 stones, with one more
// stone on the last pile whenever the drawn total comes out even. LC 877 promises an odd
// total, so the game cannot tie; the extra stone keeps every pile inside its [1, 500] and
// moves no draw.
internal static class StoneGameWorkloads
{
    private const int PileValueUpperBoundExclusive = 100;
    private const int TotalParityDivisor = 2;

    public static int[] BuildPiles(int pileCount, int seed)
    {
        var piles = SeededDraws.Values(pileCount, 1, PileValueUpperBoundExclusive, new Random(seed));
        var isTotalEven = piles.Sum() % TotalParityDivisor == 0;

        if (isTotalEven)
        {
            piles[^1]++;
        }

        return piles;
    }
}
