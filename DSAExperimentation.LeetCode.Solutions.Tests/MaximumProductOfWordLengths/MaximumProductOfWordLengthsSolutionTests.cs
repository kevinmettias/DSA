using DSAExperimentation.LeetCode.Solutions.MaximumProductOfWordLengths;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumProductOfWordLengths;

// Harness only: both arms are MaximumProductOfWordLengthsSolution's, the same
// methods MaximumProductOfWordLengthsBenchmarks measures.
public sealed partial class MaximumProductOfWordLengthsSolutionTests
{
    public static TheoryData<string[], int> Cases =>
        new()
        {
            { ["a", "b"], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProductByCharacterScan_Cases_ReturnsLargestDisjointProduct(
        string[] words, int expected) =>
        Assert.Equal(expected, MaximumProductOfWordLengthsSolution.MaxProductByCharacterScan(words));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProductByBitmaskHashMap_Cases_ReturnsLargestDisjointProduct(
        string[] words, int expected) =>
        Assert.Equal(expected, MaximumProductOfWordLengthsSolution.MaxProductByBitmaskHashMap(words));
}
