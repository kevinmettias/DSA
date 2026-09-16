using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for NumberOfValidWordsForEachPuzzleWorkloads's nested PuzzleSource - the sibling of
// WordSourceTests, which carries the fuller account of why a nested unit needs a class named after it
// and why the only surface is the workload's own builder.
//
// A puzzle's size rule is the fixed seven letters LC 1178 requires, and that is what PuzzleSource
// owns - the source comment names the per-kind size rule as each implementation's responsibility.
// The second fact pins the part the builder-level tests leave open: the source's contract says the
// sequence of calls IS the workload, so each Next must consume the Random it was handed rather than
// answering from a cached entry.
public sealed partial class PuzzleSourceTests
{
    private const int PuzzleCount = 50;
    private const int PuzzleSeed = 1178; // LC problem number, reused as the workload seed
    private const int PuzzleLength = 7; // LC 1178: every puzzle has exactly 7 distinct letters
    private const int FirstEntryIndex = 0;
    private const int SecondEntryIndex = 1;

    [Fact]
    public void Next_EveryEntry_IsExactlySevenDistinctLetters() =>
        Assert.All(BuildPuzzles(), puzzle =>
        {
            Assert.Equal(PuzzleLength, puzzle.Length);
            Assert.Equal(PuzzleLength, puzzle.Distinct().Count());
        });

    [Fact]
    public void Next_TwoConsecutiveEntries_DifferBecauseEachDrawConsumesTheSharedRandom()
    {
        var puzzles = BuildPuzzles();

        Assert.NotEqual(puzzles[FirstEntryIndex], puzzles[SecondEntryIndex]);
    }

    private static string[] BuildPuzzles() =>
        NumberOfValidWordsForEachPuzzleWorkloads.BuildPuzzles(PuzzleCount, new Random(PuzzleSeed));
}
