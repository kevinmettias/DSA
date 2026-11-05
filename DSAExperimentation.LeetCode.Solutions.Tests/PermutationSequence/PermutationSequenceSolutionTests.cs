using DSAExperimentation.LeetCode.Solutions.PermutationSequence;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PermutationSequence;

// Harness only: both strategies live in PermutationSequenceSolution and are
// asserted against the same cases, so a failure names the strategy that broke.
public sealed partial class PermutationSequenceSolutionTests
{
    public static TheoryData<int, int, string> Cases =>
        new()
        {
            { 1, 1, "1" },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetPermutationByBacktrackEnumeration_Cases_ReturnsKthPermutation(
        int digitCount, int rank, string expected)
    {
        var actual = PermutationSequenceSolution.GetPermutationByBacktrackEnumeration(digitCount, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetPermutationByFactoradicSelection_Cases_ReturnsKthPermutation(
        int digitCount, int rank, string expected)
    {
        var actual = PermutationSequenceSolution.GetPermutationByFactoradicSelection(digitCount, rank);

        Assert.Equal(expected, actual);
    }
}
