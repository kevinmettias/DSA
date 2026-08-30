using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stone Game (LC 877): plain un-memoized minimax recursion over (left, right)
// bounds - exponential, since the same (left, right) sub-range recurs through
// many different pick orders - vs. this repo's own Memoizer<TState,TResult>
// caching that exact pair, the identical shape PredictTheWinnerBenchmarks
// already uses for its own interval-DP game (LC 877 is the same recurrence as
// LC 486, just a different win condition on the resulting score difference). N
// is kept modest for the same reason PredictTheWinnerBenchmarks documents: the
// un-memoized baseline's blowup is real.
[MemoryDiagnoser]
public class StoneGameBenchmarks
{
    [Params(22, 26)]
    public int N;

    private int[] _piles = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(877);
        _piles = Enumerable.Range(0, N).Select(_ => random.Next(1, 100)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => ScoreDiff(0, N - 1);

    private int ScoreDiff(int left, int right)
    {
        if (left == right)
        {
            return _piles[left];
        }

        var takeLeft = _piles[left] - ScoreDiff(left + 1, right);
        var takeRight = _piles[right] - ScoreDiff(left, right - 1);
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
            return _piles[left];
        }

        var takeLeft = _piles[left] - bestDiff((left + 1, right));
        var takeRight = _piles[right] - bestDiff((left, right - 1));
        return Math.Max(takeLeft, takeRight);
    }
}
