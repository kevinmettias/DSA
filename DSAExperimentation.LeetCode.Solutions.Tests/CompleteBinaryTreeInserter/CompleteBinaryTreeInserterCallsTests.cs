using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Solutions.CompleteBinaryTreeInserter;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CompleteBinaryTreeInserter;

public sealed partial class CompleteBinaryTreeInserterCallsTests
{
    [Fact]
    public void GetRoot_AfterAnInsert_IsTheTreeItWasGiven()
    {
        var root = new BinaryTreeNode<int>(1);
        var inserter = CompleteBinaryTreeInserterSolution.CreateByIncompleteQueue(root);
        inserter.Insert(2);

        Assert.Same(root, CompleteBinaryTreeInserterCalls.GetRoot(inserter));
    }
}
