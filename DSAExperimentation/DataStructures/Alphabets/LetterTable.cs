using System.Collections.Immutable;

namespace DSAExperimentation.DataStructures.Alphabets;

// The layout of an alphabet whose letters are not one run of character codes: DNA's A, C, G and T,
// or Russian's а to я with ё after е. The letters are spelled out in order, and a letter's slot is
// read from a table over the codes from the lowest letter to the highest, with -1 in the gaps. A
// preset holds one in a static readonly field, built once; the JIT reads such a reference as a
// constant once the preset is initialized, so a slot costs one range check and one array read.
/// <summary>An alphabet laid out by spelling its letters out in order.</summary>
public sealed class LetterTable
{
    private const string RepeatedLetterMessage = "An alphabet's letters must each appear once.";
    private const int NoSlot = -1;

    private readonly ImmutableArray<char> _letters;
    private readonly char _lowest;
    private readonly int[] _slots;

    /// <summary>The letters, in slot order.</summary>
    public ImmutableArray<char> Letters => _letters;

    /// <summary>How many letters the alphabet has.</summary>
    public int Size => _letters.Length;

    /// <summary>Lays an alphabet out from its letters.</summary>
    /// <param name="letters">The letters in slot order, each once.</param>
    /// <exception cref="ArgumentException"><paramref name="letters"/> repeats a letter.</exception>
    public LetterTable(ImmutableArray<char> letters)
    {
        _letters = letters;
        _lowest = letters.Min();
        _slots = SlotsOf(letters, _lowest);
    }

    private static int[] SlotsOf(ImmutableArray<char> letters, char lowest)
    {
        var slots = new int[letters.Max() - lowest + 1];
        Array.Fill(slots, NoSlot);

        for (var slot = 0; slot < letters.Length; slot++)
        {
            ref var cell = ref slots[letters[slot] - lowest];

            if (cell != NoSlot)
            {
                throw new ArgumentException(RepeatedLetterMessage, nameof(letters));
            }

            cell = slot;
        }

        return slots;
    }

    /// <summary>Finds the slot a letter takes.</summary>
    /// <param name="letter">A letter of the alphabet.</param>
    /// <returns>The letter's slot.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not a letter of the alphabet.</exception>
    public int IndexOf(char letter)
    {
        if (!TryIndexOf(letter, out var index))
        {
            AlphabetFailures.ThrowNotALetter(letter);
        }

        return index;
    }

    /// <summary>Finds the slot a character takes, if it is a letter of the alphabet.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The letter's slot; meaningful only when this returns true.</param>
    /// <returns>True when <paramref name="letter"/> is a letter of the alphabet; otherwise false.</returns>
    public bool TryIndexOf(char letter, out int index)
    {
        var offset = letter - _lowest;
        index = (uint)offset < (uint)_slots.Length ? _slots[offset] : NoSlot;
        return index != NoSlot;
    }

    /// <summary>Finds the letter a slot holds.</summary>
    /// <param name="index">A slot, from 0 to <see cref="Size"/> - 1.</param>
    /// <returns>The slot's letter.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a slot of the alphabet.</exception>
    public char LetterAt(int index)
    {
        if ((uint)index >= (uint)_letters.Length)
        {
            AlphabetFailures.ThrowNotASlot(index);
        }

        return _letters[index];
    }
}
