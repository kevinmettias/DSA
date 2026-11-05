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
        ["DSAExperimentation.LeetCode.Solutions.EscapeALargeMaze.EscapeALargeMazeBoard.Size"] =
            "The side of LC 1036's board, 10^6 squares, not the parts a whole holds in parts per million.",
        ["DSAExperimentation.LeetCode.Solutions.FindPositiveIntegerSolutionForAGivenEquation.SearchRange.Bound"] =
            "LC 1237's largest x and y, not the parts a whole holds per mille.",
        ["DSAExperimentation.LeetCode.Solutions.Finding3DigitEvenNumbers.Finding3DigitEvenNumbersSolution.HundredsPlace"] =
            "The value of a three-digit number's hundreds place, not the parts a whole holds per cent.",
        ["DSAExperimentation.LeetCode.Solutions.MaximumPrimeDifference.MaximumPrimeDifferenceSolution.MaxValue"] =
            "LC 3115's largest value of nums[i], which bounds its sieve, not the parts a whole holds per cent.",
        ["DSAExperimentation.LeetCode.Solutions.CountNumberOfRectanglesContainingEachPoint.CountNumberOfRectanglesContainingEachPointPromise.MostHeight"] =
            "LC 2250's tallest rectangle or point, the bound its promise checks, not the parts a whole holds per cent.",
        ["DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStockWithTransactionFee.BestTimeToBuyAndSellStockWithTransactionFeeReach.LargestFee"] =
            "The largest fee whose cases every strategy of LC 714 finishes in time, its reach, not the parts a whole holds per cent.",
        ["DSAExperimentation.LeetCode.Solutions.KthSmallestInLexicographicalOrder.KthSmallestInLexicographicalOrderReach.LargestUpperBound"] =
            "The largest n whose cases every strategy of LC 440 finishes in time, its reach, not the parts a whole holds per myriad nor a millisecond's ticks.",
        ["DSAExperimentation.LeetCode.Solutions.MinimumOneBitOperationsToMakeIntegersZero.MinimumOneBitOperationsToMakeIntegersZeroReach.LargestTarget"] =
            "The largest n whose cases every strategy of LC 1611 finishes in time, its reach, not the parts a whole holds per million.",
        ["DSAExperimentation.LeetCode.Solutions.PrisonCellsAfterNDays.PrisonCellsAfterNDaysReach.LargestDayCount"] =
            "The most days whose cases every strategy of LC 957 finishes in time, its reach, not the parts a whole holds per million.",
        ["DSAExperimentation.LeetCode.Solutions.NimGame.NimGameReach.LargestStoneCount"] =
            "The most stones whose cases every strategy of LC 292 finishes in time, its reach, not the parts a whole holds per mille.",
        ["DSAExperimentation.LeetCode.Solutions.NthMagicalNumber.NthMagicalNumberReach.LargestRank"] =
            "The largest n whose cases every strategy of LC 878 finishes in time, its reach, not the parts a whole holds per mille.",
        ["DSAExperimentation.LeetCode.Solutions.NumberOfDigitOne.NumberOfDigitOneReach.LargestUpperBound"] =
            "The largest n whose cases every strategy of LC 233 finishes in time, its reach, not a second's ticks.",
    };

    private static readonly Lazy<IReadOnlyList<LibraryValueCopies.Copy>> Copies =
        new(() => LibraryValueCopies.In(RepositoryCompilation.Framework, RepositoryCompilation.Conventions, RepositoryCompilation.Solutions));

    [Fact]
    public void SolutionConstants_CopyNoSharedValue()
    {
        var offences = Copies.Value
            .Where(copy => !AllowedCopies.ContainsKey(copy.Name))
            .Select(copy => $"{copy.Name} repeats {copy.Source}; read it from there (ARCHITECTURE.md 17.5).")
            .ToList();

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    // The grid's steps are one fact too: a solution reads GridDirections' tables rather than writing
    // their steps out as literal pairs (LatticeTableCopies).
    [Fact]
    public void SolutionCollections_RestateNoGridTable()
    {
        var offences = LatticeTableCopies.In(RepositoryCompilation.Solutions)
            .Select(copy => $"{copy.Location} writes GridDirections.{copy.Table}'s steps; read the table (ARCHITECTURE.md 17.5).")
            .ToList();

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    // An entry whose copy has gone would otherwise sit in the list excusing nothing.
    [Fact]
    public void AllowedCopies_AreAllStillCopies()
        => Assert.DoesNotContain(AllowedCopies.Keys, name => !Copies.Value.Any(copy => copy.Name == name));
}
