using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for BinarySearchAlgorithmWorkloads (ARCHITECTURE 17.7). The reading depends on
// two coupled decisions: the array is sorted and evenly spaced, and the target is its LAST element,
// which is what forces the linear-scan arm through its full worst-case pass instead of finding it
// near the start and looking artificially competitive.
public sealed partial class BinarySearchAlgorithmWorkloadsTests
{
    private const int Length = 64;
    private const int ValueStride = 2;

    [Fact]
    public void BuildSortedValues_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, BinarySearchAlgorithmWorkloads.BuildSortedValues(Length).Length);

    [Fact]
    public void BuildSortedValues_EveryValue_IsTheStridedValueForItsPosition()
    {
        var values = BinarySearchAlgorithmWorkloads.BuildSortedValues(Length);

        for (var index = 0; index < Length; index++)
        {
            Assert.Equal(index * ValueStride, values[index]);
        }
    }

    [Fact]
    public void FarthestTarget_Length_ReturnsTheLastValueOfTheSameBuild() =>
        Assert.Equal(
            BinarySearchAlgorithmWorkloads.BuildSortedValues(Length)[^1],
            BinarySearchAlgorithmWorkloads.FarthestTarget(Length));

    [Fact]
    public void FarthestTarget_Length_ReturnsAValueTheBuiltArrayContains() =>
        Assert.Contains(
            BinarySearchAlgorithmWorkloads.FarthestTarget(Length),
            BinarySearchAlgorithmWorkloads.BuildSortedValues(Length));

    [Fact]
    public void BuildSortedValues_SameLength_ReturnsTheSameValues() =>
        Assert.Equal(
            BinarySearchAlgorithmWorkloads.BuildSortedValues(Length),
            BinarySearchAlgorithmWorkloads.BuildSortedValues(Length));
}
