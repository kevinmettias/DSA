using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Tests.DataStructures.Alphabets.Fixtures;

// Russian's 33 lowercase letters, test-only: the alphabet that proves the contract outside English
// without shipping a preset no problem reads (ARCHITECTURE §17.10). It is not one run of character
// codes: а to я is a run, but ё, the seventh letter, is coded after я, so its slot comes from a table.
internal readonly struct RussianLowercase : IAlphabet
{
    private static readonly LetterTable Table = new([.. "абвгдеёжзийклмнопрстуфхцчшщъыьэюя"]);

    public static int Size => Table.Size;

    public static int IndexOf(char letter) => Table.IndexOf(letter);

    public static bool TryIndexOf(char letter, out int index) => Table.TryIndexOf(letter, out index);

    public static char LetterAt(int index) => Table.LetterAt(index);
}
