using DSAExperimentation.LeetCode.CountTheNumberOfComputerUnlockingPermutations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheNumberOfComputerUnlockingPermutations;

// Harness only. Both strategies are
// CountTheNumberOfComputerUnlockingPermutationsSolution's - this file just pins them
// to LeetCode's published examples, including the six-computer case where every
// index-1 computer is stuck behind an equal-complexity computer 0 and no unlock order
// can ever reach it.
public sealed class CountTheNumberOfComputerUnlockingPermutationsTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [1, 2, 3], 2 },
            { [3, 3, 3, 4, 4, 4], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountUnlockingPermutationsByBacktracking_LeetCodeExamples_ReturnsLegalOrderCount(
        int[] complexity, long expected) =>
        Assert.Equal(
            expected,
            CountTheNumberOfComputerUnlockingPermutationsSolution.CountUnlockingPermutationsByBacktracking(complexity));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountUnlockingPermutationsByFactorialFormula_LeetCodeExamples_ReturnsLegalOrderCount(
        int[] complexity, long expected) =>
        Assert.Equal(
            expected,
            CountTheNumberOfComputerUnlockingPermutationsSolution.CountUnlockingPermutationsByFactorialFormula(complexity));
}
