using DSAExperimentation.LeetCode.Solutions.HandOfStraights;

namespace DSAExperimentation.LeetCode.Solutions.Tests.HandOfStraights;

// Harness only. Both greedy strategies are HandOfStraightsSolution's - this file
// just pins them to the smallest hand, a single card split into groups of one.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class HandOfStraightsSolutionTests
{
    public static TheoryData<StraightHandCase> Cases =>
        new()
        {
            { new StraightHandCase(Hand: [1], GroupSize: 1, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsNStraightHandByBclDictionary_Cases_ReturnsWhetherHandSplitsIntoStraights(
        StraightHandCase testCase)
    {
        var actual = HandOfStraightsSolution.IsNStraightHandByBclDictionary(testCase.Hand, testCase.GroupSize);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsNStraightHandByHashMapMergeSort_Cases_ReturnsWhetherHandSplitsIntoStraights(
        StraightHandCase testCase)
    {
        var actual = HandOfStraightsSolution.IsNStraightHandByHashMapMergeSort(testCase.Hand, testCase.GroupSize);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the hand, the size each straight must have, and whether the
    // hand splits into them. The `bool` is the expected answer rather than a mode, so
    // the row names it instead of leaving a bare `true` in a position to be decoded.
    public readonly record struct StraightHandCase(int[] Hand, int GroupSize, bool Expected);
}
