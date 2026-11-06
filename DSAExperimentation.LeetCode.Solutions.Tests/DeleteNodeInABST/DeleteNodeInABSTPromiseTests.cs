using DSAExperimentation.LeetCode.Solutions.DeleteNodeInABST;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DeleteNodeInABST;

public sealed partial class DeleteNodeInABSTPromiseTests
{
    public static TheoryData<int?[], bool> Trees =>
        new()
        {
            { [5, 3, 6, 2, 4, null, 7], true },
            { [], true },
            { [5, 3, 6, 2, 7], false },
            { [2, 2, 3], false },
        };

    [Theory]
    [MemberData(nameof(Trees))]
    public void IsKeptBy_Tree_HoldsForASearchTreeOfUniqueValues(int?[] tree, bool expected)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(tree);

        Assert.Equal(expected, DeleteNodeInABSTPromise.IsKeptBy(root, 0));
    }
}
