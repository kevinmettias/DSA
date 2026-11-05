using DSAExperimentation.LeetCode.Solutions.ContainsDuplicate;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ContainsDuplicate;

// Harness only: both strategies live in ContainsDuplicateSolution and are asserted
// against the same cases, so a failure names the strategy that broke.
public sealed partial class ContainsDuplicateSolutionTests
{
    public static TheoryData<DuplicateCase> Cases =>
        new()
        {
            { new DuplicateCase([], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasDuplicateByBruteForce_Cases_ReturnsExpected(DuplicateCase testCase) =>
        Assert.Equal(testCase.Expected, ContainsDuplicateSolution.HasDuplicateByBruteForce(testCase.Nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasDuplicateBySetProbe_Cases_ReturnsExpected(DuplicateCase testCase) =>
        Assert.Equal(testCase.Expected, ContainsDuplicateSolution.HasDuplicateBySetProbe(testCase.Nums));

    // One case: the array and whether it holds a repeated value. The
    // expected value is named at every construction site, so a row reads as the case
    // it is rather than as a bare `true` whose meaning is its position. Nested because
    // it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct DuplicateCase(int[] Nums, bool Expected);
}
