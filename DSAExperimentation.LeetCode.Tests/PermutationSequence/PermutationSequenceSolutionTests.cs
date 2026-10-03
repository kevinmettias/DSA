using DSAExperimentation.LeetCode.PermutationSequence;

namespace DSAExperimentation.LeetCode.Tests.PermutationSequence;

// Harness only: both strategies live in PermutationSequenceSolution and are
// asserted against the same examples, so a failure names the strategy that broke.
public sealed partial class PermutationSequenceSolutionTests
{
    public static TheoryData<int, int, string> Examples =>
        new()
        {
            { 3, 3, "213" },
            { 4, 9, "2314" },
            { 3, 1, "123" },
            { 1, 1, "1" },
            { 4, 24, "4321" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetPermutationByBacktrackEnumeration_LeetCodeExamples_ReturnsKthPermutation(
        int digitCount, int rank, string expected)
    {
        var actual = PermutationSequenceSolution.GetPermutationByBacktrackEnumeration(digitCount, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetPermutationByFactoradicSelection_LeetCodeExamples_ReturnsKthPermutation(
        int digitCount, int rank, string expected)
    {
        var actual = PermutationSequenceSolution.GetPermutationByFactoradicSelection(digitCount, rank);

        Assert.Equal(expected, actual);
    }
}
