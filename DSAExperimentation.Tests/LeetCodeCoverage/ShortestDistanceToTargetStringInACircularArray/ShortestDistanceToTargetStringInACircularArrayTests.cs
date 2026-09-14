using DSAExperimentation.LeetCode.ShortestDistanceToTargetStringInACircularArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestDistanceToTargetStringInACircularArray;

// Harness only. Both strategies live in
// ShortestDistanceToTargetStringInACircularArraySolution - including the
// min(diff, n - diff) linear scan, which used to exist as an unasserted benchmark
// baseline - and this file pins them to the same examples so a failure names the
// strategy that broke.
public sealed class ShortestDistanceToTargetStringInACircularArrayTests
{
    public static TheoryData<string[], string, int, int> Examples =>
        new()
        {
            // LeetCode example 1: "hello" sits one step left of startIndex 1 and
            // two steps right of it.
            { ["hello", "i", "am", "leetcode", "hello"], "hello", 1, 1 },

            // LeetCode example 2: the nearer "leetcode" is reached by wrapping
            // backwards off index 0.
            { ["i", "am", "leetcode", "leetcode"], "leetcode", 0, 1 },

            // LeetCode example 3: the target is absent entirely.
            { ["i", "eat", "leetcode"], "ate", 0, -1 },

            // startIndex already holds the target, so no step is needed.
            { ["a", "b", "c"], "a", 0, 0 },

            // A single-element circle, whose two edges are both self-loops.
            { ["a"], "a", 0, 0 },

            // Two elements: either direction reaches the other index in one step.
            { ["a", "b"], "b", 0, 1 },

            // Only the wraparound is short - walking forwards would cost three.
            { ["a", "b", "c", "target"], "target", 0, 1 },

            // Two matches equidistant in opposite directions from startIndex 0.
            { ["a", "t", "b", "b", "t"], "t", 0, 1 },

            // The benchmark's own workload shape: one match diametrically
            // opposite the start, so neither direction is shorter.
            { ["s", "f", "f", "t", "f", "f"], "t", 0, 3 },

            // A later start index whose nearest match is behind it.
            { ["t", "a", "b", "c", "d"], "t", 3, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestTargetByLinearScan_LeetCodeExamples_ReturnsShortestCircularDistanceOrNegativeOne(
        string[] words, string target, int startIndex, int expected) =>
        Assert.Equal(
            expected,
            ShortestDistanceToTargetStringInACircularArraySolution.ClosestTargetByLinearScan(
                words, target, startIndex));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestTargetByReduceGraph_LeetCodeExamples_ReturnsShortestCircularDistanceOrNegativeOne(
        string[] words, string target, int startIndex, int expected) =>
        Assert.Equal(
            expected,
            ShortestDistanceToTargetStringInACircularArraySolution.ClosestTargetByReduceGraph(
                words, target, startIndex));
}
