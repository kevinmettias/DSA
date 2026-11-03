using System.Collections.Immutable;
using DSAExperimentation.Domain.Games.Dice;

namespace DSAExperimentation.Tests.Domain.Games.Dice;

// What follows from a die's faces, read through presets whose faces are a run, repeat a value, or are
// weighted unequally.
public sealed partial class DieTests
{
    [Fact]
    public void LowestFace_SixSidedDie_IsOne() => Assert.Equal(1, Die<SixSidedDie>.LowestFace);

    [Fact]
    public void HighestFace_SixSidedDie_IsSix() => Assert.Equal(6, Die<SixSidedDie>.HighestFace);

    // A ten-sided die is printed from 0, so it ends one lower than its face count.
    [Fact]
    public void HighestFace_TenSidedDie_IsNine() => Assert.Equal(9, Die<TenSidedDie>.HighestFace);

    [Fact]
    public void FaceCount_FudgeDie_CountsEachRepeatedValue() => Assert.Equal(6, Die<FudgeDie>.FaceCount);

    [Fact]
    public void TotalWeight_FairDie_IsItsFaceCount() => Assert.Equal(20, Die<TwentySidedDie>.TotalWeight);

    // A fair die's total is its face count, so a loaded die is the case that tells a sum from a count.
    [Fact]
    public void TotalWeight_LoadedDie_IsTheSumOfItsWeights() => Assert.Equal(10, Die<LoadedSix>.TotalWeight);

    [Fact]
    public void IsFair_EqualWeights_IsTrue() => Assert.True(Die<SixSidedDie>.IsFair);

    [Fact]
    public void IsFair_LoadedDie_IsFalse() => Assert.False(Die<LoadedSix>.IsFair);

    [Fact]
    public void Weights_LoadedDie_AreItsFacesWeightsInOrder() => Assert.Equal<int>([1, 1, 1, 1, 1, 5], Die<LoadedSix>.Weights);

    [Fact]
    public void Faces_AreTheWitnesssFaces() => Assert.Equal<DieFace>(SixSidedDie.Faces, Die<SixSidedDie>.Faces);

    // Sicherman's pair sums with exactly the distribution of two common six-sided dice.
    [Fact]
    public void Faces_SichermanPair_SumLikeTwoSixSidedDice() =>
        Assert.Equal(SumCounts(SixSidedDie.Faces, SixSidedDie.Faces), SumCounts(SichermanSmallDie.Faces, SichermanLargeDie.Faces));

    private static SortedDictionary<int, int> SumCounts(ImmutableArray<DieFace> first, ImmutableArray<DieFace> second)
    {
        var counts = new SortedDictionary<int, int>();

        foreach (var a in first)
        {
            foreach (var b in second)
            {
                counts[a.Value + b.Value] = counts.GetValueOrDefault(a.Value + b.Value) + (a.Weight * b.Weight);
            }
        }

        return counts;
    }

    // A six weighted five times as heavily as each other face.
    private readonly struct LoadedSix : IDie
    {
        public static ImmutableArray<DieFace> Faces { get; } = [.. DieFace.FairRun(1, 5), new DieFace(6, 5)];
    }
}
