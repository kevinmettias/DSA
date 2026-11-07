using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Mathematics.Numerals;

// The two binary digits, '0' and '1', as an alphabet: a digit's slot is its value, since the digits
// are one run of character codes from '0', as the decimal ones are. Every bit of a machine integer
// is one of them, so a trie over an int's bits keeps a child for each binary digit, Size of them,
// and reads a bit's value as that digit's slot. Size stays a constant for a caller that needs one;
// the interface reads it.
/// <summary>The binary digits, '0' in slot 0 and '1' in slot 1.</summary>
public readonly struct BinaryDigits : IAlphabet
{
    /// <summary>The number of digits, 2.</summary>
    public const int Size = 2;

    /// <summary>The digit zero, in slot 0.</summary>
    public const char Zero = '0';

    /// <summary>The digit one, in the last slot.</summary>
    public const char One = '1';

    private static LetterRun Run => new(Zero, Size);

    static int IAlphabet.Size => Size;

    /// <summary>Finds a digit's slot, which is its value.</summary>
    /// <param name="letter">A binary digit, '0' or '1'.</param>
    /// <returns>The digit's value, 0 or 1.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not a binary digit.</exception>
    public static int IndexOf(char letter) => Run.IndexOf(letter);

    /// <summary>Finds a character's slot, if it is a binary digit.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The digit's value; meaningful only when this returns true.</param>
    /// <returns>True for '0' and '1'; otherwise false.</returns>
    public static bool TryIndexOf(char letter, out int index) => Run.TryIndexOf(letter, out index);

    /// <summary>Finds the digit with a value.</summary>
    /// <param name="index">A value, 0 or 1.</param>
    /// <returns>The digit, '0' or '1'.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside [0, 2).</exception>
    public static char LetterAt(int index) => Run.LetterAt(index);
}
