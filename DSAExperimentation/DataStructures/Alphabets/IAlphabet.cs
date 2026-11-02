namespace DSAExperimentation.DataStructures.Alphabets;

// An alphabet as an index scheme: a fixed, ordered set of letters, each in a dense slot from 0 to
// Size - 1, so a count table, a bitmask or a trie node can index letters by slot. Which letters, in
// what order, is a fact about a writing system or a world - the English letters, DNA's bases - so the
// presets are Domain's (ARCHITECTURE §17.6), and this contract is what they fill, here in tier 1
// because AlphabetTrie reads it. A preset is a struct whose members are static abstract, so code
// generic over it is compiled for that alphabet and a slot folds to arithmetic or a table read.
//
// IndexOf and LetterAt throw outside the alphabet rather than return -1, because a wrong slot
// silently reads another letter's count. TryIndexOf asks whether a character is a letter and finds
// its slot with the one range check.
/// <summary>A fixed, ordered set of letters, each in its own slot from 0 to <see cref="Size"/> - 1.</summary>
public interface IAlphabet
{
    /// <summary>How many letters the alphabet has.</summary>
    static abstract int Size { get; }

    /// <summary>Finds the slot a letter takes.</summary>
    /// <param name="letter">A letter of the alphabet.</param>
    /// <returns>The letter's slot, from 0 to <see cref="Size"/> - 1.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not a letter of the alphabet.</exception>
    static abstract int IndexOf(char letter);

    /// <summary>Finds the slot a character takes, if it is a letter of the alphabet.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The character's slot when this returns true; meaningful only then.</param>
    /// <returns>True when <paramref name="letter"/> is a letter of the alphabet; otherwise false.</returns>
    static abstract bool TryIndexOf(char letter, out int index);

    /// <summary>Finds the letter a slot holds.</summary>
    /// <param name="index">A slot, from 0 to <see cref="Size"/> - 1.</param>
    /// <returns>The slot's letter.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a slot of the alphabet.</exception>
    static abstract char LetterAt(int index);
}
