using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Mathematics.Numerals;

// The ten decimal digits, '0' to '9', as an alphabet: a digit's slot is its value, since the digits
// are one run of character codes from '0'. A lock's wheels carry them, and a numeral is written in
// them. Size stays a constant for a caller that needs one; the interface reads it.
/// <summary>The decimal digits, '0' in slot 0 through '9' in slot 9.</summary>
public readonly struct DecimalDigits : IAlphabet
{
    /// <summary>The number of digits, 10.</summary>
    public const int Size = 10;

    /// <summary>The digit zero, in slot 0.</summary>
    public const char Zero = '0';

    /// <summary>The digit nine, in the last slot.</summary>
    public const char Nine = '9';

    private static LetterRun Run => new(Zero, Size);

    static int IAlphabet.Size => Size;

    /// <summary>Finds a digit's slot, which is its value.</summary>
    /// <param name="letter">A decimal digit, '0' through '9'.</param>
    /// <returns>The digit's value, 0 through 9.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not a decimal digit.</exception>
    public static int IndexOf(char letter) => Run.IndexOf(letter);

    /// <summary>Finds a character's slot, if it is a decimal digit.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The digit's value; meaningful only when this returns true.</param>
    /// <returns>True for '0' through '9'; otherwise false.</returns>
    public static bool TryIndexOf(char letter, out int index) => Run.TryIndexOf(letter, out index);

    /// <summary>Finds the digit with a value.</summary>
    /// <param name="index">A value, 0 through 9.</param>
    /// <returns>The digit, '0' through '9'.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside [0, 10).</exception>
    public static char LetterAt(int index) => Run.LetterAt(index);
}
