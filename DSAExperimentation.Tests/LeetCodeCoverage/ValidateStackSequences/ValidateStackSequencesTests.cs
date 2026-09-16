using DSAExperimentation.LeetCode.ValidateStackSequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidateStackSequences;

// Harness only. Both the exhaustive backtracking search and the greedy Stack<int>
// sweep are ValidateStackSequencesSolution's; this file pins them to LeetCode's two
// published examples plus the shapes that separate "greedy is safe" from "greedy is
// lucky" - pop-immediately, pop-everything-at-the-end, and a pop order that is
// unreachable only because of what was buried beneath the first match.
public sealed partial class ValidateStackSequencesTests
{
    public static TheoryData<PushPopCase> Examples =>
        new()
        {
            { new PushPopCase(Pushed: [1, 2, 3, 4, 5], Popped: [4, 5, 3, 2, 1], Expected: true) },
            { new PushPopCase(Pushed: [1, 2, 3, 4, 5], Popped: [4, 3, 5, 1, 2], Expected: false) },
            { new PushPopCase(Pushed: [1], Popped: [1], Expected: true) },
            { new PushPopCase(Pushed: [1, 2], Popped: [1, 2], Expected: true) },
            { new PushPopCase(Pushed: [1, 2], Popped: [2, 1], Expected: true) },
            { new PushPopCase(Pushed: [2, 1, 0], Popped: [0, 1, 2], Expected: true) },
            { new PushPopCase(Pushed: [1, 2, 3], Popped: [3, 1, 2], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByBacktrackingSearch_LeetCodeExamples_ReportsWhetherSomeInterleavingProducesThePopOrder(
        PushPopCase example)
    {
        var actual = ValidateStackSequencesSolution.IsValidByBacktrackingSearch(example.Pushed, example.Popped);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByGreedyStackSweep_LeetCodeExamples_ReportsWhetherSomeInterleavingProducesThePopOrder(
        PushPopCase example)
    {
        var actual = ValidateStackSequencesSolution.IsValidByGreedyStackSweep(example.Pushed, example.Popped);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the pushed order, the popped order, and whether some
    // interleaving of pushes and pops produces that pop order. The two arrays are the
    // same type and the relation is not symmetric, so the row names which is which
    // rather than leaving two interchangeable positions. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct PushPopCase(int[] Pushed, int[] Popped, bool Expected);
}
