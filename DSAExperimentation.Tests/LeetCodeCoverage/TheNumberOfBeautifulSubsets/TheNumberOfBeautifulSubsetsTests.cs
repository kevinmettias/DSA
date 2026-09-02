using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheNumberOfBeautifulSubsets;

// LeetCode 2597. The Number of Beautiful Subsets: this repo's own Backtrack.Search
// engine (the same "fold the legality rule into Candidates so an illegal choice is
// never made" shape BeautifulArrangementTests already uses), deciding
// include/exclude for each index in turn. A HashMap<value,count> tracks how many
// currently-included elements sit at each value, so "would including nums[index]
// create a |x-y| == k pair" is an O(1) pair of lookups (value-k, value+k) instead
// of scanning the partial subset built so far. Subsets are counted by index, not by
// distinct value - two equal-valued elements at different indices never conflict
// with each other (|x-x| == 0 != k for the k >= 1 this problem guarantees), so
// duplicates are free to combine, exactly as LeetCode's own examples require.
public sealed class TheNumberOfBeautifulSubsetsTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [2, 4, 6], 2, 4 },
            { [1], 1, 1 },
            { [1, 1], 1, 3 },
            { [1, 3, 5], 2, 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountBeautifulSubsets_LeetCodeStyleExamples_ReturnsExpectedCount(int[] nums, int k, int expected)
    {
        Assert.Equal(expected, CountBeautifulSubsets(nums, k));
    }

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
