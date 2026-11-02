using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Tests.DataStructures.Alphabets.Fixtures;

// Russian's 33 capitals, test-only, in the slots of their lowercase forms: Ё is coded before А, so
// like ё it takes its slot from a table.
internal readonly struct RussianUppercase : IAlphabet
{
    private static readonly LetterTable Table = new([.. "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"]);

    public static int Size => Table.Size;

    public static int IndexOf(char letter) => Table.IndexOf(letter);

    public static bool TryIndexOf(char letter, out int index) => Table.TryIndexOf(letter, out index);

    public static char LetterAt(int index) => Table.LetterAt(index);
}
