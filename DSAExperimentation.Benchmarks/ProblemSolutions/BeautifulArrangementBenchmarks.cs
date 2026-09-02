using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Beautiful Arrangement (LC 526): generate every full permutation of [1..n] and
// reject it afterward (O(n!) permutations, each fully materialized regardless of
// how early it violates the rule) vs. this repo's own Backtrack.Search with the
// v % position == 0 / position % v == 0 rule folded directly into Candidates, so an
// illegal value is never placed and the branch is pruned immediately instead of
// discovered n steps later.
[MemoryDiagnoser]
public class BeautifulArrangementBenchmarks
{
    [Params(6, 8)]
    public int N;

    [Benchmark(Baseline = true)]
    public int GenerateThenFilter()
    {
        var n = N;
        var arrangement = new ArrangementState(n, new bool[n], new int[n]);

        return Generate(arrangement, 0);
    }

    private static int Generate(ArrangementState arrangement, int depth)
    {
        if (depth == arrangement.N)
        {
            return IsBeautiful(arrangement) ? 1 : 0;
        }

        var count = 0;

        for (var v = 1; v <= arrangement.N; v++)
        {
            if (!arrangement.Used[v - 1])
            {
                count += PlaceAndRecurse(arrangement, depth, v);
            }
        }

        return count;
    }

    private static int PlaceAndRecurse(ArrangementState arrangement, int depth, int v)
    {
        arrangement.Used[v - 1] = true;
        arrangement.Values[depth] = v;
        var count = Generate(arrangement, depth + 1);
        arrangement.Used[v - 1] = false;

        return count;
    }

    private static bool IsBeautiful(ArrangementState arrangement)
    {
        for (var position = 1; position <= arrangement.N; position++)
        {
            var v = arrangement.Values[position - 1];
            if (v % position != 0 && position % v != 0)
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public int PrunedBacktracking()
    {
        var n = N;
        var count = 0;
        var state = new State(n);

        Backtrack.Search<State, int>(
            state,
            s => s.Values.Count == n,
            s =>
            {
                if (s.Values.Count == n)
                {
                    return [];
                }

                var position = s.Values.Count + 1;
                return Enumerable.Range(1, n).Where(v => !s.Used[v - 1] && (v % position == 0 || position % v == 0));
            },
            (s, v) => { s.Used[v - 1] = true; s.Values.Add(v); },
            (s, v) => { s.Used[v - 1] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            _ => count++);

        return count;
    }

    private sealed class State(int length) { public bool[] Used { get; } = new bool[length]; public List<int> Values { get; } = []; }

    private readonly record struct ArrangementState(int N, bool[] Used, int[] Values);
}
