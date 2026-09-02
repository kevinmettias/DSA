using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Work Sessions to Finish the Tasks (LC 1986): the same bitmask-over-remaining-
// tasks recurrence either way - UnmemoizedRecursion re-derives Solve(mask) from scratch every time
// a different session split reaches the same remaining set (many splits reach the same mask, so
// the call tree revisits it repeatedly), while MemoizedBitmaskDp wires that exact recurrence through
// this repo's own Memoizer (ParallelCoursesII/SmallestSufficientTeam precedent) so each mask's
// answer is computed once and every later split that reaches it is an O(1) cache hit. Task durations
// are all 1 with sessionTime 2, so both single tasks and pairs are feasible sessions - real
// combinatorial choice, not a forced single grouping - which is exactly what makes the unmemoized
// call tree blow up: TaskCount stays small (<=9) because that blowup is factorial-ish and would
// otherwise make the baseline impractically slow, unlike the DP side's ~3^n submask-enumeration cost.
[MemoryDiagnoser]
public class MinimumNumberOfWorkSessionsToFinishTheTasksBenchmarks
{
    private const int SessionTime = 2;
    private const int TaskDuration = 1;

    [Params(6, 9)]
    public int TaskCount;

    private bool[] _feasible = null!;
    private int _fullMask;

    [GlobalSetup]
    public void Setup()
    {
        var tasks = Enumerable.Repeat(TaskDuration, TaskCount).ToArray();
        _feasible = ComputeFeasibleMasks(tasks, SessionTime);
        _fullMask = (1 << TaskCount) - 1;
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Solve(_fullMask);

    private int Solve(int remaining)
    {
        if (remaining == 0)
        {
            return 0;
        }

        var best = int.MaxValue;

        for (var sub = remaining; sub > 0; sub = (sub - 1) & remaining)
        {
            if (_feasible[sub])
            {
                best = Math.Min(best, 1 + Solve(remaining ^ sub));
            }
        }

        return best;
    }

    [Benchmark]
    public int MemoizedBitmaskDp() => Memoizer.Memoize<int, int>(_fullMask, (remaining, sessionsFor) =>
        remaining == 0 ? 0 : 1 + BestOverFeasibleSubsets(remaining, sessionsFor));

    private int BestOverFeasibleSubsets(int remaining, Func<int, int> sessionsFor)
    {
        var best = int.MaxValue;

        for (var sub = remaining; sub > 0; sub = (sub - 1) & remaining)
        {
            if (_feasible[sub])
            {
                best = Math.Min(best, sessionsFor(remaining ^ sub));
            }
        }

        return best;
    }

    private static bool[] ComputeFeasibleMasks(int[] tasks, int sessionTime)
    {
        var maskCount = 1 << tasks.Length;
        var sum = new int[maskCount];
        var feasible = new bool[maskCount];

        for (var mask = 1; mask < maskCount; mask++)
        {
            var lowestBit = mask & -mask;
            var taskIndex = TrailingZeroCount(lowestBit);
            sum[mask] = sum[mask ^ lowestBit] + tasks[taskIndex];
            feasible[mask] = sum[mask] <= sessionTime;
        }

        return feasible;
    }

    private static int TrailingZeroCount(int lowestBit)
    {
        var index = 0;

        while ((lowestBit & 1) == 0)
        {
            lowestBit >>= 1;
            index++;
        }

        return index;
    }
}
