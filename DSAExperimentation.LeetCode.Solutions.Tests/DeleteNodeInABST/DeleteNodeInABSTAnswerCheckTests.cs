using DSAExperimentation.LeetCode.Solutions.DeleteNodeInABST;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DeleteNodeInABST;

public sealed partial class DeleteNodeInABSTAnswerCheckTests
{
    public static TheoryData<int?[], int, int?[], bool> Answers =>
        new()
        {
            { [5, 3, 6, 2, 4, null, 7], 3, [5, 4, 6, 2, null, null, 7], true },
            { [5, 3, 6, 2, 4, null, 7], 3, [5, 2, 6, null, 4, null, 7], true },
            { [5, 3, 6, 2, 4, null, 7], 0, [5, 3, 6, 2, 4, null, 7], true },
            { [5, 3, 6, 2, 4, null, 7], 3, [5, 3, 6, 2, 4, null, 7], false },
            { [5, 3, 6, 2, 4, null, 7], 3, [5, 6, 4, 2, null, null, 7], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForASearchTreeOfTheRemainingValues(int?[] tree, int key, int?[] answer, bool expected)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(tree);
        var answered = LeetCodeWireFormat.ToBinaryTree(answer);

        Assert.Equal(expected, DeleteNodeInABSTAnswerCheck.IsAccepted(root, key, answered, answered));
    }
}
