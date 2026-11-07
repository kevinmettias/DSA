using DSAExperimentation.Domain.Mathematics.SetTheory;

namespace DSAExperimentation.Tests.Domain.Mathematics.SetTheory;

// Ordinal addition is putting one list after another, which does not commute: 1 + ω = ω, but
// ω + 1 is longer. The cardinalities of the two are the same, ℵ0, which is where ordinals and
// cardinals part.
public sealed partial class OrdinalTests
{
    private static readonly Ordinal OmegaPlusOne = Ordinal.Omega.Plus(Ordinal.One);

    // Each row is ω·a + b, ω·c + d and their sum, in Cantor normal form.
    public static TheoryData<Ordinal, Ordinal, Ordinal> Sums => new()
    {
        { Ordinal.Of(0, 2), Ordinal.Of(0, 3), Ordinal.Of(0, 5) },
        { Ordinal.Of(0, 7), Ordinal.Of(1, 0), Ordinal.Of(1, 0) },
        { Ordinal.Of(1, 4), Ordinal.Of(0, 3), Ordinal.Of(1, 7) },
        { Ordinal.Of(1, 4), Ordinal.Of(2, 1), Ordinal.Of(3, 1) },
        { Ordinal.Of(2, 0), Ordinal.Of(1, 0), Ordinal.Of(3, 0) },
    };

    [Fact]
    public void OnePlusOmega_IsOmega() => Assert.Equal(Ordinal.Omega, Ordinal.One.Plus(Ordinal.Omega));

    [Fact]
    public void OmegaPlusOne_IsNotOmegaAndComesAfterIt()
    {
        Assert.NotEqual(Ordinal.Omega, OmegaPlusOne);
        Assert.True(OmegaPlusOne.CompareTo(Ordinal.Omega) > 0);
    }

    [Theory]
    [MemberData(nameof(Sums))]
    public void Plus_CantorNormalForms_AbsorbsTheFinitePartBeforeAnOmega(
        Ordinal first, Ordinal second, Ordinal sum) =>
        Assert.Equal(sum, first.Plus(second));

    [Fact]
    public void Plus_IsAssociative()
    {
        Ordinal[] samples =
            [Ordinal.Zero, Ordinal.One, Ordinal.Finite(3), Ordinal.Omega, OmegaPlusOne, Ordinal.Of(2, 5)];
        var triples = samples.SelectMany(first =>
            samples.SelectMany(second => samples.Select(third => (first, second, third))));

        foreach (var (first, second, third) in triples)
        {
            Assert.Equal(first.Plus(second).Plus(third), first.Plus(second.Plus(third)));
        }
    }

    [Fact]
    public void Cardinality_OmegaAndOmegaPlusOne_AreBothAlephNull()
    {
        Assert.Equal(Cardinal.AlephNull, Ordinal.Omega.Cardinality);
        Assert.Equal(Cardinal.AlephNull, OmegaPlusOne.Cardinality);
    }

    [Fact]
    public void Cardinality_FiniteOrdinal_IsItsCount() => Assert.Equal(Cardinal.Finite(4), Ordinal.Finite(4).Cardinality);

    [Fact]
    public void FiniteOrdinals_SumAsTheirCardinalsSum() =>
        Assert.Equal(
            Cardinal.Finite(3).Plus(Cardinal.Finite(4)),
            Ordinal.Finite(3).Plus(Ordinal.Finite(4)).Cardinality);

    [Fact]
    public void Successor_OfOmega_IsOmegaPlusOneAndNoLimit()
    {
        Assert.Equal(OmegaPlusOne, Ordinal.Omega.Successor());
        Assert.False(OmegaPlusOne.IsLimit);
    }

    [Fact]
    public void IsLimit_OmegaTimesTwo_IsTrueAndZeroIsNot()
    {
        Assert.True(Ordinal.Omega.Plus(Ordinal.Omega).IsLimit);
        Assert.False(Ordinal.Zero.IsLimit);
    }

    [Fact]
    public void IsFinite_IsTrueOnlyWithoutOmega()
    {
        Assert.True(Ordinal.Finite(9).IsFinite);
        Assert.False(Ordinal.Omega.IsFinite);
    }

    [Fact]
    public void Parts_OfOmegaTimesTwoPlusFive_AreTwoAndFive()
    {
        var ordinal = Ordinal.Of(2, 5);
        Assert.Equal(2, ordinal.OmegaCoefficient);
        Assert.Equal(5, ordinal.FinitePart);
    }

    [Fact]
    public void Of_NegativePart_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => Ordinal.Of(0, -1));

    [Fact]
    public void CompareTo_OrdersByOmegasThenFinitePart() =>
        Assert.Equal(
            [Ordinal.Zero, Ordinal.Finite(7), Ordinal.Omega, OmegaPlusOne, Ordinal.Of(2, 0)],
            new[] { Ordinal.Of(2, 0), OmegaPlusOne, Ordinal.Zero, Ordinal.Omega, Ordinal.Finite(7) }.Order());
}
