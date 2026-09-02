using static DSAExperimentation.LeetCode.RandomPickIndex.RandomPickIndexSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomPickIndex;

// Harness only. Both strategies are RandomPickIndexSolution's - this replays
// repeated Pick(target) calls against each IRandomPickIndex implementation built
// from LeetCode's published nums. Pick's result is nondeterministic, so each
// example carries the full set of indices that are valid to return for its
// target rather than one exact value - the same "expected slot carries a
// candidate set" idea InsertDeleteGetRandomO1Tests already uses for GetRandom -
// plus, where the original test asserted it, that every valid index eventually
// gets returned across many calls.
public sealed class RandomPickIndexTests
{
    public static TheoryData<int[], int, int[], int, bool> Examples =>
        new()
        {
            { [1, 2, 3, 3, 3], 3, [2, 3, 4], 50, false },
            { [5, 1, 5, 2, 5, 5], 2, [3], 1, true },
            { [5, 1, 5, 2, 5, 5], 5, [0, 2, 4, 5], 200, true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void Pick_LeetCodeExamples_AlwaysReturnsAValidIndexByReservoirSampling(
        int[] nums, int target, int[] validIndices, int trials, bool expectAllSeen)
        => AssertPicksAreValid(
            new RandomPickIndexByReservoirSampling(nums), target, validIndices, trials, expectAllSeen);

    [Theory]
    [MemberData(nameof(Examples))]
    public void Pick_LeetCodeExamples_AlwaysReturnsAValidIndexByHashMapGrouping(
        int[] nums, int target, int[] validIndices, int trials, bool expectAllSeen)
        => AssertPicksAreValid(
            new RandomPickIndexByHashMapGrouping(nums), target, validIndices, trials, expectAllSeen);

    private static void AssertPicksAreValid(
        IRandomPickIndex solution, int target, int[] validIndices, int trials, bool expectAllSeen)
    {
        var seen = new HashSet<int>();

        for (var i = 0; i < trials; i++)
        {
            var picked = solution.Pick(target);
            Assert.Contains(picked, validIndices);
            seen.Add(picked);
        }

        if (expectAllSeen)
        {
            Assert.Equal(validIndices.OrderBy(index => index), seen.OrderBy(index => index));
        }
    }
}
