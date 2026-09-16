using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for WiggleSortIIWorkloads (ARCHITECTURE 17.7): LC 324's whole difficulty is duplicates
// around the median, which is what makes a naive ascending interleave invalid and the reversed-half
// interleave necessary. The fixture gets that for free by bounding values to length / divisor, so more
// values exist than slots and duplicates are forced; the test asserts the forcing rather than the draw.
public sealed partial class WiggleSortIIWorkloadsTests
{
    private const int Length = 200;
    private const int Seed = 324; // LC problem number
    private const int ValueRangeDivisor = 2;
    private const int SmallestValue = 0;
    private const int FewestRepeatedValues = 1;

    [Fact]
    public void BuildValuesWithDuplicates_Length_ReturnsThatManyValuesInsideTheNarrowedRange()
    {
        var values = WiggleSortIIWorkloads.BuildValuesWithDuplicates(Length, Seed, ValueRangeDivisor);

        Assert.Equal(Length, values.Length);
        Assert.All(values, value => Assert.InRange(value, SmallestValue, ValueRange - 1));
    }

    // More values than the range has slots is exactly what forces duplicates: by pigeonhole at least one
    // value must repeat, which is the case that separates the two interleaves.
    [Fact]
    public void BuildValuesWithDuplicates_NarrowRange_ForcesAtLeastOneDuplicate() =>
        Assert.True(
            WiggleSortIIWorkloads.BuildValuesWithDuplicates(Length, Seed, ValueRangeDivisor).Distinct().Count()
                <= Length - FewestRepeatedValues);

    [Fact]
    public void BuildValuesWithDuplicates_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            WiggleSortIIWorkloads.BuildValuesWithDuplicates(Length, Seed, ValueRangeDivisor),
            WiggleSortIIWorkloads.BuildValuesWithDuplicates(Length, Seed, ValueRangeDivisor));

    private static int ValueRange => Length / ValueRangeDivisor;
}
