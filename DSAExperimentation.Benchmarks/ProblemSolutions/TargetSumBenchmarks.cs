using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Target Sum (LC 494): plain un-memoized +/- recursion over (index, runningSum) -
// exponential, since the same (index, runningSum) pair recurs through many different
// sign-choice paths - vs. this repo's own Memoizer<TState,TResult> caching that exact
// pair, the same shape PredictTheWinnerBenchmarks already uses. N is kept modest
// specifically because the un-memoized baseline's 2^N blowup is real, the same
// reasoning FibonacciNumberBenchmarks documents.
[MemoryDiagnoser]
public class TargetSumBenchmarks
{
    [Params(18, 22)]
    public int N;

    private int[] _nums = null!;
    private int _target;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(494);
        _nums = Enumerable.Range(0, N).Select(_ => random.Next(1, 10)).ToArray();
        _target = _nums.Sum() - (2 * _nums[0]);
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => CountWays(0, 0);

    private int CountWays(int index, int sum)
    {
        if (index == _nums.Length)
        {
            return sum == _target ? 1 : 0;
        }

        return CountWays(index + 1, sum + _nums[index]) + CountWays(index + 1, sum - _nums[index]);
    }

    [Benchmark]
    public int MemoizedRecursion()
        => Memoizer.Memoize<(int Index, int Sum), int>((0, 0), CountWaysMemoized);

    private int CountWaysMemoized((int Index, int Sum) state, Func<(int Index, int Sum), int> ways)
    {
        if (state.Index == _nums.Length)
        {
            return state.Sum == _target ? 1 : 0;
        }

        return ways((state.Index + 1, state.Sum + _nums[state.Index]))
             + ways((state.Index + 1, state.Sum - _nums[state.Index]));
    }
}
