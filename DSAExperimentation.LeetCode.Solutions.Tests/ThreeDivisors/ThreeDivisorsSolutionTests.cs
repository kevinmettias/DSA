using DSAExperimentation.LeetCode.Solutions.ThreeDivisors;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ThreeDivisors;

// Harness only. Both strategies are ThreeDivisorsSolution's - the sqrt-anchored
// downward walk and the full-range trial division that used to live untested as the
// benchmark's baseline - pinned to a prime square far from 1 (prime squares are the
// only numbers with exactly three divisors), and to a large prime and a prime cube
// with four divisors, which must both report false. LeetCode's published examples
// run in CatalogCaseTests.
public sealed partial class ThreeDivisorsSolutionTests
{
    public static TheoryData<DivisorCountCase> Cases =>
        new()
        {
            new DivisorCountCase(8, HasExactlyThreeDivisors: false), // divisors: 1, 2, 4, 8
            new DivisorCountCase(994_009, HasExactlyThreeDivisors: true), // 997^2, the anchor lands far from 1
            new DivisorCountCase(999_983, HasExactlyThreeDivisors: false), // prime: two divisors
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsThreeByBinarySearchAnchor_SquarePrimes_MatchesDivisorCount(
        DivisorCountCase testCase) =>
        Assert.Equal(testCase.HasExactlyThreeDivisors, ThreeDivisorsSolution.IsThreeByBinarySearchAnchor(testCase.Num));

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsThreeByFullRangeScan_SquarePrimes_MatchesDivisorCount(
        DivisorCountCase testCase) =>
        Assert.Equal(testCase.HasExactlyThreeDivisors, ThreeDivisorsSolution.IsThreeByFullRangeScan(testCase.Num));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct DivisorCountCase(int Num, bool HasExactlyThreeDivisors);
}
