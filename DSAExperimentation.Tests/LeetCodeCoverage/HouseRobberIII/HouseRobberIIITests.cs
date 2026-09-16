using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.HouseRobberIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobberIII;

// Harness only. Both strategies are HouseRobberIIISolution's - this file pins them
// to LeetCode's published examples, given in LeetCode's own level-order-with-null
// array shape (BinaryTreeNode<int> is internal, so it cannot appear in a public
// TheoryData signature; LeetCodeWireFormat.ToBinaryTree reconstructs it). RobFoldAlgebra (the catamorphism
// this repo's generic TreeFold engine closes over) lives beside the solution and is
// exercised only through RobByTreeFoldAlgebra here, per the same precedent as
// CountWaysToBuildRoomsInAnAntColony's RoomWaysAlgebra.
public sealed class HouseRobberIIITests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            { [3, 2, 3, null, 3, null, 1], 7 },
            { [3, 4, 5, 1, 3, null, 1], 9 },
            { [5], 5 },
            { [1, 2, 3], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobByRecursivePair_LeetCodeExamples_ReturnsBestNonAdjacentSum(int?[] values, int expected) =>
        Assert.Equal(expected, HouseRobberIIISolution.RobByRecursivePair(LeetCodeWireFormat.ToBinaryTree(values)!));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobByTreeFoldAlgebra_LeetCodeExamples_ReturnsBestNonAdjacentSum(int?[] values, int expected) =>
        Assert.Equal(expected, HouseRobberIIISolution.RobByTreeFoldAlgebra(LeetCodeWireFormat.ToBinaryTree(values)!));
}
