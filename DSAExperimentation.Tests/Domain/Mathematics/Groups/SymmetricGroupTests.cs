using DSAExperimentation.Domain.Mathematics.Algebra;
using DSAExperimentation.Domain.Mathematics.Groups;

namespace DSAExperimentation.Tests.Domain.Mathematics.Groups;

// S_3 through the group contract: the laws checked by a helper that knows only IGroup, over every
// element and every pair or triple, then the order Combine composes in, on two transpositions that
// do not commute. S_3 is the smallest group that is not abelian, so a law that holds only for
// commuting elements would fail here.
public sealed partial class SymmetricGroupTests
{
    private const int DegreeOfThree = 3;
    private const int DegreeOfFour = 4;

    private static readonly Permutation SwapZeroOne = Permutation.FromCycles(DegreeOfThree, [0, 1]);
    private static readonly Permutation SwapOneTwo = Permutation.FromCycles(DegreeOfThree, [1, 2]);
    private static readonly Permutation IdentityOfFour = Permutation.Identity(DegreeOfFour);

    [Fact]
    public void Identity_IsTheIdentityOfTheGroupsDegree() =>
        Assert.Equal(Permutation.Identity(DegreeOfThree), SymmetricGroup<Three>.Identity);

    [Fact]
    public void Identity_OfAnotherDegree_IsThatDegrees() =>
        Assert.Equal(DegreeOfFour, SymmetricGroup<Four>.Identity.Degree);

    // (0 1) first sends 0 to 1, then (1 2) sends 1 to 2: so 0 goes to 2, 1 to 0, and 2 to 1.
    [Fact]
    public void Combine_AppliesTheLeftPermutationFirst() =>
        Assert.Equal(
            [2, 0, 1], SymmetricGroup<Three>.Combine(SwapZeroOne, SwapOneTwo).Images.ToArray());

    [Fact]
    public void Combine_IsTheRightAfterTheLeft() =>
        Assert.Equal(
            SwapOneTwo.After(SwapZeroOne), SymmetricGroup<Three>.Combine(SwapZeroOne, SwapOneTwo));

    [Fact]
    public void Combine_DoesNotCommute() =>
        Assert.NotEqual(
            SymmetricGroup<Three>.Combine(SwapZeroOne, SwapOneTwo),
            SymmetricGroup<Three>.Combine(SwapOneTwo, SwapZeroOne));

    [Fact]
    public void Combine_ObeysTheGroupLaws_OverAllOfTheGroup() =>
        AssertGroupLaws<Permutation, SymmetricGroup<Three>>(
            [.. PermutationCyclesTests.AllOfDegree(DegreeOfThree)]);

    [Fact]
    public void Combine_APermutationOfAnotherDegree_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            SymmetricGroup<Three>.Combine(IdentityOfFour, IdentityOfFour));
        Assert.Throws<ArgumentException>(() =>
            SymmetricGroup<Three>.Combine(SwapZeroOne, IdentityOfFour));
    }

    [Fact]
    public void Invert_UndoesThePermutation()
    {
        var composite = SwapZeroOne.After(SwapOneTwo);
        var inverse = SymmetricGroup<Three>.Invert(composite);

        Assert.Equal(
            SymmetricGroup<Three>.Identity, SymmetricGroup<Three>.Combine(composite, inverse));
    }

    [Fact]
    public void Invert_APermutationOfAnotherDegree_Throws() =>
        Assert.Throws<ArgumentException>(() => SymmetricGroup<Three>.Invert(IdentityOfFour));

    // Identity, inverse and associativity, read only through the contract a generic structure sees.
    private static void AssertGroupLaws<TElement, TGroup>(TElement[] elements)
        where TGroup : struct, IGroup<TElement>
    {
        foreach (var element in elements)
        {
            Assert.Equal(element, TGroup.Combine(TGroup.Identity, element));
            Assert.Equal(element, TGroup.Combine(element, TGroup.Identity));
            Assert.Equal(TGroup.Identity, TGroup.Combine(element, TGroup.Invert(element)));
            Assert.Equal(TGroup.Identity, TGroup.Combine(TGroup.Invert(element), element));
            Assert.All(elements, second =>
                AssertAssociative<TElement, TGroup>(element, second, elements));
        }
    }

    // (first second) third = first (second third), for every third element.
    private static void AssertAssociative<TElement, TGroup>(
        TElement first, TElement second, TElement[] elements)
        where TGroup : struct, IGroup<TElement>
        => Assert.All(elements, third =>
            Assert.Equal(
                TGroup.Combine(TGroup.Combine(first, second), third),
                TGroup.Combine(first, TGroup.Combine(second, third))));

    private readonly struct Three : IPermutationDegree
    {
        public static int Degree => DegreeOfThree;
    }

    private readonly struct Four : IPermutationDegree
    {
        public static int Degree => DegreeOfFour;
    }
}
