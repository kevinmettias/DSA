using DSAExperimentation.LeetCode.FindTheScoreDifferenceInAGame;

namespace DSAExperimentation.LeetCode.Tests.FindTheScoreDifferenceInAGame;

// Harness only. Both strategies are FindTheScoreDifferenceInAGameSolution's. The first three
// rows are LeetCode's published examples; the rest are derived by hand from the statement's rules:
// - twelve 2s: no odd swaps; the first player takes games 0-4, the swap at game 5 hands games 5-10
//   to the second, and the swap at game 11 hands it back - 12 points each, difference 0;
// - six 1s: every game swaps, and game 5 swaps twice - the second player takes games 0, 2, 4 and
//   5, the first takes 1 and 3, so 2 - 4 = -2;
// - [7..13]: the second takes 7, 8, 11 and 13, the first takes 9, 10 and 12 (game 5's even 12
//   lands on the first player only through the 6th-game swap), so 31 - 39 = -8.
public sealed partial class FindTheScoreDifferenceInAGameSolutionTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3], 0 },
            { [2, 4, 2, 1, 2, 1], 4 },
            { [1], -1 },
            { [2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2], 0 },
            { [1, 1, 1, 1, 1, 1], -2 },
            { [7, 8, 9, 10, 11, 12, 13], -8 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ScoreDifferenceByRoleSimulation_Examples_ReturnsFirstMinusSecondTotal(int[] nums, int expected) =>
        Assert.Equal(expected, FindTheScoreDifferenceInAGameSolution.ScoreDifferenceByRoleSimulation(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ScoreDifferenceBySwapParity_Examples_ReturnsFirstMinusSecondTotal(int[] nums, int expected) =>
        Assert.Equal(expected, FindTheScoreDifferenceInAGameSolution.ScoreDifferenceBySwapParity(nums));
}
