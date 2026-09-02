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
    public static string FinalStringByReversal(string s)
    {
        var chars = new List<char>();

        foreach (var c in s)
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
    public static string FinalStringByDeque(string s)
    {
        var deque = new Deque<char>();
        var appendToBack = true;

        foreach (var c in s)
        {
            if (c == 'i')
            {
                appendToBack = !appendToBack;
            }
            else if (appendToBack)
            {
                deque.PushBack(c);
            }
            else
            {
                deque.PushFront(c);
            }
        }

        return ReadOut(deque, appendToBack);
    }

    // appendToBack true means the deque's own front-to-back order already IS the
    // final string (characters were only ever pushed to the back); false means
    // the deque holds it back-to-front, so popping from the back walks it out in
    // the correct order without ever physically reversing anything.
    private static string ReadOut(Deque<char> deque, bool appendToBack)
    {
        var result = new char[deque.Count];

        for (var i = 0; i < result.Length; i++)
        {
            if (appendToBack)
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
}
