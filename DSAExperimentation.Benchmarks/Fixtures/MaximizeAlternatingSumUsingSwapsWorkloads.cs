namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing for LC 3695: half as many random swap draws as there are indices, so
// the indices fall into a handful of nontrivial connected components rather than
// singletons. LC 3695 promises 0 <= p < q and no repeated pair, so a draw of one index
// twice is dropped, every pair is written low index first, and a pair drawn again is
// kept once.
//
// A Random is taken rather than a seed because the harness draws the array's values and
// then the swaps from one stream.
internal static class MaximizeAlternatingSumUsingSwapsWorkloads
{
    private const int IndicesPerSwapDraw = 2;

    public static int[][] BuildSwaps(int elementCount, Random random) =>
        [.. Enumerable.Range(0, elementCount / IndicesPerSwapDraw)
            .Select(_ => (First: random.Next(elementCount), Second: random.Next(elementCount)))
            .Where(draw => draw.First != draw.Second)
            .Select(draw => (Low: Math.Min(draw.First, draw.Second), High: Math.Max(draw.First, draw.Second)))
            .Distinct()
            .Select(pair => new[] { pair.Low, pair.High })];
}
