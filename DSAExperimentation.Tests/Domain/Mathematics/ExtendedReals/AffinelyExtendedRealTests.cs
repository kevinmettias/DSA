using DSAExperimentation.Domain.Mathematics.ExtendedReals;
using Real = DSAExperimentation.Domain.Mathematics.ExtendedReals.AffinelyExtendedReal<long>;

namespace DSAExperimentation.Tests.Domain.Mathematics.ExtendedReals;

// The arithmetic rules and the undefined forms of the line with two ends, each as its own test.
public sealed partial class AffinelyExtendedRealTests
{
    private static readonly Real PlusInfinity = Real.PositiveInfinity;
    private static readonly Real MinusInfinity = Real.NegativeInfinity;

    [Fact]
    public void TryAdd_FiniteAndPlusInfinity_IsPlusInfinity() => AssertDefined(Real.TryAdd, new(5), PlusInfinity, PlusInfinity);

    [Fact]
    public void TryAdd_InfinityToItself_IsThatInfinity() =>
        AssertDefined(Real.TryAdd, MinusInfinity, MinusInfinity, MinusInfinity);

    [Fact]
    public void TryAdd_OppositeInfinities_IsUndefined() => Assert.False(Real.TryAdd(PlusInfinity, MinusInfinity, out _));

    [Fact]
    public void TryAdd_Finite_IsTheSum() => AssertDefined(Real.TryAdd, new(2), new(3), new(5));

    [Fact]
    public void TrySubtract_InfinityFromItself_IsUndefined() =>
        Assert.False(Real.TrySubtract(PlusInfinity, PlusInfinity, out _));

    [Fact]
    public void TrySubtract_MinusInfinityFromPlusInfinity_IsPlusInfinity() =>
        AssertDefined(Real.TrySubtract, PlusInfinity, MinusInfinity, PlusInfinity);

    [Fact]
    public void TryMultiply_ZeroAndInfinity_IsUndefined() => Assert.False(Real.TryMultiply(new(0), PlusInfinity, out _));

    [Fact]
    public void TryMultiply_NegativeAndPlusInfinity_IsMinusInfinity() =>
        AssertDefined(Real.TryMultiply, new(-2), PlusInfinity, MinusInfinity);

    [Fact]
    public void TryMultiply_TwoMinusInfinities_IsPlusInfinity() =>
        AssertDefined(Real.TryMultiply, MinusInfinity, MinusInfinity, PlusInfinity);

    [Fact]
    public void TryMultiply_Finite_IsTheProduct() => AssertDefined(Real.TryMultiply, new(-3), new(4), new(-12));

    [Fact]
    public void TryDivide_ByZero_IsUndefined()
    {
        Assert.False(Real.TryDivide(new(1), new(0), out _));
        Assert.False(Real.TryDivide(new(0), new(0), out _));
        Assert.False(Real.TryDivide(PlusInfinity, new(0), out _));
    }

    [Fact]
    public void TryDivide_InfinityByInfinity_IsUndefined() =>
        Assert.False(Real.TryDivide(PlusInfinity, MinusInfinity, out _));

    [Fact]
    public void TryDivide_FiniteByInfinity_IsZero() => AssertDefined(Real.TryDivide, new(7), MinusInfinity, new(0));

    [Fact]
    public void TryDivide_InfinityByNegative_FlipsItsSign() =>
        AssertDefined(Real.TryDivide, PlusInfinity, new(-3), MinusInfinity);

    [Fact]
    public void TryDivide_Finite_IsTheQuotient() => AssertDefined(Real.TryDivide, new(12), new(4), new(3));

    [Fact]
    public void Negate_SwapsTheEnds()
    {
        Assert.Equal(MinusInfinity, Real.Negate(PlusInfinity));
        Assert.Equal(new Real(-4), Real.Negate(new Real(4)));
    }

    [Fact]
    public void CompareTo_PutsMinusInfinityFirstAndPlusInfinityLast() =>
        Assert.Equal(
            [MinusInfinity, new Real(long.MinValue), new Real(0), new Real(long.MaxValue), PlusInfinity],
            new[] { PlusInfinity, new Real(0), MinusInfinity, new Real(long.MaxValue), new Real(long.MinValue) }.Order());

    [Fact]
    public void TryGetFinite_ReadsOnlyAFiniteValue()
    {
        Assert.True(new Real(6).TryGetFinite(out var six));
        Assert.Equal(6, six);
        Assert.False(PlusInfinity.TryGetFinite(out _));
        Assert.False(PlusInfinity.IsFinite);
    }

    [Fact]
    public void Constructor_ADoublesOwnInfinity_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new AffinelyExtendedReal<double>(double.PositiveInfinity));

    private static void AssertDefined(Operation operation, Real left, Real right, Real expected)
    {
        Assert.True(operation(left, right, out var result));
        Assert.Equal(expected, result);
    }

    private delegate bool Operation(Real left, Real right, out Real result);
}
