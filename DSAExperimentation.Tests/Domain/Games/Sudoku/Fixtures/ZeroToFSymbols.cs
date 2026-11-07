using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku.Fixtures;

// One way puzzles write a 16x16 board's sixteen symbols, test-only: the hexadecimal digits '0' to
// '9' then 'A' to 'F'. SixteenBySixteenSudoku fixes no alphabet, since puzzles disagree.
internal readonly struct ZeroToFSymbols : IAlphabet
{
    private static readonly LetterTable Table = new([.. "0123456789ABCDEF"]);

    public static int Size => Table.Size;

    public static int IndexOf(char letter) => Table.IndexOf(letter);

    public static bool TryIndexOf(char letter, out int index) => Table.TryIndexOf(letter, out index);

    public static char LetterAt(int index) => Table.LetterAt(index);
}
