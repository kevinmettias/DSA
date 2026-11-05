using DSAExperimentation.LeetCode.Solutions.SortItemsByGroupsRespectingDependencies;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SortItemsByGroupsRespectingDependencies;

public sealed partial class SortItemsByGroupsRespectingDependenciesAnswerCheckTests
{
    public static TheoryData<int, int, int[], int[][], int[], int[], bool> Answers =>
        new()
        {
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3, 6], [], [], []], [6, 3, 4, 1, 5, 2, 0, 7], [6, 3, 4, 1, 5, 2, 0, 7], true },
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3, 6], [], [], []], [6, 3, 4, 5, 2, 0, 7, 1], [6, 3, 4, 1, 5, 2, 0, 7], true },
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3, 6], [], [], []], [0, 7, 6, 3, 4, 1, 5, 2], [6, 3, 4, 1, 5, 2, 0, 7], true },
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3], [], [4], []], [], [], true },
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3, 6], [], [], []], [3, 6, 4, 1, 5, 2, 0, 7], [6, 3, 4, 1, 5, 2, 0, 7], false },
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3, 6], [], [], []], [6, 3, 1, 4, 5, 2, 0, 7], [6, 3, 4, 1, 5, 2, 0, 7], false },
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3, 6], [], [], []], [6, 3, 4, 1, 5, 2, 0], [6, 3, 4, 1, 5, 2, 0, 7], false },
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3, 6], [], [], []], [6, 3, 4, 1, 5, 2, 0, 0], [6, 3, 4, 1, 5, 2, 0, 7], false },
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3, 6], [], [], []], [], [6, 3, 4, 1, 5, 2, 0, 7], false },
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3], [], [4], []], [6, 3, 4, 1, 5, 2, 0, 7], [], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForAnOrderKeepingEachGroupTogetherAndEveryItemAfterItsBeforeItems(int itemCount, int declaredGroupCount, int[] group, int[][] beforeItems, int[] answer, int[] recorded, bool expected)
        => Assert.Equal(expected, SortItemsByGroupsRespectingDependenciesAnswerCheck.IsAccepted(itemCount, declaredGroupCount, group, beforeItems, answer, recorded));
}
