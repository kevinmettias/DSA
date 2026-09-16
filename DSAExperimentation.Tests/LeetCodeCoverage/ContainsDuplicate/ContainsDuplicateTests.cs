using DSAExperimentation.LeetCode.ContainsDuplicate;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContainsDuplicate;

// Harness only: both strategies live in ContainsDuplicateSolution and are asserted
// against the same examples, so a failure names the strategy that broke.
public sealed partial class ContainsDuplicateTests
{
    public static TheoryData<DuplicateCase> Examples =>
        new()
        {
            { new DuplicateCase([1, 2, 3, 1], Expected: true) },
            { new DuplicateCase([1, 2, 3, 4], Expected: false) },
            { new DuplicateCase([1, 1, 1, 3, 3, 4, 3, 2, 4, 2], Expected: true) },
            { new DuplicateCase([], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasDuplicateByBruteForce_LeetCodeExamples_ReturnsExpected(DuplicateCase example) =>
        Assert.Equal(example.Expected, ContainsDuplicateSolution.HasDuplicateByBruteForce(example.Nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasDuplicateBySetProbe_LeetCodeExamples_ReturnsExpected(DuplicateCase example) =>
        Assert.Equal(example.Expected, ContainsDuplicateSolution.HasDuplicateBySetProbe(example.Nums));

    // One LeetCode example: the array and whether it holds a repeated value. The
    // expected value is named at every construction site, so a row reads as the case
    // it is rather than as a bare `true` whose meaning is its position. Nested because
    // it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct DuplicateCase(int[] Nums, bool Expected);
}
