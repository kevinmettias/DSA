using DSAExperimentation.LeetCode.ValidateStackSequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidateStackSequences;

// Harness only. Both the exhaustive backtracking search and the greedy Stack<int>
// sweep are ValidateStackSequencesSolution's; this file pins them to LeetCode's two
// published examples plus the shapes that separate "greedy is safe" from "greedy is
// lucky" - pop-immediately, pop-everything-at-the-end, and a pop order that is
// unreachable only because of what was buried beneath the first match.
public sealed class ValidateStackSequencesTests
{
    public static TheoryData<int[], int[], bool> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], [4, 5, 3, 2, 1], true },
            { [1, 2, 3, 4, 5], [4, 3, 5, 1, 2], false },
            { [1], [1], true },
            { [1, 2], [1, 2], true },
            { [1, 2], [2, 1], true },
            { [2, 1, 0], [0, 1, 2], true },
            { [1, 2, 3], [3, 1, 2], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByBacktrackingSearch_LeetCodeExamples_ReportsWhetherSomeInterleavingProducesThePopOrder(
        int[] pushed, int[] popped, bool expected) =>
        Assert.Equal(expected, ValidateStackSequencesSolution.IsValidByBacktrackingSearch(pushed, popped));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByGreedyStackSweep_LeetCodeExamples_ReportsWhetherSomeInterleavingProducesThePopOrder(
        int[] pushed, int[] popped, bool expected) =>
        Assert.Equal(expected, ValidateStackSequencesSolution.IsValidByGreedyStackSweep(pushed, popped));
}
