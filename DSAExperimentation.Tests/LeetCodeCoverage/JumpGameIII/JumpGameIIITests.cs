using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameIII;

// LeetCode 1306. Jump Game III: from index i you may jump to i+arr[i] or i-arr[i]
// (whichever stays in bounds); can you ever reach some index holding value 0? This
// is reachability over an implicit bidirectional-hop graph, answered directly by
// this repo's own DepthFirstSearch.Traverse closed over a bare
// Func<int,IEnumerable<int>> successor relation - no bespoke visited-set/stack code,
// no new primitive, unlike JumpGameIITests' weighted ShortestPath.Dijkstra
// composition (that problem needs a minimum hop *count*; this one only needs
// *reachability*, so the unweighted DepthFirstSearch tier is the right fit).
public sealed partial class JumpGameIIITests
{
    [Theory]
    [InlineData(new[] { 4, 2, 3, 0, 3, 1, 2 }, 5, true)]
    [InlineData(new[] { 4, 2, 3, 0, 3, 1, 2 }, 0, true)]
    [InlineData(new[] { 3, 0, 2, 1, 2 }, 2, false)]
    public void CanReach_LeetCodeExamples_ReturnsExpected(int[] arr, int start, bool expected)
        => Assert.Equal(expected, CanReach(arr, start));

    private static bool CanReach(int[] arr, int start)
    {
        var visited = DepthFirstSearch.Traverse(start, index => Successors(arr, index));
        return visited.Exists(index => arr[index] == 0);
    }

    private static IEnumerable<int> Successors(int[] arr, int index)
    {
        var forward = index + arr[index];
        var backward = index - arr[index];

        if (forward < arr.Length)
        {
            yield return forward;
        }

        if (backward >= 0)
        {
            yield return backward;
        }
    }
}
