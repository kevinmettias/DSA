using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Language.English;

// The 26 English capitals as slots 0 to 25, as LC 424's strings and LC 621's tasks are written: their
// count tables index capitals by it. Like the lowercase letters they are one run of character codes,
// from 'A', and there are EnglishLetters.Size of them. As the lowercase run's are, its two ends are
// constants, the last one derived from the first and the count, so a loop between them or a slot
// taken as letter - FirstLetter folds to the literals.
/// <summary>The 26 English capitals, 'A' in slot 0 through 'Z' in slot 25.</summary>
public readonly struct EnglishUppercase : IAlphabet
{
    /// <summary>The first capital, 'A', in slot 0.</summary>
    public const char FirstLetter = 'A';

    /// <summary>The last capital, 'Z', in slot 25.</summary>
    public const char LastLetter = (char)(FirstLetter + EnglishLetters.Size - 1);

    private static LetterRun Run => new(FirstLetter, EnglishLetters.Size);

    static int IAlphabet.Size => EnglishLetters.Size;

    /// <summary>Finds the slot a capital takes.</summary>
    /// <param name="letter">An English capital, 'A' through 'Z'.</param>
    /// <returns>The letter's slot, 0 for 'A' through 25 for 'Z'.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is outside 'A' through 'Z'.</exception>
    public static int IndexOf(char letter) => Run.IndexOf(letter);

    /// <summary>Finds the slot a character takes, if it is an English capital.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The letter's slot; meaningful only when this returns true.</param>
    /// <returns>True for 'A' through 'Z'; otherwise false.</returns>
    public static bool TryIndexOf(char letter, out int index) => Run.TryIndexOf(letter, out index);

    /// <summary>Finds the capital a slot holds.</summary>
    /// <param name="index">A slot, 0 through 25.</param>
    /// <returns>The slot's letter, 'A' for 0 through 'Z' for 25.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside [0, 26).</exception>
    public static char LetterAt(int index) => Run.LetterAt(index);
}
