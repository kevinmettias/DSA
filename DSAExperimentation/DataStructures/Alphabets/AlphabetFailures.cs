namespace DSAExperimentation.DataStructures.Alphabets;

// The two ways a layout refuses a lookup, worded once for every layout: a character that is not one
// of the alphabet's letters, and an index that is not one of its slots.
internal static class AlphabetFailures
{
    private const string NotALetterMessage = "Expected a letter of the alphabet.";
    private const string NotASlotMessage = "Expected a slot of the alphabet.";

    public static void ThrowNotALetter(char letter) =>
        throw new ArgumentOutOfRangeException(nameof(letter), letter, NotALetterMessage);

    public static void ThrowNotASlot(int index) =>
        throw new ArgumentOutOfRangeException(nameof(index), index, NotASlotMessage);
}
