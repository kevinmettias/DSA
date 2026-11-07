using DSAExperimentation.Domain.Mathematics.ExtendedReals;
using Real = DSAExperimentation.Domain.Mathematics.ExtendedReals.ProjectivelyExtendedReal<long>;

namespace DSAExperimentation.Tests.Domain.Mathematics.ExtendedReals;

// The line closed by one point: division by zero is defined, and ∞ has no sign.
public sealed partial class ProjectivelyExtendedRealTests
{
    private static readonly Real Infinity = Real.Infinity;

    [Fact]
    public void TryDivide_NonzeroByZero_IsInfinity()
    {
        AssertDefined(Real.TryDivide, new(1), new(0), Infinity);
        AssertDefined(Real.TryDivide, new(-1), new(0), Infinity);
    }

    [Fact]
    public void TryDivide_ZeroByZero_IsUndefined() => Assert.False(Real.TryDivide(new(0), new(0), out _));

    [Fact]
    public void TryDivide_InfinityByInfinity_IsUndefined() => Assert.False(Real.TryDivide(Infinity, Infinity, out _));

    [Fact]
    public void TryDivide_FiniteByInfinity_IsZero() => AssertDefined(Real.TryDivide, new(5), Infinity, new(0));

    [Fact]
    public void TryDivide_InfinityByFinite_IsInfinity() => AssertDefined(Real.TryDivide, Infinity, new(-2), Infinity);

    [Fact]
    public void TryDivide_Finite_IsTheQuotient() => AssertDefined(Real.TryDivide, new(9), new(3), new(3));

    [Fact]
    public void TryAdd_InfinityToItself_IsUndefined() => Assert.False(Real.TryAdd(Infinity, Infinity, out _));

    [Fact]
    public void TryAdd_FiniteAndInfinity_IsInfinity() => AssertDefined(Real.TryAdd, new(-8), Infinity, Infinity);

    [Fact]
    public void TryAdd_Finite_IsTheSum() => AssertDefined(Real.TryAdd, new(2), new(3), new(5));

    [Fact]
    public void TrySubtract_InfinityFromItself_IsUndefined() => Assert.False(Real.TrySubtract(Infinity, Infinity, out _));

    [Fact]
    public void TryMultiply_ZeroAndInfinity_IsUndefined() => Assert.False(Real.TryMultiply(Infinity, new(0), out _));

    [Fact]
    public void TryMultiply_NonzeroAndInfinity_IsInfinity() =>
        AssertDefined(Real.TryMultiply, new(-3), Infinity, Infinity);

    [Fact]
    public void TryMultiply_Finite_IsTheProduct() => AssertDefined(Real.TryMultiply, new(-3), new(4), new(-12));

    [Fact]
    public void Negate_Infinity_IsInfinity()
    {
        Assert.Equal(Infinity, Real.Negate(Infinity));
        Assert.Equal(new Real(-4), Real.Negate(new Real(4)));
    }

    [Fact]
    public void TryGetFinite_ReadsOnlyAFiniteValue()
    {
        Assert.True(new Real(6).TryGetFinite(out var six));
        Assert.Equal(6, six);
        Assert.False(Infinity.TryGetFinite(out _));
        Assert.False(Infinity.IsFinite);
    }

    [Fact]
    public void Constructor_ADoublesNaN_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProjectivelyExtendedReal<double>(double.NaN));

    private static void AssertDefined(Operation operation, Real left, Real right, Real expected)
    {
        Assert.True(operation(left, right, out var result));
        Assert.Equal(expected, result);
    }

    private delegate bool Operation(Real left, Real right, out Real result);
}
