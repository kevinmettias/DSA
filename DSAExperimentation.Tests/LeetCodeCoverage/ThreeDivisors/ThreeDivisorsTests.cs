using DSAExperimentation.LeetCode.ThreeDivisors;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ThreeDivisors;

// Harness only. Both strategies are ThreeDivisorsSolution's - the sqrt-anchored
// downward walk and the full-range trial division that used to live untested as the
// benchmark's baseline - pinned to LeetCode's published examples plus prime squares
// (the only numbers with exactly three divisors), primes, one, perfect squares of
// composites and a number with nine divisors, which must all report false.
public sealed class ThreeDivisorsTests
{
    public static TheoryData<DivisorCountExample> Examples =>
        new()
        {
            new DivisorCountExample(1, HasExactlyThreeDivisors: false), // divisors: 1
            new DivisorCountExample(2, HasExactlyThreeDivisors: false), // divisors: 1, 2
            new DivisorCountExample(3, HasExactlyThreeDivisors: false), // divisors: 1, 3
            new DivisorCountExample(4, HasExactlyThreeDivisors: true), // divisors: 1, 2, 4 (2^2)
            new DivisorCountExample(8, HasExactlyThreeDivisors: false), // divisors: 1, 2, 4, 8
            new DivisorCountExample(9, HasExactlyThreeDivisors: true), // divisors: 1, 3, 9 (3^2)
            new DivisorCountExample(16, HasExactlyThreeDivisors: false), // 4^2 is square, but 4 is not prime: five divisors
            new DivisorCountExample(25, HasExactlyThreeDivisors: true), // 5^2
            new DivisorCountExample(49, HasExactlyThreeDivisors: true), // 7^2
            new DivisorCountExample(100, HasExactlyThreeDivisors: false), // 2^2 * 5^2 has nine divisors
            new DivisorCountExample(121, HasExactlyThreeDivisors: true), // 11^2
            new DivisorCountExample(994_009, HasExactlyThreeDivisors: true), // 997^2, the anchor lands far from 1
            new DivisorCountExample(999_983, HasExactlyThreeDivisors: false), // prime: two divisors
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsThreeByBinarySearchAnchor_LeetCodeAndSquarePrimeExamples_MatchesDivisorCount(
        DivisorCountExample example) =>
        Assert.Equal(example.HasExactlyThreeDivisors, ThreeDivisorsSolution.IsThreeByBinarySearchAnchor(example.Num));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsThreeByFullRangeScan_LeetCodeAndSquarePrimeExamples_MatchesDivisorCount(
        DivisorCountExample example) =>
        Assert.Equal(example.HasExactlyThreeDivisors, ThreeDivisorsSolution.IsThreeByFullRangeScan(example.Num));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct DivisorCountExample(int Num, bool HasExactlyThreeDivisors);
}
