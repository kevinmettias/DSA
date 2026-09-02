using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stone Game IX (LC 2029): a full game-tree negamax search over remaining
// (count0,count1,count2,runningSumMod3) state - O(n^3) states, memoized by this
// repo's own Memoizer<TState,TResult> - vs. bucketing stones by value mod 3 with
// this repo's own HashMap<int,int> and reading the answer off a closed-form parity
// rule in O(n). Both explore the identical rule set (a move landing the running sum
// on a multiple of 3 loses immediately for whoever made it); the game-tree side just
// re-derives that fact by exhaustive search instead of the closed form.
[MemoryDiagnoser]
public class StoneGameIXBenchmarks
{
    private const int MaxStoneValueExclusive = 1_000;
    private const int RemainderBucketCount = 3;
    private const int RemainderTwoIndex = 2;
    private const int EvenCountDivisor = 2;
    private const int MinimumImbalanceThreshold = 3;

    [Params(20, 50)]
    public int Length;

    private int[] _stones = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _stones = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxStoneValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool GameTreeMinimax()
    {
        var counts = CountByRemainder(_stones);
        var outcome = Memoizer.Memoize<(int C0, int C1, int C2, int TurnSum), int>(
            (counts[0], counts[1], counts[RemainderTwoIndex], 0),
            SolveState);

        return outcome == 1;
    }

    private static int[] CountByRemainder(int[] stones)
    {
        var counts = new int[RemainderBucketCount];
        foreach (var stone in stones)
        {
            counts[stone % RemainderBucketCount]++;
        }

        return counts;
    }

    private static int SolveState(
        (int C0, int C1, int C2, int TurnSum) state, Func<(int, int, int, int), int> solve)
    {
        var (c0, c1, c2, _) = state;
        if (c0 + c1 + c2 == 0)
        {
            return 0;
        }

        return BestOverRemainders(state, solve);
    }

    private static int BestOverRemainders(
        (int C0, int C1, int C2, int TurnSum) state, Func<(int, int, int, int), int> solve)
    {
        var (c0, c1, c2, _) = state;
        var counts = new[] { c0, c1, c2 };
        var best = int.MinValue;

        for (var r = 0; r < RemainderBucketCount; r++)
        {
            if (counts[r] == 0)
            {
                continue;
            }

            var nextState = NextState(state, r);
            var branchValue = Branch(r, nextState, solve);
            best = Math.Max(best, branchValue);
        }

        return best;
    }

    private static GameState NextState((int C0, int C1, int C2, int TurnSum) state, int r)
    {
        var (c0, c1, c2, turnSum) = state;
        return r switch
        {
            0 => new GameState(c0 - 1, c1, c2, turnSum),
            1 => new GameState(c0, c1 - 1, c2, turnSum),
            _ => new GameState(c0, c1, c2 - 1, turnSum),
        };
    }

    // Negamax step: a move of remainder r updates the running sum; landing on a
    // multiple of 3 loses immediately for the mover (-1), otherwise the outcome is
    // whatever the opponent's own best play yields, negated back to this mover's
    // perspective.
    private static int Branch(int r, GameState state, Func<(int, int, int, int), int> solve)
    {
        var newSum = (state.TurnSum + r) % RemainderBucketCount;
        return newSum == 0 ? -1 : -solve((state.C0, state.C1, state.C2, newSum));
    }

    private readonly record struct GameState(int C0, int C1, int C2, int TurnSum);

    [Benchmark]
    public bool ClosedFormHashMapCounting()
    {
        var counts = new HashMap<int, int>();

        foreach (var stone in _stones)
        {
            counts.TryGetValue(stone % RemainderBucketCount, out var c);
            counts.Set(stone % RemainderBucketCount, c + 1);
        }

        counts.TryGetValue(0, out var cnt0);
        counts.TryGetValue(1, out var cnt1);
        counts.TryGetValue(RemainderTwoIndex, out var cnt2);

        return cnt0 % EvenCountDivisor == 0
            ? cnt1 >= 1 && cnt2 >= 1
            : Math.Abs(cnt1 - cnt2) >= MinimumImbalanceThreshold;
    }
}
