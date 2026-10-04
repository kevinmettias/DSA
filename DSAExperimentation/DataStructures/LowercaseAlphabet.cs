namespace DSAExperimentation.DataStructures;

// The 26 lowercase English letters as dense slots 0..25: LowercaseTrieNode's children are indexed by
// it, and every solution that counts letters, loops over them or packs them into a bitmask reads the
// same three facts - how many there are, which slot a letter takes, which letter a slot holds. It sits
// here beside AlgorithmConstants rather than in the trie's folder because the trie is only one of its
// readers, and in tier 1 because the trie reads it (§17.6a: the lowest tier that needs it).
//
// Not Graph/Hamming's Alphabet, which is a runtime-chosen set of letters a mutation substitutes from
// (DNA's ACGT among them); this is a fixed index scheme. IndexOf and LetterAt throw outside the
// alphabet rather than return -1, because a wrong slot silently reads another letter's count.
internal static class LowercaseAlphabet
{
    public const int Size = 26;

    private const char FirstLetter = 'a';
    private const string NotALowercaseLetterMessage = "Expected a lowercase English letter ('a'-'z').";
    private const string NotALetterSlotMessage = "Expected a letter slot in [0, 26).";

    public static int IndexOf(char letter)
    {
        var index = letter - FirstLetter;

        if (!IsSlot(index))
        {
            ThrowNotALetter(letter);
        }

        return index;
    }

    private static void ThrowNotALetter(char letter) =>
        throw new ArgumentOutOfRangeException(nameof(letter), letter, NotALowercaseLetterMessage);

    public static char LetterAt(int index)
    {
        if (!IsSlot(index))
        {
            ThrowNotASlot(index);
        }

        return (char)(FirstLetter + index);
    }

    private static void ThrowNotASlot(int index) =>
        throw new ArgumentOutOfRangeException(nameof(index), index, NotALetterSlotMessage);

    private static bool IsSlot(int index) => (uint)index < Size;
}
