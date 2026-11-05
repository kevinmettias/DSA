using DSAExperimentation.LeetCode.Solutions.ValidateStackSequences;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidateStackSequences;

// Harness only. Both the exhaustive backtracking search and the greedy Stack<int>
// sweep are ValidateStackSequencesSolution's; this file pins them to the shapes that
// separate "greedy is safe" from "greedy is lucky" - pop-immediately,
// pop-everything-at-the-end, and a pop order that is unreachable only because of
// what was buried beneath the first match. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class ValidateStackSequencesSolutionTests
{
    public static TheoryData<PushPopCase> Cases =>
        new()
        {
            { new PushPopCase(Pushed: [1, 2], Popped: [1, 2], Expected: true) },
            { new PushPopCase(Pushed: [1, 2], Popped: [2, 1], Expected: true) },
            { new PushPopCase(Pushed: [1, 2, 3], Popped: [3, 1, 2], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidByBacktrackingSearch_Cases_ReportsWhetherSomeInterleavingProducesThePopOrder(
        PushPopCase testCase)
    {
        var actual = ValidateStackSequencesSolution.IsValidByBacktrackingSearch(testCase.Pushed, testCase.Popped);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidByGreedyStackSweep_Cases_ReportsWhetherSomeInterleavingProducesThePopOrder(
        PushPopCase testCase)
    {
        var actual = ValidateStackSequencesSolution.IsValidByGreedyStackSweep(testCase.Pushed, testCase.Popped);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the pushed order, the popped order, and whether some
    // interleaving of pushes and pops produces that pop order. The two arrays are the
    // same type and the relation is not symmetric, so the row names which is which
    // rather than leaving two interchangeable positions. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct PushPopCase(int[] Pushed, int[] Popped, bool Expected);
}
