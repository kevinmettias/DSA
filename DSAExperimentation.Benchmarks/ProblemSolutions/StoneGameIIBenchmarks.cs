using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stone Game II (LC 1140): plain un-memoized minimax recursion over
// (index, M) - exponential, since the same (index, M) state recurs through
// many different pick-sequences reaching it - vs. this repo's own
// Memoizer<TState,TResult> caching that exact pair, the identical shape
// StoneGameBenchmarks/MinimumScoreTriangulationOfPolygonBenchmarks already
// use. Both benchmarks share one precomputed suffix-sum array from Setup, so
// the only thing that differs between them is memoization itself, not
// whether the sum is recomputed. PileCount is kept modest for the same
// reason those other interval/window-DP benchmarks' input sizes are: the
// un-memoized baseline's blowup is real (each state can branch into up to
// 2*M sub-states).
[MemoryDiagnoser]
public class StoneGameIIBenchmarks
{
    [Params(10, 14)]
    public int PileCount;

    private int[] _suffixSum = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1140);
        var piles = Enumerable.Range(0, PileCount).Select(_ => random.Next(1, 100)).ToArray();

        _suffixSum = new int[PileCount + 1];
        for (var i = PileCount - 1; i >= 0; i--)
        {
            _suffixSum[i] = _suffixSum[i + 1] + piles[i];
        }
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Best(0, 1);

    private int Best(int index, int m)
    {
        if (index + (2 * m) >= PileCount)
        {
            return _suffixSum[index];
        }

        var result = 0;
        for (var x = 1; x <= 2 * m; x++)
        {
            result = Math.Max(result, _suffixSum[index] - Best(index + x, Math.Max(m, x)));
        }

        return result;
    }

    [Benchmark]
    public int MemoizedRecursion()
        => Memoizer.Memoize<(int Index, int M), int>((0, 1), BestMemoized);

    private int BestMemoized((int Index, int M) state, Func<(int Index, int M), int> best)
    {
        var (index, m) = state;
        if (index + (2 * m) >= PileCount)
        {
            return _suffixSum[index];
        }

        var result = 0;
        for (var x = 1; x <= 2 * m; x++)
        {
            result = Math.Max(result, _suffixSum[index] - best((index + x, Math.Max(m, x))));
        }

        return result;
    }
}
