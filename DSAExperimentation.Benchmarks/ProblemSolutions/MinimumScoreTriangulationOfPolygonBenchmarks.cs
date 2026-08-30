using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Score Triangulation of Polygon (LC 1039): plain un-memoized interval
// recursion over (left, right) vertex-index pairs - exponential, since the same
// (left, right) sub-polygon recurs across many different choices of apex outside
// it - vs. this repo's own Memoizer<TState,TResult> caching that exact pair.
// VertexCount is kept modest (<=14) for the same reason BurstBalloonsBenchmarks'
// UnmemoizedRecursion is: the un-memoized baseline's blowup is real.
[MemoryDiagnoser]
public class MinimumScoreTriangulationOfPolygonBenchmarks
{
    [Params(10, 14)]
    public int VertexCount;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, VertexCount).Select(_ => random.Next(1, 100)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => ScoreBetween(0, _values.Length - 1);

    private int ScoreBetween(int left, int right)
    {
        if (right - left < 2)
        {
            return 0;
        }

        var best = int.MaxValue;

        for (var apex = left + 1; apex < right; apex++)
        {
            var triangleScore = (_values[left] * _values[apex] * _values[right])
                + ScoreBetween(left, apex) + ScoreBetween(apex, right);
            best = Math.Min(best, triangleScore);
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion()
    {
        return Memoizer.Memoize<(int Left, int Right), int>((0, _values.Length - 1), ScoreBetweenMemoized);

        int ScoreBetweenMemoized((int Left, int Right) range, Func<(int Left, int Right), int> score)
        {
            var (left, right) = range;
            if (right - left < 2)
            {
                return 0;
            }

            var best = int.MaxValue;

            for (var apex = left + 1; apex < right; apex++)
            {
                var triangleScore = (_values[left] * _values[apex] * _values[right])
                    + score((left, apex)) + score((apex, right));
                best = Math.Min(best, triangleScore);
            }

            return best;
        }
    }
}
