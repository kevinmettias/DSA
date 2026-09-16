using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PolygonTriangulationWorkloads (ARCHITECTURE 17.7). The reading depends on LC
// 1039's vertex weights being positive values well below the round-trip cost both strategies compare,
// so a choice of apex is decided by the weights rather than by a zero swallowing the product.
public sealed partial class PolygonTriangulationWorkloadsTests
{
    private const int VertexCount = 10;
    private const int Seed = 1;
    private const int MaxVertexWeightExclusive = 100;

    [Fact]
    public void BuildVertexWeights_VertexCount_ReturnsOneWeightPerVertex() =>
        Assert.Equal(
            VertexCount,
            PolygonTriangulationWorkloads.BuildVertexWeights(VertexCount, Seed).Length);

    [Fact]
    public void BuildVertexWeights_EveryWeight_IsPositiveAndStaysBelowTheDocumentedUpperBound() =>
        Assert.All(
            PolygonTriangulationWorkloads.BuildVertexWeights(VertexCount, Seed),
            weight => Assert.InRange(weight, 1, MaxVertexWeightExclusive - 1));

    [Fact]
    public void BuildVertexWeights_SameSeed_ReturnsTheSameWeights() =>
        Assert.Equal(
            AnswerText.Of(PolygonTriangulationWorkloads.BuildVertexWeights(VertexCount, Seed)),
            AnswerText.Of(PolygonTriangulationWorkloads.BuildVertexWeights(VertexCount, Seed)));
}
