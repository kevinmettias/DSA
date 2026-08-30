using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Most Stones Removed with Same Row or Column (LC 947): both strategies use this
// repo's own DisjointSet, but differ in how they discover which stones to union.
// PairwiseScan checks every O(n^2) stone pair for a shared row/column before
// unioning by stone index - the naive way to find the edges of the "shares a
// row/column" graph. RowColumnKeyedUnion skips discovering those edges altogether by
// unioning each stone directly to its row id and column id (offset into one shared
// DisjointSet universe, per the test's own doc comment) - O(n) unions instead of
// O(n^2) comparisons, since two stones end up connected exactly when they route
// through the same row or column id regardless of ever being compared directly.
[MemoryDiagnoser]
public class MostStonesRemovedWithSameRowOrColumnBenchmarks
{
    [Params(200, 2_000)]
    public int StoneCount;

    private int[][] _stones = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(947);
        var range = Math.Max(1, StoneCount / 4);
        var seen = new HashSet<(int Row, int Col)>();
        var stones = new List<int[]>();

        while (stones.Count < StoneCount)
        {
            var row = random.Next(range);
            var col = random.Next(range);

            if (seen.Add((row, col)))
            {
                stones.Add([row, col]);
            }
        }

        _stones = stones.ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScanThenUnion()
    {
        var n = _stones.Length;
        var components = new DisjointSet(n);

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                if (_stones[i][0] == _stones[j][0] || _stones[i][1] == _stones[j][1])
                {
                    components.Union(i, j);
                }
            }
        }

        var roots = new HashSet<int>();
        for (var i = 0; i < n; i++)
        {
            roots.Add(components.Find(i));
        }

        return n - roots.Count;
    }

    [Benchmark]
    public int RowColumnKeyedUnion()
    {
        var colOffset = _stones.Max(stone => stone[0]) + 1;
        var universeSize = colOffset + _stones.Max(stone => stone[1]) + 1;
        var components = new DisjointSet(universeSize);

        foreach (var stone in _stones)
        {
            components.Union(stone[0], colOffset + stone[1]);
        }

        var roots = new HashSet<int>();
        foreach (var stone in _stones)
        {
            roots.Add(components.Find(stone[0]));
        }

        return _stones.Length - roots.Count;
    }
}
