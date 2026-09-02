using System.Text;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.FindTheSequenceOfStringsAppearedOnTheScreen;

// LeetCode 3324. Find the Sequence of Strings Appeared on the Screen: typing
// target with only two keys - append 'a', or roll the last character to its next
// letter (z wraps to a) - always appends 'a' then rolls it up to the next target
// character, since rolling any earlier character could only ever add presses.
// Return every intermediate string that appears, in order.
//
// Both strategies walk that same append-then-roll sequence one target character
// at a time; they differ only in what holds the growing on-screen string between
// key presses - a BCL StringBuilder, or this repo's own DynamicArray<char>, the
// same growable buffer Stack is built on and DataStructures.Sequence.
// DynamicArraySequence already reuses unmodified for an unrelated domain.
internal static class FindTheSequenceOfStringsAppearedOnTheScreenSolution
{
    private const char FirstLetter = 'a';
    private const char LastLetter = 'z';

    // A textbook walk over System.Text.StringBuilder.
    public static List<string> StringSequenceByStringBuilder(string target)
    {
        var screen = new List<string>();
        var onScreen = new StringBuilder();

        foreach (var targetChar in target)
        {
            onScreen.Append(FirstLetter);
            screen.Add(onScreen.ToString());

            while (onScreen[^1] != targetChar)
            {
                onScreen[^1] = NextLetter(onScreen[^1]);
                screen.Add(onScreen.ToString());
            }
        }

        return screen;
    }

    // The same append-then-roll walk, but the on-screen buffer is this repo's own
    // DynamicArray<char> - Add for a key-1 press, Set(Count - 1, ...) for a key-2
    // roll - reused unmodified from the structure Stack's growth already needed.
    public static List<string> StringSequenceByGrowableBuffer(string target)
    {
        var screen = new List<string>();
        var onScreen = new DynamicArray<char>();

        foreach (var targetChar in target)
        {
            onScreen.Add(FirstLetter);
            screen.Add(Materialize(onScreen));

            while (onScreen.Get(onScreen.Count - 1) != targetChar)
            {
                onScreen.Set(onScreen.Count - 1, NextLetter(onScreen.Get(onScreen.Count - 1)));
                screen.Add(Materialize(onScreen));
            }
        }

        return screen;
    }

    private static char NextLetter(char letter) => letter == LastLetter ? FirstLetter : (char)(letter + 1);

    private static string Materialize(DynamicArray<char> onScreen)
    {
        var chars = new char[onScreen.Count];

        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = onScreen.Get(i);
        }

        return new string(chars);
    }
}
