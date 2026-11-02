namespace DSAExperimentation.DataStructures.Alphabets;

// The layout of an alphabet whose letters are one unbroken run of character codes, as 'a' to 'z'
// are: a letter's slot is its distance from the first. A preset builds one from two constants in
// each member it forwards, so the JIT inlines the run and folds both into the caller. Most of the
// world's alphabets are not such a run - Greek's final sigma sits inside α to ω, Russian's ё lies
// outside а to я - and those are tabled instead.
/// <summary>An alphabet laid out as a run of consecutive character codes.</summary>
/// <param name="first">The first letter, which takes slot 0.</param>
/// <param name="size">How many letters the run holds.</param>
public readonly struct LetterRun(char first, int size)
{
    private const string NotALetterMessage = "Expected a letter of the alphabet.";
    private const string NotASlotMessage = "Expected a slot of the alphabet.";

    /// <summary>The first letter, which takes slot 0.</summary>
    public char First { get; } = first;

    /// <summary>How many letters the run holds.</summary>
    public int Size { get; } = size;

    /// <summary>Finds the slot a letter takes.</summary>
    /// <param name="letter">A letter of the run.</param>
    /// <returns>The letter's distance from <see cref="First"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is outside the run.</exception>
    public int IndexOf(char letter)
    {
        if (!TryIndexOf(letter, out var index))
        {
            ThrowNotALetter(letter);
        }

        return index;
    }

    private static void ThrowNotALetter(char letter) =>
        throw new ArgumentOutOfRangeException(nameof(letter), letter, NotALetterMessage);

    /// <summary>Finds the slot a character takes, if it lies in the run.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The character's distance from <see cref="First"/>; meaningful only when this returns true.</param>
    /// <returns>True when <paramref name="letter"/> lies in the run; otherwise false.</returns>
    public bool TryIndexOf(char letter, out int index)
    {
        index = letter - First;
        return IsSlot(index);
    }

    /// <summary>Finds the letter a slot holds.</summary>
    /// <param name="index">A slot, from 0 to <see cref="Size"/> - 1.</param>
    /// <returns>The letter <paramref name="index"/> places after <see cref="First"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the run.</exception>
    public char LetterAt(int index)
    {
        if (!IsSlot(index))
        {
            ThrowNotASlot(index);
        }

        return (char)(First + index);
    }

    private static void ThrowNotASlot(int index) =>
        throw new ArgumentOutOfRangeException(nameof(index), index, NotASlotMessage);

    private bool IsSlot(int index) => (uint)index < (uint)Size;
}
