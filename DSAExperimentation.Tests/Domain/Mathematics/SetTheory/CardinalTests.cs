using DSAExperimentation.Domain.Mathematics.SetTheory;

namespace DSAExperimentation.Tests.Domain.Mathematics.SetTheory;

// Cardinal addition commutes, so 1 + ℵ0 = ℵ0 + 1 = ℵ0, where ordinal addition does not
// (OrdinalTests). An infinite sum or product is the larger size, and a power set is a size larger.
public sealed partial class CardinalTests
{
    [Fact]
    public void Plus_OneAndAlephNull_IsAlephNullEitherWay()
    {
        Assert.Equal(Cardinal.AlephNull, Cardinal.Finite(1).Plus(Cardinal.AlephNull));
        Assert.Equal(Cardinal.AlephNull, Cardinal.AlephNull.Plus(Cardinal.Finite(1)));
    }

    [Fact]
    public void Plus_AlephNullTwice_IsAlephNull() =>
        Assert.Equal(Cardinal.AlephNull, Cardinal.AlephNull.Plus(Cardinal.AlephNull));

    [Fact]
    public void Plus_ContinuumAndAlephNull_IsTheContinuum() =>
        Assert.Equal(Cardinal.Continuum, Cardinal.AlephNull.Plus(Cardinal.Continuum));

    [Fact]
    public void Plus_FiniteCounts_AddAsNumbers() =>
        Assert.Equal(Cardinal.Finite(7), Cardinal.Finite(3).Plus(Cardinal.Finite(4)));

    [Fact]
    public void Times_AlephNullSquared_IsAlephNull() =>
        Assert.Equal(Cardinal.AlephNull, Cardinal.AlephNull.Times(Cardinal.AlephNull));

    [Fact]
    public void Times_ZeroAndAnInfiniteSize_IsZero() =>
        Assert.Equal(Cardinal.Finite(0), Cardinal.Continuum.Times(Cardinal.Finite(0)));

    [Fact]
    public void Times_FiniteAndInfinite_IsTheInfinite() =>
        Assert.Equal(Cardinal.Continuum, Cardinal.Finite(5).Times(Cardinal.Continuum));

    [Fact]
    public void Times_FiniteCounts_MultiplyAsNumbers() =>
        Assert.Equal(Cardinal.Finite(12), Cardinal.Finite(3).Times(Cardinal.Finite(4)));

    [Fact]
    public void PowerSet_OfAlephNull_IsTheContinuumAndLarger()
    {
        Assert.Equal(Cardinal.Continuum, Cardinal.AlephNull.PowerSet());
        Assert.True(Cardinal.Continuum.CompareTo(Cardinal.AlephNull) > 0);
    }

    [Fact]
    public void PowerSet_OfTheContinuum_IsBethTwo() => Assert.Equal(Cardinal.Beth(2), Cardinal.Continuum.PowerSet());

    [Fact]
    public void PowerSet_OfAFiniteCount_IsTwoToIt() => Assert.Equal(Cardinal.Finite(8), Cardinal.Finite(3).PowerSet());

    [Fact]
    public void PowerSet_PastALong_Throws() =>
        Assert.Throws<InvalidOperationException>(() => Cardinal.Finite(63).PowerSet());

    [Fact]
    public void PowerSet_OfTheLargestThatFits_IsTwoToTheSixtySecond() =>
        Assert.Equal(Cardinal.Finite(1L << 62), Cardinal.Finite(62).PowerSet());

    [Fact]
    public void AlephNull_IsBethZeroAndInfinite()
    {
        Assert.Equal(0, Cardinal.AlephNull.BethIndex);
        Assert.False(Cardinal.AlephNull.IsFinite);
    }

    [Fact]
    public void Count_OfAFiniteCardinal_IsItsCount() => Assert.Equal(9, Cardinal.Finite(9).Count);

    [Fact]
    public void Count_OfAnInfiniteCardinal_Throws() =>
        Assert.Throws<InvalidOperationException>(() => Cardinal.AlephNull.Count);

    [Fact]
    public void BethIndex_OfAFiniteCardinal_Throws() =>
        Assert.Throws<InvalidOperationException>(() => Cardinal.Finite(1).BethIndex);

    [Fact]
    public void Finite_NegativeCount_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => Cardinal.Finite(-1));

    [Fact]
    public void Beth_NegativeIndex_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => Cardinal.Beth(-1));

    [Fact]
    public void CompareTo_EveryFiniteCountIsBelowAlephNull() =>
        Assert.True(Cardinal.Finite(long.MaxValue).CompareTo(Cardinal.AlephNull) < 0);

    [Fact]
    public void Default_IsTheEmptySetsSize() => Assert.Equal(Cardinal.Finite(0), default);

    [Fact]
    public void ToString_PrintsTheCountOrTheBethIndexThatHolds()
    {
        Assert.Equal("Cardinal { Count = 5 }", Cardinal.Finite(5).ToString());
        Assert.Equal("Cardinal { BethIndex = 1 }", Cardinal.Continuum.ToString());
    }
}
