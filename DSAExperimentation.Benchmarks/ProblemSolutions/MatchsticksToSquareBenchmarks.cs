using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Matchsticks to Square (LC 473): the same NQueensBenchmarks shape (a hand-specialized
// recursion vs. this repo's generic Backtrack.TrySearch closed over the identical
// choose/explore/unchoose steps) applied to a 4-bucket equal-sum partition instead of
// an N-queens board. _matchsticks is four interleaved copies of 1..SticksPerSide, so a
// perfect split always exists (each side re-assembles the copy it came from) but the
// shuffled ordering still forces a real search rather than an immediate match.
[MemoryDiagnoser]
public class MatchsticksToSquareBenchmarks
{
    [Params(6, 8)]
    public int SticksPerSide;

    private int[] _matchsticks = null!;

    [GlobalSetup]
    public void Setup()
    {
        var perSide = Enumerable.Range(1, SticksPerSide).ToArray();
        var all = new List<int>();

        for (var side = 0; side < 4; side++)
        {
            all.AddRange(perSide);
        }

        var random = new Random(1);
        _matchsticks = all.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool NaiveBacktracking()
    {
        var sorted = (int[])_matchsticks.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        var side = sorted.Sum() / 4;
        var buckets = new int[4];

        return Search(0);

        bool Search(int index)
        {
            if (index == sorted.Length)
            {
                return true;
            }

            for (var bucket = 0; bucket < 4; bucket++)
            {
                if (buckets[bucket] + sorted[index] > side)
                {
                    continue;
                }

                buckets[bucket] += sorted[index];

                if (Search(index + 1))
                {
                    return true;
                }

                buckets[bucket] -= sorted[index];
            }

            return false;
        }
    }

    [Benchmark]
    public bool BacktrackPrimitive()
    {
        var sorted = (int[])_matchsticks.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        var side = sorted.Sum() / 4;
        var state = new State(sorted, side);

        return Backtrack.TrySearch<State, int>(state, new BacktrackingSteps<State, int>(
            IsSolution: s => s.Index == sorted.Length,
            Candidates: s => s.Index == sorted.Length ? [] : Enumerable.Range(0, 4).Where(s.CanPlace),
            Choose: (s, bucket) => s.Place(bucket),
            Unchoose: (s, bucket) => s.Remove(bucket),
            OnSolution: _ => true));
    }

    private sealed class State(int[] matchsticks, int side)
    {
        private readonly int[] _buckets = new int[4];

        public int Index { get; private set; }

        public bool CanPlace(int bucket) => _buckets[bucket] + matchsticks[Index] <= side;

        public void Place(int bucket)
        {
            _buckets[bucket] += matchsticks[Index];
            Index++;
        }

        public void Remove(int bucket)
        {
            Index--;
            _buckets[bucket] -= matchsticks[Index];
        }
    }
}
