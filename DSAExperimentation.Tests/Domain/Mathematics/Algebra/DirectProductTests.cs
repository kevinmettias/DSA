using DSAExperimentation.Domain.Mathematics.Algebra;
using SumAndXor = DSAExperimentation.Domain.Mathematics.Algebra.DirectProduct<
    int, int,
    DSAExperimentation.Domain.Mathematics.Algebra.Addition<int>,
    DSAExperimentation.Domain.Mathematics.Algebra.BitwiseXor<int>>;
using XorAndSum = DSAExperimentation.Domain.Mathematics.Algebra.DirectProduct<
    int, int,
    DSAExperimentation.Domain.Mathematics.Algebra.BitwiseXor<int>,
    DSAExperimentation.Domain.Mathematics.Algebra.Addition<int>>;

namespace DSAExperimentation.Tests.Domain.Mathematics.Algebra;

// The product of two different groups, addition and exclusive or, so a component combined or
// inverted in the other's group would show: each law is checked on pairs whose components answer
// differently under the two. Exclusive or is its own inverse, so inverting is checked with the
// groups each way round, where either component left uninverted would show.
public sealed partial class DirectProductTests
{
    private const int LeftFirst = 5;
    private const int LeftSecond = 6;
    private const int RightFirst = 3;
    private const int RightSecond = 3;

    private static readonly (int First, int Second) Left = (LeftFirst, LeftSecond);
    private static readonly (int First, int Second) Right = (RightFirst, RightSecond);

    [Fact]
    public void Identity_IsThePairOfIdentities() =>
        Assert.Equal((Addition<int>.Identity, BitwiseXor<int>.Identity), SumAndXor.Identity);

    [Fact]
    public void Combine_CombinesEachComponentInItsOwnGroup() =>
        Assert.Equal(
            (LeftFirst + RightFirst, LeftSecond ^ RightSecond), SumAndXor.Combine(Left, Right));

    [Fact]
    public void Combine_WithIdentity_ReturnsThePairUnchanged() =>
        Assert.Equal(Left, SumAndXor.Combine(SumAndXor.Identity, Left));

    [Fact]
    public void Combine_InEitherOrder_GivesTheSamePair() =>
        Assert.Equal(SumAndXor.Combine(Left, Right), SumAndXor.Combine(Right, Left));

    [Fact]
    public void Invert_InvertsEachComponentInItsOwnGroup() =>
        Assert.Equal((-LeftFirst, LeftSecond), SumAndXor.Invert(Left));

    [Fact]
    public void Invert_GroupsTheOtherWayRound_InvertsEachComponentInItsOwnGroup() =>
        Assert.Equal((LeftFirst, -LeftSecond), XorAndSum.Invert(Left));

    [Fact]
    public void Combine_WithInvertOfSamePair_ReturnsIdentity() =>
        Assert.Equal(SumAndXor.Identity, SumAndXor.Combine(Left, SumAndXor.Invert(Left)));
}
