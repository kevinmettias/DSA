using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Fair Distribution of Cookies (LC 2305): hand-rolled recursive backtracking (same
// symmetry-breaking + branch-and-bound pruning as FairDistributionOfCookiesTests) vs.
// this repo's generic Backtrack.Search closed over identical steps - the same
// hand-rolled-vs-generic-primitive shape PartitionToKEqualSumSubsetsBenchmarks
// already uses. Bags are random in [1, MaxBagSize) so a real search is needed instead
// of an immediately-degenerate all-equal split.
[MemoryDiagnoser]
public class FairDistributionOfCookiesBenchmarks
{
    private const int RandomSeed = 2305; // LC problem number
    private const int MaxBagSize = 20;
    private const int Children = 3;

    [Params(6, 8)]
    public int BagCount;

    private int[] _cookies = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _cookies = Enumerable.Range(0, BagCount).Select(_ => random.Next(1, MaxBagSize)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NaiveBacktracking()
    {
        var sorted = SortDescending(_cookies);
        var buckets = new int[Children];
        var best = sorted.Sum();
        Search(sorted, buckets, 0, ref best);
        return best;
    }

    private void Search(int[] sorted, int[] buckets, int index, ref int best)
    {
        if (index == sorted.Length)
        {
            var max = buckets.Max();
            if (max < best)
            {
                best = max;
            }

            return;
        }

        var sawEmpty = false;

        for (var child = 0; child < Children; child++)
        {
            if (buckets[child] == 0)
            {
                if (sawEmpty)
                {
                    continue;
                }

                sawEmpty = true;
            }

            if (buckets[child] + sorted[index] >= best)
            {
                continue;
            }

            buckets[child] += sorted[index];
            Search(sorted, buckets, index + 1, ref best);
            buckets[child] -= sorted[index];
        }
    }

    [Benchmark]
    public int BacktrackPrimitive()
    {
        var sorted = SortDescending(_cookies);
        var state = new State(sorted, Children);

        Backtrack.Search(
            state,
            isSolution: s => s.Index == sorted.Length,
            candidates: s => s.Index == sorted.Length ? [] : s.CandidateChildren(),
            choose: (s, child) => s.Place(child),
            unchoose: (s, child) => s.Remove(child),
            onSolution: s => s.RecordIfBetter());

        return state.Best;
    }

    private static int[] SortDescending(int[] cookies)
    {
        var sorted = (int[])cookies.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        return sorted;
    }

    private sealed class State(int[] cookies, int k)
    {
        private readonly int[] _buckets = new int[k];

        public int Index { get; private set; }

        public int Best { get; private set; } = cookies.Sum();

        public IEnumerable<int> CandidateChildren()
        {
            var sawEmpty = false;

            for (var child = 0; child < k; child++)
            {
                if (_buckets[child] == 0)
                {
                    if (sawEmpty)
                    {
                        continue;
                    }

                    sawEmpty = true;
                }

                if (_buckets[child] + cookies[Index] < Best)
                {
                    yield return child;
                }
            }
        }

        public void Place(int child)
        {
            _buckets[child] += cookies[Index];
            Index++;
        }

        public void Remove(int child)
        {
            Index--;
            _buckets[child] -= cookies[Index];
        }

        public void RecordIfBetter()
        {
            var max = _buckets.Max();
            if (max < Best)
            {
                Best = max;
            }
        }
    }
}
