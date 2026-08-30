using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Parallel Courses II (LC 1494): the textbook unmemoized bitmask recursion (every
// order in which "up to k ready courses per semester" can be chosen re-explores the
// same completed-course mask from scratch - O(3^CourseCount)-ish blowup from
// overlapping subproblems) vs. the identical recursion routed through this repo's own
// Memoizer, keyed on the completed-course bitmask (each of the 2^CourseCount states
// solved exactly once). CourseCount has zero prerequisites, so every course is always
// "ready" - the same "force the real worst case" shape CanIWinBenchmarks already uses,
// here maximizing how many different orders can reach the same mask.
[MemoryDiagnoser]
public class ParallelCoursesIIBenchmarks
{
    private const int K = 2;

    [Params(6, 8)]
    public int CourseCount;

    private int _fullMask;

    [GlobalSetup]
    public void Setup() => _fullMask = (1 << CourseCount) - 1;

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() => SemestersFrom(0);

    private int SemestersFrom(int completedMask)
    {
        if (completedMask == _fullMask)
        {
            return 0;
        }

        var ready = _fullMask & ~completedMask;
        var best = int.MaxValue;
        for (var subset = ready; subset > 0; subset = (subset - 1) & ready)
        {
            if (PopCount(subset) > K)
            {
                continue;
            }

            best = Math.Min(best, SemestersFrom(completedMask | subset));
        }

        return 1 + best;
    }

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<int, int>(0, (completedMask, semestersFrom) =>
    {
        if (completedMask == _fullMask)
        {
            return 0;
        }

        var ready = _fullMask & ~completedMask;
        var best = int.MaxValue;
        for (var subset = ready; subset > 0; subset = (subset - 1) & ready)
        {
            if (PopCount(subset) > K)
            {
                continue;
            }

            best = Math.Min(best, semestersFrom(completedMask | subset));
        }

        return 1 + best;
    });

    private static int PopCount(int value)
    {
        var count = 0;
        while (value != 0)
        {
            value &= value - 1;
            count++;
        }

        return count;
    }
}
