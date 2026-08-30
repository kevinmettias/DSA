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
    [Params(8, 10)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, 50)).ToArray();
        Array.Sort(_nums);
    }

    [Benchmark(Baseline = true)]
    public int GenerateThenFilter() => CountViaFullPermutations(_nums);

    [Benchmark]
    public int PrunedBacktrack() => CountViaBacktrack(_nums);

    private static int CountViaFullPermutations(int[] nums)
    {
        var count = 0;
        var used = new bool[nums.Length];
        var current = new int[nums.Length];

        void Permute(int depth)
        {
            if (depth == nums.Length)
            {
                if (IsSquareful(current))
                {
                    count++;
                }

                return;
            }

            for (var i = 0; i < nums.Length; i++)
            {
                if (used[i] || (i > 0 && nums[i] == nums[i - 1] && !used[i - 1]))
                {
                    continue;
                }

                used[i] = true;
                current[depth] = nums[i];
                Permute(depth + 1);
                used[i] = false;
            }
        }

        Permute(0);
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
}
