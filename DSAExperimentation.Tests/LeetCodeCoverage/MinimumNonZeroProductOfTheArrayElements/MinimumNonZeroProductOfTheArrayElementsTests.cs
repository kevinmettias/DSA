using DSAExperimentation.LeetCode.MinimumNonZeroProductOfTheArrayElements;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNonZeroProductOfTheArrayElements;

// Harness only: both strategies live in
// MinimumNonZeroProductOfTheArrayElementsSolution and are asserted against the same
// examples - LeetCode's own three, plus larger p whose answers are the modular
// product written out by hand, so the squaring arm cannot agree with the naive arm
// on a wrong pairing. p stays small enough that the O(exponent) baseline can run
// every case too, which is the whole point of it being a first-class strategy.
public sealed class MinimumNonZeroProductOfTheArrayElementsTests
{
    public static TheoryData<int, long> Examples =>
        new()
        {
            { 1, 1L }, // [1], nothing to swap
            { 2, 6L }, // 1 * 2 * 3
            { 3, 1512L }, // 1 * 6 * 6 * 7
            { 4, 581202553L }, // 14^7 * 15 = 1,581,202,560, folded under 1e9+7
            { 5, 202795991L }, // 30^15 * 31 mod 1e9+7
            { 10, 586669277L }, // 1022^511 * 1023 mod 1e9+7
            { 15, 431467833L }, // 32766^16383 * 32767 mod 1e9+7
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinNonZeroProductByRepeatedMultiplication_LeetCodeExamples_ReturnsMinimalProduct(
        int power, long expected)
        => Assert.Equal(
            expected, MinimumNonZeroProductOfTheArrayElementsSolution.MinNonZeroProductByRepeatedMultiplication(power));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinNonZeroProductBySquaring_LeetCodeExamples_ReturnsMinimalProduct(int power, long expected)
        => Assert.Equal(expected, MinimumNonZeroProductOfTheArrayElementsSolution.MinNonZeroProductBySquaring(power));
}
