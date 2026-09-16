using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.LeetCode.FaultyKeyboard;

// LeetCode 2810. Faulty Keyboard: typing 'i' reverses everything typed so far;
// every other character is appended normally. Return the string finally left on
// screen.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class FaultyKeyboardSolution
{
    // The textbook approach: physically reverse the buffer built so far every time
    // 'i' is typed. Correct, but each reversal costs O(current length), so a
    // string with many 'i's spread through it degrades toward O(n^2).
    public static string FinalStringByReversal(string keystrokes)
    {
        var chars = new List<char>();

        foreach (var c in keystrokes)
        {
            if (c == 'i')
            {
                chars.Reverse();
            }
            else
            {
                chars.Add(c);
            }
        }

        return new string(chars.ToArray());
    }

    // One O(n) pass over this repo's own Deque<char>: 'i' never touches the
    // buffer at all, it just flips which end subsequent characters get appended
    // to, so the "reversal" costs nothing. Direction only matters once, at the
    // very end, when the buffer is read back out.
    public static string FinalStringByDeque(string keystrokes)
    {
        var deque = new Deque<char>();
        var order = ReadDirection.Forward;

        foreach (var c in keystrokes)
        {
            if (c == 'i')
            {
                order = order == ReadDirection.Forward ? ReadDirection.Backward : ReadDirection.Forward;
            }
            else if (order == ReadDirection.Forward)
            {
                deque.PushBack(c);
            }
            else
            {
                deque.PushFront(c);
            }
        }

        return ReadOut(deque, order);
    }

    // ReadDirection.Forward means the deque's own front-to-back order already IS the
    // final string (characters were only ever pushed to the back); Backward means
    // the deque holds it back-to-front, so popping from the back walks it out in
    // the correct order without ever physically reversing anything.
    private static string ReadOut(Deque<char> deque, ReadDirection order)
    {
        var result = new char[deque.Count];

        for (var i = 0; i < result.Length; i++)
        {
            if (order == ReadDirection.Forward)
            {
                deque.TryPopFront(out result[i]);
            }
            else
            {
                deque.TryPopBack(out result[i]);
            }
        }

        return new string(result);
    }

    // Which way the deque reads back out is a state and not a flag: an 'i' flips it,
    // and every other character is then appended at whichever end keeps that reading
    // order - named so the call site says which one it wants rather than `true`.
    private enum ReadDirection
    {
        // The deque's front-to-back order is the final string.
        Forward,

        // The deque holds the string back-to-front.
        Backward,
    }
}
