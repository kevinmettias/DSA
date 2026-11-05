using DSAExperimentation.LeetCode.Solutions.JumpGameIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.JumpGameIV;

// Harness only: both strategies live in JumpGameIVSolution - the textbook
// level-order BFS with same-value group pruning, and modeling the same
// i+1/i-1/same-value reachability as an implicit unweighted-hop graph answered
// with this repo's own BreadthFirstDistances. That hop graph is asserted on its own,
// index by index.
public sealed partial class JumpGameIVSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [6, 1, 9], 2 },
            { [1, 2, 3, 4, 5], 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinJumpsByBfsWithGroupPruning_Cases_ReturnsMinimumStepCount(int[] arr, int expected) =>
        Assert.Equal(expected, JumpGameIVSolution.MinJumpsByBfsWithGroupPruning(arr));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinJumpsByBreadthFirstDistances_Cases_ReturnsMinimumStepCount(int[] arr, int expected) =>
        Assert.Equal(expected, JumpGameIVSolution.MinJumpsByBreadthFirstDistances(arr));

    // LeetCode's third example, 7 6 9 6 9 6 9 7: 7 sits at 0 and 7, 6 at 1, 3 and 5, 9 at
    // 2, 4 and 6. Each index hops to i + 1, then i - 1, then every other index holding its
    // value in ascending order - so 0 -> 1, 7 and 7 -> 6, 0 at the ends, and 3 -> 4, 2, 1, 5
    // in the middle.
    [Fact]
    public void BuildHopGraph_LeetCodeThirdExample_WiresNeighboursThenEveryEqualValue()
    {
        var nodes = JumpGameIVSolution.BuildHopGraph([7, 6, 9, 6, 9, 6, 9, 7]);
        var ids = nodes.Select(node => node.Id);
        var targets = nodes.Select(node => node.Neighbors.Select(target => target.Id).ToArray());

        Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7], ids);
        Assert.Equal(
            [[1, 7], [2, 0, 3, 5], [3, 1, 4, 6], [4, 2, 1, 5], [5, 3, 2, 6], [6, 4, 1, 3], [7, 5, 2, 4], [6, 0]],
            targets);
    }
}
