using System.Collections.Immutable;
using DSAExperimentation.Domain.Games.Dice;

namespace DSAExperimentation.Tests.Domain.Games.Dice;

// The faces are the fact itself, so the tests state each preset's numbers in order.
public sealed partial class DiePresetsTests
{
    public static TheoryData<string, int[]> PresetFaces => new()
    {
        { nameof(FourSidedDie), [1, 2, 3, 4] },
        { nameof(EightSidedDie), [1, 2, 3, 4, 5, 6, 7, 8] },
        { nameof(TenSidedDie), [0, 1, 2, 3, 4, 5, 6, 7, 8, 9] },
        { nameof(TwelveSidedDie), [.. Enumerable.Range(1, 12)] },
        { nameof(TwentySidedDie), [.. Enumerable.Range(1, 20)] },
        { nameof(PercentileDie), [0, 10, 20, 30, 40, 50, 60, 70, 80, 90] },
        { nameof(FudgeDie), [-1, -1, 0, 0, 1, 1] },
        { nameof(DoublingCube), [2, 4, 8, 16, 32, 64] },
        { nameof(SichermanSmallDie), [1, 2, 2, 3, 3, 4] },
        { nameof(SichermanLargeDie), [1, 3, 4, 5, 6, 8] },
    };

    [Theory]
    [MemberData(nameof(PresetFaces))]
    public void Faces_ArePresetsNumbersInOrderAndFair(string preset, int[] values)
    {
        var faces = FacesOf(preset);

        Assert.Equal(values, faces.Select(face => face.Value));
        Assert.All(faces, face => Assert.Equal(1, face.Weight));
    }

    private static ImmutableArray<DieFace> FacesOf(string preset) => preset switch
    {
        nameof(FourSidedDie) => FourSidedDie.Faces,
        nameof(EightSidedDie) => EightSidedDie.Faces,
        nameof(TenSidedDie) => TenSidedDie.Faces,
        nameof(TwelveSidedDie) => TwelveSidedDie.Faces,
        nameof(TwentySidedDie) => TwentySidedDie.Faces,
        nameof(PercentileDie) => PercentileDie.Faces,
        nameof(FudgeDie) => FudgeDie.Faces,
        nameof(DoublingCube) => DoublingCube.Faces,
        nameof(SichermanSmallDie) => SichermanSmallDie.Faces,
        _ => SichermanLargeDie.Faces,
    };
}
