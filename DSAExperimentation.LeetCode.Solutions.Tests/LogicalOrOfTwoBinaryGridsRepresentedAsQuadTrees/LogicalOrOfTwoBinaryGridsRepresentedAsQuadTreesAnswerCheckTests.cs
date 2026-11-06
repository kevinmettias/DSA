using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Solutions.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees;

public sealed partial class LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesAnswerCheckTests
{
    private static readonly QuadTreeNode Leaf = new(true, true);

    public static TheoryData<int[]?[], bool> Answers =>
        new()
        {
            { [[0, 0], [1, 1], [1, 1], [1, 1], [1, 0]], true },
            { [[0, 1], [1, 1], [1, 1], [1, 1], [1, 0]], true },
            { [[0, 0], [1, 1], [1, 1], [1, 1], [1, 1]], false },
            { [[1, 1]], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForTheRecordedTreeWhateverItsInnerValues(int[]?[] answer, bool expected)
    {
        var answered = Assert.IsType<QuadTreeNode>(LeetCodeQuadTrees.From(answer));
        var recorded = Assert.IsType<QuadTreeNode>(LeetCodeQuadTrees.From([[0, 0], [1, 1], [1, 1], [1, 1], [1, 0]]));

        Assert.Equal(expected, LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesAnswerCheck.IsAccepted(Leaf, Leaf, answered, recorded));
    }
}
