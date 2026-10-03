using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for WordSearchWorkloads (ARCHITECTURE 17.7). LC 79's benchmark pins both arms to
// false, and relies on them getting deep before they fail. Both rest on the fixture's comment: the
// word's last letter appears nowhere on the board, and every letter before it is read off a walk
// across the board. The fixture hands that walk back, so the second claim is checked by following
// it: one cell per letter, no cell used twice, each step to an orthogonal neighbour, and the board's
// letters along it spelling the word up to its last letter. The sizes are LeetCode's caps - a 6 x 6
// board and a 15-letter word.
public sealed partial class WordSearchWorkloadsTests
{
    private const int Side = 6;
    private const int WordLength = 15;
    private const int Seed = 79; // LC problem number
    private const string Alphabet = "AB";

    [Fact]
    public void Build_Side_ReturnsASquareBoardOverTheNarrowAlphabet()
    {
        var board = Build().Board;

        Assert.Equal(Side, board.Length);
        Assert.All(board, row => Assert.Equal(Side, row.Length));
        Assert.All(board.SelectMany(row => row), letter => Assert.Contains(letter, Alphabet));
    }

    [Fact]
    public void Build_LastLetter_AppearsNowhereOnTheBoard()
    {
        var (board, word, _) = Build();

        Assert.Equal(WordLength, word.Length);
        Assert.Equal(WordSearchWorkloads.AbsentLetter, word[^1]);
        Assert.DoesNotContain(WordSearchWorkloads.AbsentLetter, board.SelectMany(row => row));
    }

    [Fact]
    public void Build_Path_IsADistinctOrthogonalWalkSpellingTheWordBeforeItsLastLetter()
    {
        var (board, word, path) = Build();

        Assert.Equal(word.Length - 1, path.Length);
        Assert.Equal(path.Length, path.Distinct().Count());
        Assert.All(
            path.Zip(path.Skip(1)),
            step => Assert.Equal(1, Math.Abs(step.First.Row - step.Second.Row) + Math.Abs(step.First.Column - step.Second.Column)));
        Assert.Equal(word[..^1], new string([.. path.Select(cell => board[cell.Row][cell.Column])]));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameBoardAndWord()
    {
        var first = Build();
        var second = Build();

        Assert.Equal(first.Board, second.Board);
        Assert.Equal(first.Word, second.Word);
    }

    private static (char[][] Board, string Word, (int Row, int Column)[] Path) Build() =>
        WordSearchWorkloads.Build(Side, WordLength, new Random(Seed));
}
