using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The Number of Beautiful Subsets (LC 2597): enumerate every one of the 2^n index
// subsets as a bitmask and reject the ones containing a |x-y|==k pair afterward
// (checking all C(size,2) pairs per subset) vs. this repo's own Backtrack.Search
// with the conflict rule folded directly into Candidates via a running
// HashMap<value,count>, so an illegal inclusion is never made and the branch is
// pruned immediately instead of discovered after the fact - the same "prune in
// Candidates, don't generate-then-filter" shape BeautifulArrangementBenchmarks
// already establishes.
[MemoryDiagnoser]
public class TheNumberOfBeautifulSubsetsBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 2597;

    private const int MaxValueExclusive = 50;
    private const int K = 3;

    [Params(12, 16)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int GenerateThenFilter() => CountByBitmask(_nums, K);

    private static int CountByBitmask(int[] nums, int k)
    {
        var count = 0;
        var subsetCount = 1 << nums.Length;

        for (var mask = 1; mask < subsetCount; mask++)
        {
            if (IsBeautiful(nums, k, mask))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsBeautiful(int[] nums, int k, int mask)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            if ((mask & (1 << i)) == 0)
            {
                continue;
            }

            for (var j = i + 1; j < nums.Length; j++)
            {
                if ((mask & (1 << j)) != 0 && Math.Abs(nums[i] - nums[j]) == k)
                {
                    return false;
                }
            }
        }

        return true;
    }

    [Benchmark]
    public int PrunedBacktracking() => CountBeautifulSubsets(_nums, K);

    private static int CountBeautifulSubsets(int[] nums, int k)
    {
        var count = 0;
        var state = new State();

        Backtrack.Search<State, bool>(
            state,
            s => s.Index == nums.Length,
            s => Candidates(s, nums, k),
            (s, include) => Choose(s, nums, include),
            (s, include) => Unchoose(s, nums, include),
            s =>
            {
                if (s.Size > 0)
                {
                    count++;
                }
            });

        return count;
    }

    private static IEnumerable<bool> Candidates(State state, int[] nums, int k)
    {
        if (state.Index == nums.Length)
        {
            yield break;
        }

        yield return false;

        if (CanInclude(state, nums[state.Index], k))
        {
            yield return true;
        }
    }

    private static bool CanInclude(State state, int value, int k)
    {
        state.Frequency.TryGetValue(value - k, out var lower);
        state.Frequency.TryGetValue(value + k, out var upper);
        return lower == 0 && upper == 0;
    }

    private static void Choose(State state, int[] nums, bool include)
    {
        if (include)
        {
            var value = nums[state.Index];
            state.Frequency.TryGetValue(value, out var current);
            state.Frequency.Set(value, current + 1);
            state.Size++;
        }

        state.Index++;
    }

    private static void Unchoose(State state, int[] nums, bool include)
    {
        state.Index--;

        if (include)
        {
            var value = nums[state.Index];
            state.Frequency.TryGetValue(value, out var current);
            state.Frequency.Set(value, current - 1);
            state.Size--;
        }
    }

    private sealed class State
    {
        public int Index { get; set; }

        public int Size { get; set; }

        public HashMap<int, int> Frequency { get; } = new();
    }
}
