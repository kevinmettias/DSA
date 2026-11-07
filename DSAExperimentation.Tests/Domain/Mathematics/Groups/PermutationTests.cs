using DSAExperimentation.Domain.Mathematics.Groups;

namespace DSAExperimentation.Tests.Domain.Mathematics.Groups;

// The operations on a few named permutations of three points. The two transpositions (0 1) and
// (1 2) do not commute, so a composition or an inverse taken in the wrong order shows on them, and
// the 3-cycle (0 1 2) is its own inverse's square, so an inverse that returned the permutation
// itself shows on it.
public sealed partial class PermutationTests
{
    private static readonly Permutation SwapZeroOne = Permutation.FromCycles(3, [0, 1]);
    private static readonly Permutation SwapOneTwo = Permutation.FromCycles(3, [1, 2]);
    private static readonly Permutation RotateThree = Permutation.FromCycles(3, [0, 1, 2]);
    private static readonly Permutation SwapZeroOneOfThree = Permutation.FromImages([1, 0, 2]);

    public static TheoryData<int[]> NotPermutations =>
        new() { new[] { 0, 0 }, new[] { 1, 2 }, new[] { -1, 0 } };

    [Fact]
    public void Identity_SendsEveryPointToItself() =>
        Assert.Equal([0, 1, 2, 3], Permutation.Identity(4).Images.ToArray());

    [Fact]
    public void Identity_OfANegativeDegree_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Permutation.Identity(-1));

    [Fact]
    public void Degree_OfTheDefaultPermutation_IsNone() =>
        Assert.Equal(0, default(Permutation).Degree);

    [Fact]
    public void Images_OfTheDefaultPermutation_AreTheIdentityOfNoPoints() =>
        Assert.Equal(Permutation.Identity(0), default);

    [Fact]
    public void FromImages_ReadsOneLineNotation() =>
        Assert.Equal(2, Permutation.FromImages([2, 0, 1]).Apply(0));

    [Theory]
    [MemberData(nameof(NotPermutations))]
    public void FromImages_NotNamingEachPointOnce_Throws(int[] images) =>
        Assert.Throws<ArgumentException>(() => Permutation.FromImages(images));

    [Fact]
    public void FromImages_CopiesTheImages()
    {
        int[] images = [1, 0];
        var swap = Permutation.FromImages(images);

        images[0] = 0;

        Assert.Equal([1, 0], swap.Images.ToArray());
    }

    [Fact]
    public void FromCycles_SendsEachPointToTheNextAndTheLastToTheFirst() =>
        Assert.Equal([2, 1, 4, 3, 0], Permutation.FromCycles(5, [0, 2, 4]).Images.ToArray());

    [Fact]
    public void FromCycles_WithAPointInTwoCycles_Throws() =>
        Assert.Throws<ArgumentException>(() => Permutation.FromCycles(4, [0, 1], [1, 2]));

    [Fact]
    public void FromCycles_WithAPointPastTheDegree_Throws() =>
        Assert.Throws<ArgumentException>(() => Permutation.FromCycles(3, [1, 3]));

    [Fact]
    public void Apply_ToAPointPastTheDegree_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SwapZeroOne.Apply(3));

    // The item at position i lands at position p(i): 'a' at 0 goes to 2, 'b' at 1 to 0, 'c' to 1.
    [Fact]
    public void Apply_ToASequence_MovesTheItemAtEachPositionToItsImage() =>
        Assert.Equal("bca", new string(Permutation.FromImages([2, 0, 1]).Apply<char>("abc")));

    [Fact]
    public void Apply_ToASequenceOfAnotherLength_Throws() =>
        Assert.Throws<ArgumentException>(() => SwapZeroOne.Apply<char>("ab"));

    [Fact]
    public void Apply_ToASequence_ByACompositeAppliesTheInnerFirst()
    {
        var innerFirst = SwapZeroOne.Apply(SwapOneTwo.Apply<char>("abc"));

        Assert.Equal(innerFirst, SwapZeroOne.After(SwapOneTwo).Apply<char>("abc"));
    }

    // (0 1) after (1 2): 0 stays under (1 2), then goes to 1; 1 goes to 2, which (0 1) keeps; 2
    // goes to 1, then to 0.
    [Fact]
    public void After_AppliesTheInnerPermutationFirst() =>
        Assert.Equal([1, 2, 0], SwapZeroOne.After(SwapOneTwo).Images.ToArray());

    [Fact]
    public void After_SendsEachPointToTheOuterImageOfTheInnerImage()
    {
        var composite = RotateThree.After(SwapZeroOne);

        Assert.All(Enumerable.Range(0, 3), point =>
            Assert.Equal(RotateThree.Apply(SwapZeroOne.Apply(point)), composite.Apply(point)));
    }

    [Fact]
    public void After_APermutationOfAnotherDegree_Throws() =>
        Assert.Throws<ArgumentException>(() => SwapZeroOne.After(Permutation.Identity(4)));

    [Fact]
    public void Inverse_ComposedEitherWay_IsTheIdentity()
    {
        Assert.Equal(Permutation.Identity(3), RotateThree.After(RotateThree.Inverse()));
        Assert.Equal(Permutation.Identity(3), RotateThree.Inverse().After(RotateThree));
    }

    [Fact]
    public void Inverse_OfACycle_RunsItBackwards() =>
        Assert.Equal(Permutation.FromCycles(3, [2, 1, 0]), RotateThree.Inverse());

    [Fact]
    public void Inverse_OfAComposite_IsTheInversesInTheOtherOrder()
    {
        var composite = SwapZeroOne.After(SwapOneTwo);

        Assert.Equal(SwapOneTwo.Inverse().After(SwapZeroOne.Inverse()), composite.Inverse());
        Assert.NotEqual(SwapZeroOne.Inverse().After(SwapOneTwo.Inverse()), composite.Inverse());
    }

    [Fact]
    public void Equals_SameImages_IsTrue() => Assert.True(SwapZeroOne.Equals(SwapZeroOneOfThree));

    [Fact]
    public void Equals_IdentitiesOfDifferentDegrees_IsFalse() =>
        Assert.False(Permutation.Identity(3).Equals(Permutation.Identity(4)));

    [Fact]
    public void Equals_ABoxedEqualPermutation_IsTrue() =>
        Assert.True(SwapZeroOne.Equals((object)SwapZeroOneOfThree));

    [Fact]
    public void GetHashCode_OfEqualPermutations_IsEqual() =>
        Assert.Equal(SwapZeroOne.GetHashCode(), SwapZeroOneOfThree.GetHashCode());
}
