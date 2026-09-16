using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RotatedSortedArrayWorkloads (ARCHITECTURE 17.7), shared by the two
// duplicate-tolerant siblings LC 154 and LC 81. The reading depends on the array really being
// 0..length-1 rotated left by the pivot the fixture reports back - that pivot is the only thing
// LC 81's harness knows about where the pre-rotation segment starts - and on a bounded band of
// duplicates stamped across both ends, so ties make real work without swamping the logarithmic win
// over a linear scan.
public sealed partial class RotatedSortedArrayWorkloadsTests
{
    private const int Length = 200;
    private const int PivotDivisor = 3;
    private const int MinimumDuplicateBandWidth = 2;

    // The fixture caps the duplicate band at 40 elements at each end, so anything this far inside
    // the array is untouched by the stamping and must still be the pure rotation.
    private const int DuplicateBandMargin = 40;

    [Fact]
    public void RotatedWithDuplicateBoundaryBand_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(
            Length,
            RotatedSortedArrayWorkloads.RotatedWithDuplicateBoundaryBand(Length).Values.Length);

    [Fact]
    public void RotatedWithDuplicateBoundaryBand_Pivot_ReportsTheRotationTheValuesActuallyCarry()
    {
        var (values, pivot) = RotatedSortedArrayWorkloads.RotatedWithDuplicateBoundaryBand(Length);

        Assert.Equal(Length / PivotDivisor, pivot);

        for (var index = DuplicateBandMargin; index < Length - DuplicateBandMargin; index++)
        {
            Assert.Equal((index + pivot) % Length, values[index]);
        }
    }

    // What makes this the duplicate-tolerant fixture rather than its plain sibling: the same value
    // sits at both ends, so the sorted search meets ties and neither arm may binary-search past
    // them. The band is at least this wide at each end, whatever the fixture's own sizing formula.
    [Fact]
    public void RotatedWithDuplicateBoundaryBand_DuplicateBand_RepeatsTheBoundaryValueAtBothEnds()
    {
        var (values, _) = RotatedSortedArrayWorkloads.RotatedWithDuplicateBoundaryBand(Length);

        Assert.All(values[..MinimumDuplicateBandWidth], value => Assert.Equal(values[0], value));
        Assert.All(values[^MinimumDuplicateBandWidth..], value => Assert.Equal(values[0], value));
        Assert.True(values.Distinct().Count() < Length);
    }
}
