using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for NumberOfValidWordsForEachPuzzleWorkloads (ARCHITECTURE 17.7). The reading depends
// on every generated word and puzzle having DISTINCT letters - LC 1178's own constraint - and on each
// puzzle being exactly seven letters, so the composed strategy's submask enumeration does its full work.
public sealed partial class NumberOfValidWordsForEachPuzzleWorkloadsTests
{
    private const int WordCount = 200;
    private const int PuzzleCount = 50;
    private const int WordSeed = 1178; // LC problem number, reused as the workload seed
    private const int MinWordLength = 3;
    private const int WordLengthUpperBound = 9; // exclusive; word length ranges [3, 8]
    private const int PuzzleLength = 7; // LC 1178: every puzzle has exactly 7 distinct letters

    [Fact]
    public void BuildWords_Count_ReturnsOneWordPerPosition() =>
        Assert.Equal(
            WordCount,
            NumberOfValidWordsForEachPuzzleWorkloads.BuildWords(WordCount, new Random(WordSeed)).Length);

    [Fact]
    public void BuildWords_EveryWord_HasDistinctLettersWithinTheDocumentedLengthBand() =>
        Assert.All(
            NumberOfValidWordsForEachPuzzleWorkloads.BuildWords(WordCount, new Random(WordSeed)),
            word => AssertDistinctLetters(word, MinWordLength, WordLengthUpperBound - 1));

    [Fact]
    public void BuildWords_SameSeed_ReturnsTheSameWords() =>
        Assert.Equal(
            AnswerText.Of(NumberOfValidWordsForEachPuzzleWorkloads.BuildWords(WordCount, new Random(WordSeed))),
            AnswerText.Of(NumberOfValidWordsForEachPuzzleWorkloads.BuildWords(WordCount, new Random(WordSeed))));

    [Fact]
    public void BuildPuzzles_Count_ReturnsOnePuzzlePerPosition() =>
        Assert.Equal(
            PuzzleCount,
            NumberOfValidWordsForEachPuzzleWorkloads.BuildPuzzles(PuzzleCount, new Random(WordSeed)).Length);

    [Fact]
    public void BuildPuzzles_EveryPuzzle_HasExactlySevenDistinctLetters() =>
        Assert.All(
            NumberOfValidWordsForEachPuzzleWorkloads.BuildPuzzles(PuzzleCount, new Random(WordSeed)),
            puzzle => AssertDistinctLetters(puzzle, PuzzleLength, PuzzleLength));

    [Fact]
    public void BuildPuzzles_SameSeed_ReturnsTheSamePuzzles() =>
        Assert.Equal(
            AnswerText.Of(NumberOfValidWordsForEachPuzzleWorkloads.BuildPuzzles(PuzzleCount, new Random(WordSeed))),
            AnswerText.Of(NumberOfValidWordsForEachPuzzleWorkloads.BuildPuzzles(PuzzleCount, new Random(WordSeed))));

    // The two entry kinds share one Random and `Next` consumes it once per slot, which the generator
    // names as that member's own contract: the sequence of calls IS the workload, so a caller must not
    // reorder them. Drawing the puzzles off the stream the words already consumed must therefore differ
    // from drawing them off a fresh stream - if it does not, `Next` is not advancing the Random it was
    // handed, and a recorded measurement was taken against a workload the harness cannot rebuild.
    [Fact]
    public void Next_SharedStream_AdvancesSoTheCallOrderChangesTheWorkload()
    {
        var shared = new Random(WordSeed);
        NumberOfValidWordsForEachPuzzleWorkloads.BuildWords(WordCount, shared);

        Assert.NotEqual(
            AnswerText.Of(NumberOfValidWordsForEachPuzzleWorkloads.BuildPuzzles(PuzzleCount, new Random(WordSeed))),
            AnswerText.Of(NumberOfValidWordsForEachPuzzleWorkloads.BuildPuzzles(PuzzleCount, shared)));
    }

    private static void AssertDistinctLetters(string entry, int shortest, int longest)
    {
        Assert.InRange(entry.Length, shortest, longest);
        Assert.Equal(entry.Length, entry.Distinct().Count());
    }
}
