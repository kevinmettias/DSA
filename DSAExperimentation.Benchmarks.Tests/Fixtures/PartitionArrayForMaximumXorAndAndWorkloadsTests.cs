using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PartitionArrayForMaximumXorAndAndWorkloads (ARCHITECTURE 17.7). The reading
// depends on LC 3630's array being deterministic random values inside a range narrower than a signed
// 32-bit integer, so both the AND and the XOR walks see real spread instead of one saturated value.
public sealed partial class PartitionArrayForMaximumXorAndAndWorkloadsTests
{
    private const int ElementCount = 8;
    private const int Seed = 3630; // LC problem number
    private const int MaxValue = 1_000_000_000;

    // The benchmark's own params (8 and 14) are too small a sample for a claim about how the draw is
    // spread, so the range fact below is measured on a larger draw of the same generator; the count,
    // band and determinism facts stay on the benchmark's own element count.
    private const int StatisticalCount = 256;

    [Fact]
    public void BuildNums_ElementCount_ReturnsOneValuePerPosition() =>
        Assert.Equal(ElementCount, PartitionArrayForMaximumXorAndAndWorkloads.BuildNums(ElementCount, Seed).Length);

    [Fact]
    public void BuildNums_EveryValue_StaysInsideTheDocumentedThirtyBitBand() =>
        Assert.All(
            PartitionArrayForMaximumXorAndAndWorkloads.BuildNums(ElementCount, Seed),
            value => Assert.InRange(value, 1, MaxValue));

    [Fact]
    public void BuildNums_LargerSample_SpansBothHalvesOfTheBandSoTheAndAndXorWalksHaveWorkToDo()
    {
        var nums = PartitionArrayForMaximumXorAndAndWorkloads.BuildNums(StatisticalCount, Seed);
        var half = MaxValue / AlgorithmConstants.HalvingFactor;

        Assert.InRange(nums.Min(), 1, half);
        Assert.InRange(nums.Max(), half, MaxValue);
    }

    [Fact]
    public void BuildNums_SameSeed_ReturnsTheSameNums() =>
        Assert.Equal(
            AnswerText.Of(PartitionArrayForMaximumXorAndAndWorkloads.BuildNums(ElementCount, Seed)),
            AnswerText.Of(PartitionArrayForMaximumXorAndAndWorkloads.BuildNums(ElementCount, Seed)));
}
