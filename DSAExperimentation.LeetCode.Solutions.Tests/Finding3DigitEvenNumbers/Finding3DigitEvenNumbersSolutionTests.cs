using DSAExperimentation.LeetCode.Solutions.Finding3DigitEvenNumbers;

namespace DSAExperimentation.LeetCode.Solutions.Tests.Finding3DigitEvenNumbers;

// Harness only. Both strategies live in Finding3DigitEvenNumbersSolution - the
// List.Contains dedupe baseline that used to exist only as an unasserted benchmark
// arm, and the Set<int> + MergeSort composition - and this file pins both to
// the degenerate digit sets.
public sealed partial class Finding3DigitEvenNumbersSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            // Exactly one even digit, which must therefore take the ones place.
            { [1, 2, 3], [132, 312] },

            // All three positions carry the same even digit.
            { [6, 6, 6], [666] },

            // Fewer than three digits: no triple of distinct positions exists.
            { [2, 4], [] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindEvenNumbersByListScanDedupe_Cases_ReturnsSortedDistinctEvenNumbers(
        int[] digits, int[] expected) =>
        Assert.Equal(expected, Finding3DigitEvenNumbersSolution.FindEvenNumbersByListScanDedupe(digits));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindEvenNumbersBySetDedupe_Cases_ReturnsSortedDistinctEvenNumbers(
        int[] digits, int[] expected) =>
        Assert.Equal(expected, Finding3DigitEvenNumbersSolution.FindEvenNumbersBySetDedupe(digits));
}
