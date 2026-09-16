using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for KthSmallestInstructionsWorkloads (ARCHITECTURE 17.7). Both choices here drive
// the LC 1643 reading: the destination every route ends at, and the median rank that keeps the
// greedy walk making a real decision at nearly every step.
public sealed partial class KthSmallestInstructionsWorkloadsTests
{
    private const int Size = 10;
    private const int DestinationFieldCount = 2; // VerticalSteps, HorizontalSteps
    private const int TotalStepsMultiplier = 2; // mirrors KthSmallestInstructionsWorkloads.TotalStepsMultiplier
    private const int SmallestSize = 1;
    private const int MedianSequenceDivisor = 2;
    private const long SmallestSizeMedianRank = 1; // C(2, 1) / 2
    private const long SquareOfSizeTenMedianRank = 92_378; // C(20, 10) / 2

    [Fact]
    public void SquareDestination_Size_ReturnsASquareDestinationAtThatSize()
    {
        var destination = KthSmallestInstructionsWorkloads.SquareDestination(Size);

        Assert.Equal(DestinationFieldCount, destination.Length);
        Assert.Equal(Size, destination[0]);
        Assert.Equal(Size, destination[1]);
    }

    [Fact]
    public void MedianRank_Size_ReturnsHalfOfTheKnownCentralBinomialCoefficient()
    {
        Assert.Equal(SmallestSizeMedianRank, KthSmallestInstructionsWorkloads.MedianRank(SmallestSize));
        Assert.Equal(SquareOfSizeTenMedianRank, KthSmallestInstructionsWorkloads.MedianRank(Size));
    }

    // Recomputing the route count independently - by building Pascal's triangle rather than by the
    // fixture's own multiplicative formula - is what makes this a check of the rank instead of a
    // restatement of it.
    [Fact]
    public void MedianRank_EverySize_ReturnsHalfOfAnIndependentlyCountedRouteTotal()
    {
        foreach (var size in Enumerable.Range(SmallestSize, Size))
        {
            Assert.Equal(
                CentralBinomialCoefficient(size) / MedianSequenceDivisor,
                KthSmallestInstructionsWorkloads.MedianRank(size));
        }
    }

    private static long CentralBinomialCoefficient(int size)
    {
        var row = new long[(TotalStepsMultiplier * size) + 1];
        row[0] = 1;

        for (var step = 0; step < TotalStepsMultiplier * size; step++)
        {
            for (var column = step + 1; column > 0; column--)
            {
                row[column] += row[column - 1];
            }
        }

        return row[size];
    }
}
