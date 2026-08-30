using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Partition to K Equal Sum Subsets (LC 698): the same MatchsticksToSquareBenchmarks
// shape (a hand-specialized recursion vs. this repo's generic Backtrack.TrySearch
// closed over the identical choose/explore/unchoose steps) generalized from a fixed
// 4-bucket split to K buckets. _nums is K interleaved copies of 1..NumbersPerSubset,
// so a perfect split always exists (each subset re-assembles the copy it came from)
// but the shuffled ordering still forces a real search rather than an immediate match.
[MemoryDiagnoser]
public class PartitionToKEqualSumSubsetsBenchmarks
{
    private const int NumbersPerSubset = 6;

    [Params(3, 5)]
    public int K;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var perSubset = Enumerable.Range(1, NumbersPerSubset).ToArray();
        var all = new List<int>();

        for (var subset = 0; subset < K; subset++)
        {
            all.AddRange(perSubset);
        }

        var random = new Random(2);
        _nums = all.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool NaiveBacktracking()
    {
        var sorted = (int[])_nums.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        var target = sorted.Sum() / K;
        var buckets = new int[K];

        return Search(0);

        bool Search(int index)
        {
            if (index == sorted.Length)
            {
                return true;
            }

            for (var bucket = 0; bucket < K; bucket++)
            {
                if (buckets[bucket] + sorted[index] > target)
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
        var sorted = (int[])_nums.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        var target = sorted.Sum() / K;
        var state = new State(sorted, target, K);

        return Backtrack.TrySearch<State, int>(state, new BacktrackingSteps<State, int>(
            IsSolution: s => s.Index == sorted.Length,
            Candidates: s => s.Index == sorted.Length ? [] : Enumerable.Range(0, K).Where(s.CanPlace),
            Choose: (s, bucket) => s.Place(bucket),
            Unchoose: (s, bucket) => s.Remove(bucket),
            OnSolution: _ => true));
    }

    private sealed class State(int[] nums, int target, int k)
    {
        private readonly int[] _buckets = new int[k];

        public int Index { get; private set; }

        public bool CanPlace(int bucket) => _buckets[bucket] + nums[Index] <= target;

        public void Place(int bucket)
        {
            _buckets[bucket] += nums[Index];
            Index++;
        }

        public void Remove(int bucket)
        {
            Index--;
            _buckets[bucket] -= nums[Index];
        }
    }
}
