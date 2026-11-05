using DSAExperimentation.Domain.Games.Chess;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Games.Chess;

// Every preset is the piece its Betza notation writes, compared as the set of (step, reach, when) it
// can make, since the queen's one move in the Octilinear table's order is the notation's two rides.
public sealed partial class BetzaNotationTests
{
    public static TheoryData<string, string, string> Presets => new()
    {
        { "rook", "R", "white" },
        { "bishop", "B", "white" },
        { "queen", "Q", "white" },
        { "king", "K", "white" },
        { "knight", "N", "white" },
        { "white pawn", "mfWcfFifmW2", "white" },
        { "black pawn", "mfWcfFifmW2", "black" },
    };

    public static TheoryData<string> Unread => ["lW", "X", "W!", "mf"];

    [Theory]
    [MemberData(nameof(Presets))]
    public void Parse_PresetsNotation_IsThePreset(string preset, string notation, string side) =>
        Assert.Equal(Expanded(PresetNamed(preset).Moves), Expanded(BetzaNotation.Parse(notation, SideNamed(side))));

    [Fact]
    public void Parse_AtomWrittenTwice_Rides() =>
        Assert.Null(Assert.Single(BetzaNotation.Parse("WW", ChessSide.White)).Reach);

    [Fact]
    public void Parse_AtomWithANumber_RidesThatFar() =>
        Assert.Equal(3, Assert.Single(BetzaNotation.Parse("F3", ChessSide.White)).Reach);

    [Fact]
    public void Parse_DabbabaAndAlfil_LeapTwoSquares() =>
        Assert.Equal(
            [.. GridDirections.Orthogonal.Concat(GridDirections.Diagonal).Select(step => step + step)],
            BetzaNotation.Parse("DA", ChessSide.White).SelectMany(move => move.Steps).ToList());

    // The knight's four forward jumps for each side: two rows forward and one across, or one forward
    // and two across.
    [Fact]
    public void Parse_Forward_KeepsTheSidesForwardSteps() =>
        Assert.Equal(
            (true, true, 4, 4),
            (
                Assert.Single(BetzaNotation.Parse("fN", ChessSide.White)).Steps.All(step => step.DeltaRow < 0),
                Assert.Single(BetzaNotation.Parse("fN", ChessSide.Black)).Steps.All(step => step.DeltaRow > 0),
                BetzaNotation.Parse("fN", ChessSide.White)[0].Steps.Length,
                BetzaNotation.Parse("bN", ChessSide.White)[0].Steps.Length));

    [Fact]
    public void Parse_MovesAndCapturesBoth_IsEither() =>
        Assert.Equal(MoveConditions.MovesOrCaptures, Assert.Single(BetzaNotation.Parse("mcW", ChessSide.White)).When);

    [Theory]
    [MemberData(nameof(Unread))]
    public void Parse_LetterItDoesNotRead_Throws(string notation) =>
        Assert.Throws<ArgumentException>(() => BetzaNotation.Parse(notation, ChessSide.White));

    private static HashSet<(GridStep Step, int? Reach, MoveConditions When)> Expanded(IEnumerable<PieceMove> moves) =>
        [.. moves.SelectMany(move => move.Steps.Select(step => (step, move.Reach, move.When)))];

    private static ChessPiece PresetNamed(string preset) => preset switch
    {
        "rook" => ChessPiece.Rook,
        "bishop" => ChessPiece.Bishop,
        "queen" => ChessPiece.Queen,
        "king" => ChessPiece.King,
        "knight" => ChessPiece.Knight,
        "white pawn" => ChessPiece.WhitePawn,
        _ => ChessPiece.BlackPawn,
    };

    private static ChessSide SideNamed(string side) => side == "white" ? ChessSide.White : ChessSide.Black;
}
