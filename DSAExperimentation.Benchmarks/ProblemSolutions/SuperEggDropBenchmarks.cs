using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Super Egg Drop (LC 887): the textbook memoized DP that exhaustively scans every
// candidate trial floor at each (eggs, floors) state - O(eggs * floors^2) - vs.
// binary-searching for the trial floor that balances the "breaks" and "survives"
// branches at each state, since WorstCaseMoves(trial) is monotonic in trial -
// O(eggs * floors * log(floors)). Both share this repo's own
// Memoizer<TState,TResult> for the (eggs, floors) cache (GuessNumberHigherOrLowerII
// Benchmarks' own shape); only how the next trial floor is chosen differs.
[MemoryDiagnoser]
public class SuperEggDropBenchmarks
{
    private const int Eggs = 2;

    [Params(50, 400)]
    public int Floors;

    [Benchmark(Baseline = true)]
    public int LinearScanDp()
        => Memoizer.Memoize<(int Eggs, int Floors), int>((Eggs, Floors), LinearScanMoves);

    private static int LinearScanMoves((int Eggs, int Floors) state, Func<(int Eggs, int Floors), int> movesFor)
    {
        var (eggs, floors) = state;
        if (floors == 0)
        {
            return 0;
        }

        if (eggs == 1)
        {
            return floors;
        }

        var best = int.MaxValue;

        for (var trial = 1; trial <= floors; trial++)
        {
            var breaks = movesFor((eggs - 1, trial - 1));
            var survives = movesFor((eggs, floors - trial));
            best = Math.Min(best, 1 + Math.Max(breaks, survives));
        }

        return best;
    }

    [Benchmark]
    public int BinarySearchDp()
        => Memoizer.Memoize<(int Eggs, int Floors), int>((Eggs, Floors), BinarySearchMoves);

    private static int BinarySearchMoves((int Eggs, int Floors) state, Func<(int Eggs, int Floors), int> movesFor)
    {
        var (eggs, floors) = state;
        if (floors == 0)
        {
            return 0;
        }

        if (eggs == 1)
        {
            return floors;
        }

        var low = 1;
        var high = floors;
        var best = int.MaxValue;

        while (low <= high)
        {
            var trial = (low + high) / 2;
            var breaks = movesFor((eggs - 1, trial - 1));
            var survives = movesFor((eggs, floors - trial));
            best = Math.Min(best, 1 + Math.Max(breaks, survives));

            if (breaks < survives)
            {
                low = trial + 1;
            }
            else
            {
                high = trial - 1;
            }
        }

        return best;
    }
}
