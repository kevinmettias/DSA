using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.BackspaceStringCompare;

// LeetCode 844. Backspace String Compare: do two keystroke strings type out the
// same text, where '#' is a backspace that deletes the previous character (and
// does nothing on empty text)?
//
// Both strategies are the same replay - push a letter, pop on '#', compare what
// survives - so the only thing they differ in is which stack does the replaying:
// the BCL's Stack<char>, or this repo's own DynamicArray-backed Stack<char>, the
// same push-on-open/pop-on-close shape ValidParentheses uses for LC 20.
internal static class BackspaceStringCompareSolution
{
    // LeetCode's backspace character.
    private const char Backspace = '#';

    // The textbook answer: BCL Stack<char> replaying each string, then
    // ToArray + Array.Reverse to read the surviving text back out in typing
    // order. Deliberately written without this repo's primitives - it is the
    // arm the composed strategy below has to justify itself against.
    public static bool IsTypedTextEqualByBclStack(string firstText, string secondText) =>
        ApplyBackspacesByBclStack(firstText) == ApplyBackspacesByBclStack(secondText);

    private static string ApplyBackspacesByBclStack(string keystrokes)
    {
        var stack = new Stack<char>();

        foreach (var character in keystrokes)
        {
            if (character == Backspace)
            {
                if (stack.Count > 0)
                {
                    stack.Pop();
                }
            }
            else
            {
                stack.Push(character);
            }
        }

        var characters = stack.ToArray();
        Array.Reverse(characters);

        return new string(characters);
    }

    // This repo's own Stack<char>: TryPop already answers "pop if there is
    // anything to pop", so the empty-text backspace LeetCode calls a no-op needs
    // no guard of its own, and the surviving text is unwound straight into a
    // right-sized buffer filled back-to-front.
    public static bool IsTypedTextEqualByStackReplay(string firstText, string secondText) =>
        ApplyBackspacesByStackReplay(firstText) == ApplyBackspacesByStackReplay(secondText);

    private static string ApplyBackspacesByStackReplay(string keystrokes)
    {
        var stack = new RepoCharStack();

        foreach (var character in keystrokes)
        {
            if (character == Backspace)
            {
                stack.TryPop(out _);
            }
            else
            {
                stack.Push(character);
            }
        }

        var characters = new char[stack.Count];

        for (var i = characters.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out characters[i]);
        }

        return new string(characters);
    }
}
