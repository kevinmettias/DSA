using DSAExperimentation.LeetCode.ThreeDivisors;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ThreeDivisors;

// Harness only. Both strategies are ThreeDivisorsSolution's - the sqrt-anchored
// downward walk and the full-range trial division that used to live untested as the
// benchmark's baseline - pinned to LeetCode's published examples plus prime squares
// (the only numbers with exactly three divisors), primes, one, perfect squares of
// composites and a number with nine divisors, which must all report false.
public sealed class ThreeDivisorsTests
{
    public static TheoryData<int, bool> Examples =>
        new()
        {
            { 1, false }, // divisors: 1
            { 2, false }, // divisors: 1, 2
            { 3, false }, // divisors: 1, 3
            { 4, true }, // divisors: 1, 2, 4 (2^2)
            { 8, false }, // divisors: 1, 2, 4, 8
            { 9, true }, // divisors: 1, 3, 9 (3^2)
            { 16, false }, // 4^2 is a square, but 4 is not prime: five divisors
            { 25, true }, // 5^2
            { 49, true }, // 7^2
            { 100, false }, // 2^2 * 5^2 has nine divisors
            { 121, true }, // 11^2
            { 994_009, true }, // 997^2, the anchor lands far from 1
            { 999_983, false }, // prime: two divisors
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsThreeByBinarySearchAnchor_LeetCodeAndSquarePrimeExamples_MatchesDivisorCount(
        int num, bool expected) =>
        Assert.Equal(expected, ThreeDivisorsSolution.IsThreeByBinarySearchAnchor(num));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsThreeByFullRangeScan_LeetCodeAndSquarePrimeExamples_MatchesDivisorCount(
        int num, bool expected) =>
        Assert.Equal(expected, ThreeDivisorsSolution.IsThreeByFullRangeScan(num));
}
