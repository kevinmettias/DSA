using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Predict the Winner (LC 486): plain un-memoized minimax recursion over (left, right)
// bounds - exponential, since the same (left, right) sub-range recurs through many
// different pick orders - vs. this repo's own Memoizer<TState,TResult> caching that
// exact pair, the same shape GuessNumberHigherOrLowerIIBenchmarks already uses for
// its own interval-DP game. N is kept modest specifically because the un-memoized
// baseline's blowup is real, the same reasoning FibonacciBenchmarks documents.
[MemoryDiagnoser]
public class PredictTheWinnerBenchmarks
{
    [Params(22, 26)]
    public int N;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, N).Select(_ => random.Next(1, 100)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => ScoreDiff(0, N - 1);

    private int ScoreDiff(int left, int right)
    {
        if (left == right)
        {
            return _nums[left];
        }

        var takeLeft = _nums[left] - ScoreDiff(left + 1, right);
        var takeRight = _nums[right] - ScoreDiff(left, right - 1);
        return Math.Max(takeLeft, takeRight);
    }

    [Benchmark]
    public int MemoizedRecursion()
        => Memoizer.Memoize<(int Left, int Right), int>((0, N - 1), ScoreDiffMemoized);

    private int ScoreDiffMemoized((int Left, int Right) range, Func<(int Left, int Right), int> bestDiff)
    {
        var (left, right) = range;
        if (left == right)
        {
            return _nums[left];
        }

        var takeLeft = _nums[left] - bestDiff((left + 1, right));
        var takeRight = _nums[right] - bestDiff((left, right - 1));
        return Math.Max(takeLeft, takeRight);
    }
}
