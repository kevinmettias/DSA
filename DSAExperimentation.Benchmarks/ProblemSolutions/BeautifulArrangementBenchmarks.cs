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
        var used = new bool[n];
        var values = new int[n];
        var count = 0;

        void Generate(int depth)
        {
            if (depth == n)
            {
                for (var position = 1; position <= n; position++)
                {
                    var v = values[position - 1];
                    if (v % position != 0 && position % v != 0)
                    {
                        return;
                    }
                }

                count++;
                return;
            }

            for (var v = 1; v <= n; v++)
            {
                if (!used[v - 1])
                {
                    used[v - 1] = true;
                    values[depth] = v;
                    Generate(depth + 1);
                    used[v - 1] = false;
                }
            }
        }

        Generate(0);
        return count;
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
}
