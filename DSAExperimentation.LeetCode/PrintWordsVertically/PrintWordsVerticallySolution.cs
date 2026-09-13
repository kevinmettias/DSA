using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.PrintWordsVertically;

// LeetCode 1324. Print Words Vertically: read the sentence's words down their
// columns instead of across, padding short words with spaces and dropping the
// trailing spaces every column picks up on the way.
//
// Both strategies build one output row per character column and differ only in
// how that row is trimmed: the textbook arm materializes the padded row as a
// string and calls TrimEnd, while the composed arm trims in place by popping the
// tail of this repo's own DynamicArray<char> before the string is ever built.
internal static class PrintWordsVerticallySolution
{
    private const char Padding = ' ';

    // The textbook answer: a List<char> per column, turned into a string and
    // TrimEnd'ed - one padded string allocated and thrown away per row.
    // Deliberately written without this repo's primitives (the word container it
    // is handed aside) - it is the arm the DynamicArray walk below has to justify
    // itself against.
    public static List<string> PrintVerticallyByListTrimEnd(string s) =>
        PrintVerticallyByListTrimEnd(SplitWords(s));

    public static List<string> PrintVerticallyByListTrimEnd(DynamicArray<string> words)
    {
        var rows = new List<string>();
        var columns = LongestWordLength(words);

        for (var column = 0; column < columns; column++)
        {
            var chars = new List<char>();

            for (var i = 0; i < words.Count; i++)
            {
                chars.Add(CharacterAt(words.Get(i), column));
            }

            rows.Add(new string([.. chars]).TrimEnd(Padding));
        }

        return rows;
    }

    // This repo's own DynamicArray<char> - the same growable char buffer
    // StreamOfCharacters uses - earns its place here because trimming needs both
    // indexed Get (to read the last character) and O(1) removal from the end: the
    // padding is popped off the tail before the row's string exists, so no padded
    // string is ever allocated.
    public static List<string> PrintVerticallyByDynamicArrayColumns(string s) =>
        PrintVerticallyByDynamicArrayColumns(SplitWords(s));

    public static List<string> PrintVerticallyByDynamicArrayColumns(DynamicArray<string> words)
    {
        var rows = new List<string>();
        var columns = LongestWordLength(words);

        for (var column = 0; column < columns; column++)
        {
            rows.Add(BuildColumn(words, column));
        }

        return rows;
    }

    private static string BuildColumn(DynamicArray<string> words, int column)
    {
        var buffer = new DynamicArray<char>();

        for (var i = 0; i < words.Count; i++)
        {
            buffer.Add(CharacterAt(words.Get(i), column));
        }

        while (buffer.Count > 0 && buffer.Get(buffer.Count - 1) == Padding)
        {
            buffer.RemoveAt(buffer.Count - 1);
        }

        var chars = new char[buffer.Count];

        for (var i = 0; i < buffer.Count; i++)
        {
            chars[i] = buffer.Get(i);
        }

        return new string(chars);
    }

    // A word that has run out of characters contributes a space to the column,
    // which the trim at the end of the row may or may not survive.
    private static char CharacterAt(string word, int column) =>
        column < word.Length ? word[column] : Padding;

    // LeetCode hands the sentence in as one space-separated string; every strategy
    // works over the words, so the split is done once here and the prepared-words
    // overloads let a benchmark hoist it out of the measured method entirely.
    private static DynamicArray<string> SplitWords(string s)
    {
        var words = new DynamicArray<string>();

        foreach (var word in s.Split(Padding, StringSplitOptions.RemoveEmptyEntries))
        {
            words.Add(word);
        }

        return words;
    }

    // The number of output rows: the sentence is as tall as its longest word.
    private static int LongestWordLength(DynamicArray<string> words)
    {
        var longest = 0;

        for (var i = 0; i < words.Count; i++)
        {
            longest = Math.Max(longest, words.Get(i).Length);
        }

        return longest;
    }
}
