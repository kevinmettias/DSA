using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Tallest Billboard (LC 956): plain un-memoized skip/taller/shorter recursion over
// (index, diff) - exponential (3^N), since the same (index, diff) pair recurs through
// many different skip/taller/shorter choice paths - vs. this repo's own
// Memoizer<TState,TResult> caching that exact pair, the same (index, runningState) shape
// TargetSumBenchmarks already uses. N is kept modest specifically because the
// un-memoized baseline's 3^N blowup is real, the same reasoning TargetSumBenchmarks
// documents for its own 2^N baseline.
[MemoryDiagnoser]
public class TallestBillboardBenchmarks
{
    private const int Unreachable = int.MinValue / 2;
    private const int RandomSeed = 956; // LC problem number
    private const int MaxRodLength = 50; // exclusive upper bound passed to Random.Next

    [Params(12, 14)]
    public int N;

    private int[] _rods = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _rods = Enumerable.Range(0, N).Select(_ => random.Next(1, MaxRodLength)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Solve(0, 0);

    private int Solve(int index, int diff)
    {
        if (index == _rods.Length)
        {
            return diff == 0 ? 0 : Unreachable;
        }

        var rod = _rods[index];
        var skip = Solve(index + 1, diff);
        var addToTaller = rod + Solve(index + 1, diff + rod);
        var addToShorter = Solve(index + 1, diff - rod);
        var bestAddChoice = Math.Max(addToTaller, addToShorter);

        return Math.Max(skip, bestAddChoice);
    }

    [Benchmark]
    public int MemoizedRecursion()
        => Memoizer.Memoize<(int Index, int Diff), int>((0, 0), SolveMemoized);

    private int SolveMemoized((int Index, int Diff) state, Func<(int Index, int Diff), int> solve)
    {
        if (state.Index == _rods.Length)
        {
            return state.Diff == 0 ? 0 : Unreachable;
        }

        var rod = _rods[state.Index];
        var skip = solve((state.Index + 1, state.Diff));
        var addToTaller = rod + solve((state.Index + 1, state.Diff + rod));
        var addToShorter = solve((state.Index + 1, state.Diff - rod));
        var bestAddChoice = Math.Max(addToTaller, addToShorter);

        return Math.Max(skip, bestAddChoice);
    }
}
