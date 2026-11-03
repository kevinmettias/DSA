namespace DSAExperimentation.Tests.Architecture;

// ARCHITECTURE.md section 17.5's one source per fact: every arm, the baseline among them, reads a
// value the library or the LeetCode tier's root states rather than declaring its own copy.
// LibraryValueCopies says what counts as a copy, and LibraryValueCopiesTests pins that down.
public sealed partial class SharedValueCopyTests
{
    // Keyed by the copy's full name: a constant that repeats a shared value but means something else.
    private static readonly Dictionary<string, string> AllowedCopies = new(StringComparer.Ordinal)
    {
        ["DSAExperimentation.LeetCode.Solutions.ValidSquare.ValidSquareSolution.FourthSideIndex"] =
            "The fourth side's place among the sorted distances, not how many sides a polygon needs.",
        ["DSAExperimentation.LeetCode.Solutions.TransposeMatrix.TransposeMatrixSolution.BlockSize"] =
            "The side of a tile the cache-blocked transpose copies at once, not the bits in an integer.",
        ["DSAExperimentation.LeetCode.Solutions.AvailableCapturesForRook.AvailableCapturesForRookSolution.Empty"] =
            "An empty square in LC 999's chessboard notation, not Sudoku's empty cell.",
        ["DSAExperimentation.LeetCode.Solutions.CreateGridWithExactlyKPathsI.CreateGridWithExactlyKPathsISolution.FreeCell"] =
            "An open cell in the grid LC 3934's answer draws, not Sudoku's empty cell.",
        ["DSAExperimentation.LeetCode.Solutions.CreateGridWithExactlyKPathsI.CreateGridWithExactlyKPathsISolution.DoubleChainSide"] =
            "How many cells a doubled chain's side spans, not how many sides a polygon needs.",
        ["DSAExperimentation.LeetCode.Solutions.CheckIfAStringContainsAllBinaryCodesOfSizeK.CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.Zero"] =
            "A binary string's 0 bit, which LC 1461 writes with the same glyph as the decimal digit.",
    };

    private static readonly Lazy<IReadOnlyList<LibraryValueCopies.Copy>> Copies =
        new(() => LibraryValueCopies.In(RepositoryCompilation.Framework, RepositoryCompilation.Solutions));

    [Fact]
    public void SolutionConstants_CopyNoSharedValue()
    {
        var offences = Copies.Value
            .Where(copy => !AllowedCopies.ContainsKey(copy.Name))
            .Select(copy => $"{copy.Name} repeats {copy.Source}; read it from there (ARCHITECTURE.md 17.5).")
            .ToList();

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    // An entry whose copy has gone would otherwise sit in the list excusing nothing.
    [Fact]
    public void AllowedCopies_AreAllStillCopies()
        => Assert.DoesNotContain(AllowedCopies.Keys, name => !Copies.Value.Any(copy => copy.Name == name));
}
