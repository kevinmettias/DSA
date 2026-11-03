using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Games.Sudoku;

// Classic Sudoku's nine symbols, the digits '1' to '9', as an alphabet: '1' in slot 0 through '9' in
// slot 8. There are as many as the board is wide (ClassicSudoku.Size). A 16x16 board writes its
// sixteen symbols differently, so the digits are the classic grid's, not every grid's.
/// <summary>Classic Sudoku's digits, '1' in slot 0 through '9' in slot 8.</summary>
public readonly struct SudokuDigits : IAlphabet
{
    private const char FirstDigit = '1';

    private static LetterRun Run => new(FirstDigit, ClassicSudoku.Size);

    /// <summary>The number of digits, 9.</summary>
    public static int Size => ClassicSudoku.Size;

    /// <summary>Finds the slot a digit takes.</summary>
    /// <param name="letter">A Sudoku digit, '1' through '9'.</param>
    /// <returns>The digit's slot, 0 for '1' through 8 for '9'.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not a Sudoku digit.</exception>
    public static int IndexOf(char letter) => Run.IndexOf(letter);

    /// <summary>Finds the slot a character takes, if it is a Sudoku digit.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The digit's slot; meaningful only when this returns true.</param>
    /// <returns>True for '1' through '9'; otherwise false.</returns>
    public static bool TryIndexOf(char letter, out int index) => Run.TryIndexOf(letter, out index);

    /// <summary>Finds the digit a slot holds.</summary>
    /// <param name="index">A slot, 0 through 8.</param>
    /// <returns>The digit, '1' for 0 through '9' for 8.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside [0, 9).</exception>
    public static char LetterAt(int index) => Run.LetterAt(index);
}
