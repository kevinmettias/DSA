using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries.Fixtures;

// An alphabet one letter too large for an alphabet trie, whose node mask has a bit per letter in 64:
// the 65 character codes from 'A' up, test-only.
internal readonly struct SixtyFiveLetters : IAlphabet
{
    private const int LetterCount = 65;

    private static LetterRun Run => new('A', LetterCount);

    public static int Size => LetterCount;

    public static int IndexOf(char letter) => Run.IndexOf(letter);

    public static bool TryIndexOf(char letter, out int index) => Run.TryIndexOf(letter, out index);

    public static char LetterAt(int index) => Run.LetterAt(index);
}
