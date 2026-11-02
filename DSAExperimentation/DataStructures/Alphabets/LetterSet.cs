using System.Collections.Immutable;
using System.Numerics;

namespace DSAExperimentation.DataStructures.Alphabets;

// A letter set read as a mask over its alphabet's slots, built once per set: membership is a slot
// lookup and a bit test. The mask is a primitive static readonly field of an instantiation over two
// structs, which the JIT reads as a constant once the type is initialized, as it reads a Life-like
// rule's masks. A mask holds 64 slots, enough for any alphabet a language writes; a larger alphabet
// fails when the mask is first read, which is the first question about a letter of the alphabet.
/// <summary>Membership in a named subset of an alphabet's letters.</summary>
/// <typeparam name="TSet">The letter set, such as a language's vowels.</typeparam>
/// <typeparam name="TAlphabet">The alphabet the set's letters belong to.</typeparam>
public static class LetterSet<TSet, TAlphabet>
    where TSet : struct, ILetterSet<TAlphabet>
    where TAlphabet : struct, IAlphabet
{
    private const int MaskSlots = 64;
    private const string TooManyLettersMessage = "A letter set's alphabet must have at most 64 letters.";

    private static readonly ulong Mask = MaskOf(TSet.Letters);

    /// <summary>How many letters the set holds.</summary>
    public static int Count => BitOperations.PopCount(Mask);

    /// <summary>Tests whether a character is one of the set's letters.</summary>
    /// <param name="letter">The character to test.</param>
    /// <returns>True when <paramref name="letter"/> is in the set; false for any other character, letter or not.</returns>
    public static bool Has(char letter) => TAlphabet.TryIndexOf(letter, out var slot) && (Mask >> slot & 1) != 0;

    private static ulong MaskOf(ImmutableArray<char> letters)
    {
        if (TAlphabet.Size > MaskSlots)
        {
            throw new InvalidOperationException(TooManyLettersMessage);
        }

        var mask = 0UL;

        foreach (var letter in letters)
        {
            mask |= 1UL << TAlphabet.IndexOf(letter);
        }

        return mask;
    }
}
