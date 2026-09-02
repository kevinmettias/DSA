using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Special Permutations (LC 2741): BruteForceBacktracking enumerates every one of the
// n! full permutations via this repo's own Backtrack.Search (same shape
// PermutationsBenchmarks already exercises) before checking the adjacency rule once a
// full ordering exists - Length has to stay small enough for this arm to finish.
// BitmaskMemo instead threads (Remaining, Last) through this repo's own Memoizer in a
// single call, the SpecialPermutationsTests precedent, visiting each of the
// O(n * 2^n) reachable states at most once regardless of how many of the n! orderings
// would have been legal.
[MemoryDiagnoser]
public class SpecialPermutationsBenchmarks
{
    private const int Modulo = 1_000_000_007;
    private const int MaxValueExclusive = 60;
    private const int Seed = 2741;

    [Params(8, 10)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var distinct = new HashSet<int>();

        while (distinct.Count < Length)
        {
            distinct.Add(random.Next(1, MaxValueExclusive));
        }

        _nums = [.. distinct];
    }

    [Benchmark(Baseline = true)]
    public int BruteForceBacktracking()
    {
        var count = 0;
        var state = new PermutationState(_nums.Length);

        Backtrack.Search<PermutationState, int>(
            state,
            s => s.Values.Count == _nums.Length,
            s => s.Values.Count == _nums.Length
                ? []
                : Enumerable.Range(0, _nums.Length).Where(i => !s.Used[i]),
            (s, i) => { s.Used[i] = true; s.Values.Add(_nums[i]); },
            (s, i) => { s.Used[i] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            s =>
            {
                if (IsSpecial(s.Values))
                {
                    count++;
                }
            });

        return count;
    }

    private static bool IsSpecial(List<int> permutation)
    {
        for (var i = 0; i < permutation.Count - 1; i++)
        {
            var (a, b) = (permutation[i], permutation[i + 1]);
            if (a % b != 0 && b % a != 0)
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public long BitmaskMemo()
    {
        var fullMask = (1 << _nums.Length) - 1;

        long Recurrence((int Remaining, int Last) state, Func<(int Remaining, int Last), long> ways)
        {
            var (remaining, last) = state;
            if (remaining == 0)
            {
                return 1L;
            }

            var total = 0L;

            for (var next = 0; next < _nums.Length; next++)
            {
                if ((remaining & (1 << next)) == 0)
                {
                    continue;
                }

                if (last != -1 && _nums[last] % _nums[next] != 0 && _nums[next] % _nums[last] != 0)
                {
                    continue;
                }

                total = (total + ways((remaining & ~(1 << next), next))) % Modulo;
            }

            return total;
        }

        return Memoizer.Memoize<(int Remaining, int Last), long>((fullMask, -1), Recurrence);
    }

    private sealed class PermutationState(int length)
    {
        public bool[] Used { get; } = new bool[length];
        public List<int> Values { get; } = [];
    }
}
