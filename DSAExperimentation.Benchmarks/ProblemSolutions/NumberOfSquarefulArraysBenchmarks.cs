using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Squareful Arrays (LC 996): generate every full permutation and only
// then filter for the squareful property (the textbook O(n!) approach, no
// pruning) vs. this repo's own Backtrack.Search, which folds the "adjacent sum is
// a perfect square" check into Candidates so an invalid partial permutation is
// abandoned the moment its last placed pair fails, instead of after every
// remaining position has already been filled in.
[MemoryDiagnoser]
public class NumberOfSquarefulArraysBenchmarks
{
    private const int ValueUpperBound = 50;

    [Params(8, 10)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBound)).ToArray();
        Array.Sort(_nums);
    }

    [Benchmark(Baseline = true)]
    public int GenerateThenFilter() => CountViaFullPermutations(_nums);

    [Benchmark]
    public int PrunedBacktrack() => CountViaBacktrack(_nums);

    private static int CountViaFullPermutations(int[] nums)
    {
        var state = new PermutationState(nums);
        return Permute(state, 0);
    }

    private static int Permute(PermutationState state, int depth)
    {
        if (depth == state.Nums.Length)
        {
            return IsSquareful(state.Current) ? 1 : 0;
        }

        var count = 0;

        for (var i = 0; i < state.Nums.Length; i++)
        {
            count += TryPlace(state, depth, i);
        }

        return count;
    }

    private static int TryPlace(PermutationState state, int depth, int i)
    {
        var used = state.Used;
        var nums = state.Nums;

        if (used[i] || (i > 0 && nums[i] == nums[i - 1] && !used[i - 1]))
        {
            return 0;
        }

        used[i] = true;
        state.Current[depth] = nums[i];
        var count = Permute(state, depth + 1);
        used[i] = false;

        return count;
    }

    private static bool IsSquareful(int[] arrangement)
    {
        for (var i = 1; i < arrangement.Length; i++)
        {
            if (!IsPerfectSquare(arrangement[i] + arrangement[i - 1]))
            {
                return false;
            }
        }

        return true;
    }

    private static int CountViaBacktrack(int[] nums)
    {
        var count = 0;
        var state = new State(nums.Length);

        Backtrack.Search<State, int>(
            state,
            s => s.Values.Count == nums.Length,
            s => s.Values.Count == nums.Length
                ? []
                : Enumerable.Range(0, nums.Length).Where(i =>
                    !s.Used[i] &&
                    (i == 0 || nums[i] != nums[i - 1] || s.Used[i - 1]) &&
                    (s.Values.Count == 0 || IsPerfectSquare(nums[i] + s.Values[^1]))),
            (s, i) => { s.Used[i] = true; s.Values.Add(nums[i]); },
            (s, i) => { s.Used[i] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            _ => count++);

        return count;
    }

    private static bool IsPerfectSquare(int value)
    {
        var root = (int)Math.Sqrt(value);
        return root * root == value || (root + 1) * (root + 1) == value;
    }

    private sealed class State(int length)
    {
        public bool[] Used { get; } = new bool[length];
        public List<int> Values { get; } = [];
    }

    private sealed class PermutationState(int[] nums)
    {
        public int[] Nums { get; } = nums;
        public bool[] Used { get; } = new bool[nums.Length];
        public int[] Current { get; } = new int[nums.Length];
    }
}
