using DSAExperimentation.LeetCode.Solutions.InsertIntoABinarySearchTree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.InsertIntoABinarySearchTree;

public sealed partial class InsertIntoABinarySearchTreeAnswerCheckTests
{
    public static TheoryData<int?[], int, int?[], bool> Answers =>
        new()
        {
            { [4, 2, 7, 1, 3], 5, [4, 2, 7, 1, 3, 5], true },
            { [4, 2, 7, 1, 3], 5, [3, 2, 5, 1, null, 4, 7], true },
            { [4, 2, 7, 1, 3], 5, [4, 2, 7, 1, 3], false },
            { [4, 2, 7, 1, 3], 5, [4, 2, 7, 1, 3, null, 5], false },
            { [], 5, [5], true },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForASearchTreeOfTheOldValuesAndTheNewOne(int?[] tree, int newValue, int?[] answer, bool expected)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(tree);
        var answered = LeetCodeWireFormat.ToBinaryTree(answer);

        Assert.Equal(expected, InsertIntoABinarySearchTreeAnswerCheck.IsAccepted(root, newValue, answered, answered));
    }
}
