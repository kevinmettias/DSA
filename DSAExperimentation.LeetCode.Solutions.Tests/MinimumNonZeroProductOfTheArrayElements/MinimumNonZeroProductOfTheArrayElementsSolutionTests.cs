using DSAExperimentation.LeetCode.Solutions.MinimumNonZeroProductOfTheArrayElements;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNonZeroProductOfTheArrayElements;

// Harness only: both strategies live in
// MinimumNonZeroProductOfTheArrayElementsSolution and are asserted against the same
// cases - p = 4, 5, 10 and 15, whose answers are the modular product written out by
// hand, so the squaring arm cannot agree with the naive arm on a wrong pairing. p
// stays small enough that the O(exponent) baseline can run every case too, which is
// the whole point of it being a first-class strategy. LeetCode's published examples
// run in CatalogCaseTests.
public sealed partial class MinimumNonZeroProductOfTheArrayElementsSolutionTests
{
    public static TheoryData<int, long> Cases =>
        new()
        {
            { 4, 581202553L }, // 14^7 * 15 = 1,581,202,560, folded under 1e9+7
            { 5, 202795991L }, // 30^15 * 31 mod 1e9+7
            { 10, 586669277L }, // 1022^511 * 1023 mod 1e9+7
            { 15, 431467833L }, // 32766^16383 * 32767 mod 1e9+7
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinNonZeroProductByRepeatedMultiplication_Cases_ReturnsMinimalProduct(
        int power, long expected)
        => Assert.Equal(
            expected, MinimumNonZeroProductOfTheArrayElementsSolution.MinNonZeroProductByRepeatedMultiplication(power));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinNonZeroProductBySquaring_Cases_ReturnsMinimalProduct(int power, long expected)
        => Assert.Equal(expected, MinimumNonZeroProductOfTheArrayElementsSolution.MinNonZeroProductBySquaring(power));
}
