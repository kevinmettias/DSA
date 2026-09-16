using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SortAnArrayWorkloads (ARCHITECTURE 17.7). LC 912's solutions sort a copy of
// nums internally, so all this fixture decides is how many values to hand them and which band those
// values come from - and that band spans both signs, which is what stops a comparison sort's branch
// behaviour from being an artifact of every value sharing a sign.
public sealed partial class SortAnArrayWorkloadsTests
{
    private const int Length = 200;
    private const int Seed = 912; // LC problem number
    private const int ValueBand = 50_000;

    [Fact]
    public void BuildValues_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, SortAnArrayWorkloads.BuildValues(Length, Seed).Length);

    [Fact]
    public void BuildValues_EveryValue_FallsInsideTheDocumentedBand() =>
        Assert.All(
            SortAnArrayWorkloads.BuildValues(Length, Seed),
            value => Assert.InRange(value, -ValueBand, ValueBand - 1));

    [Fact]
    public void BuildValues_Values_SpanBothSigns()
    {
        var values = SortAnArrayWorkloads.BuildValues(Length, Seed);

        Assert.Contains(values, value => value < 0);
        Assert.Contains(values, value => value > 0);
    }

    [Fact]
    public void BuildValues_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            SortAnArrayWorkloads.BuildValues(Length, Seed),
            SortAnArrayWorkloads.BuildValues(Length, Seed));
}
