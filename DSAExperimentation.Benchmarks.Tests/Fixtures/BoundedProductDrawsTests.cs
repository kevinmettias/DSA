using System.Numerics;
using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for BoundedProductDraws (ARCHITECTURE 17.7), the draw LC 238's and LC 1352's
// harnesses share. The reading depends on the product of every value fitting in an int - the
// guarantee both problems state and a uniform draw breaks - while the array is still more than a run
// of ones: some planted factors, and for the signed draw both signs, so the arms multiply real values.
// The product is taken here in BigInteger, so the bound is checked by arithmetic that cannot itself
// overflow.
public sealed partial class BoundedProductDrawsTests
{
    private const int Count = 5_000;
    private const int Seed = 238; // LC problem number
    private const int FactorBoundExclusive = 31;
    private const int FewestPlantedFactors = 2;

    [Fact]
    public void Factors_Count_ReturnsOneValuePerPosition() =>
        Assert.Equal(Count, DrawFactors().Length);

    [Fact]
    public void Factors_EveryValue_IsOneOrAFactorBelowTheBound() =>
        Assert.All(DrawFactors(), value => Assert.InRange(value, 1, FactorBoundExclusive - 1));

    [Fact]
    public void Factors_Product_FitsInAnInt() =>
        Assert.True(MagnitudeProduct(DrawFactors()) <= int.MaxValue);

    [Fact]
    public void Factors_SeededDraw_PlantsMoreThanOneFactor() =>
        Assert.True(DrawFactors().Count(value => value > 1) >= FewestPlantedFactors);

    [Fact]
    public void Factors_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(DrawFactors(), DrawFactors());

    [Fact]
    public void SignedFactors_EveryValue_KeepsItsMagnitudeBelowTheBound() =>
        Assert.All(DrawSignedFactors(), value => Assert.InRange(Math.Abs(value), 1, FactorBoundExclusive - 1));

    [Fact]
    public void SignedFactors_Product_FitsInAnInt() =>
        Assert.True(MagnitudeProduct(DrawSignedFactors()) <= int.MaxValue);

    [Fact]
    public void SignedFactors_SeededDraw_HoldsBothSigns()
    {
        var values = DrawSignedFactors();

        Assert.Contains(values, value => value < 0);
        Assert.Contains(values, value => value > 0);
    }

    private static int[] DrawFactors() =>
        BoundedProductDraws.Factors(Count, FactorBoundExclusive, new Random(Seed));

    private static int[] DrawSignedFactors() =>
        BoundedProductDraws.SignedFactors(Count, FactorBoundExclusive, new Random(Seed));

    private static BigInteger MagnitudeProduct(int[] values) =>
        values.Aggregate(BigInteger.One, (product, value) => product * Math.Abs(value));
}
