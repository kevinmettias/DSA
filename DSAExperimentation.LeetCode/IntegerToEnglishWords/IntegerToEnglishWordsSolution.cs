using Vocabulary = DSAExperimentation.LeetCode.IntegerToEnglishWords.IntegerToEnglishWordsVocabulary;
using WordStack = DSAExperimentation.DataStructures.Stack.Stack<string>;

namespace DSAExperimentation.LeetCode.IntegerToEnglishWords;

// LeetCode 273. Integer to English Words: each 3-digit group (ones, then
// "Thousand"/"Million"/"Billion") is only ever produced least-significant-
// group first via the mod/div cascade, so building the answer in reading
// order needs the group order reversed.
//
// The two strategies differ only in how they undo that reversal - repeated
// string prepending (the shortcut most people reach for first, reallocating
// and copying the whole result string on every group) or this repo's own
// Stack<string>, pushed bottom-up and popped once into reading order in a
// single string.Join - the same "build reversed, un-reverse through a
// stack" composition AddBinarySolution uses for carry digits.
internal static class IntegerToEnglishWordsSolution
{
    // The naive baseline: prepend each group onto the accumulated result,
    // reallocating and copying the whole string every time. What you would
    // write without this repo - plain BCL strings, no stack.
    public static string NumberToWordsByStringPrepend(int num)
    {
        if (num == 0)
        {
            return Vocabulary.ZeroWord;
        }

        var result = Vocabulary.EmptyWords;
        var scale = 0;

        while (num > 0)
        {
            result = PrependGroup(result, num % Vocabulary.GroupSize, scale);
            num /= Vocabulary.GroupSize;
            scale++;
        }

        return result;
    }

    private static string PrependGroup(string result, int group, int scale)
    {
        if (group == 0)
        {
            return result;
        }

        var groupWords = GroupToWords(group);
        var piece = Vocabulary.Scales[scale].Length > 0 ? GroupWithScale(groupWords, scale) : groupWords;
        return result.Length > 0 ? PrependPiece(new GroupPiece(piece), new AccumulatedWords(result)) : piece;
    }

    private static string GroupWithScale(string groupWords, int scale) =>
        groupWords + Vocabulary.WordSeparator + Vocabulary.Scales[scale];

    private static string PrependPiece(GroupPiece piece, AccumulatedWords result) =>
        piece.Text + Vocabulary.WordSeparator + result.Text;

    // Push each group least-significant-first onto Stack<string>; popping
    // yields them most-significant first, so the whole answer is built with
    // a single string.Join instead of n reallocating prepends.
    public static string NumberToWordsByWordStack(int num)
    {
        if (num == 0)
        {
            return Vocabulary.ZeroWord;
        }

        var stack = new WordStack();
        var scale = 0;

        while (num > 0)
        {
            PushGroup(stack, num % Vocabulary.GroupSize, scale);
            num /= Vocabulary.GroupSize;
            scale++;
        }

        var words = new List<string>();
        while (stack.TryPop(out var word))
        {
            words.Add(word);
        }

        return string.Join(Vocabulary.WordSeparator, words);
    }

    private static void PushGroup(WordStack stack, int group, int scale)
    {
        if (group == 0)
        {
            return;
        }

        if (Vocabulary.Scales[scale].Length > 0)
        {
            stack.Push(Vocabulary.Scales[scale]);
        }

        stack.Push(GroupToWords(group));
    }

    private static string GroupToWords(int group)
    {
        var hundreds = group / Vocabulary.HundredsDivisor;
        var remainder = group % Vocabulary.HundredsDivisor;
        var parts = new List<string>();

        AppendHundreds(parts, hundreds);
        AppendTensAndOnes(parts, remainder);

        return string.Join(Vocabulary.WordSeparator, parts);
    }

    private static void AppendHundreds(List<string> parts, int hundreds)
    {
        if (hundreds > 0)
        {
            parts.Add(Vocabulary.Below20[hundreds]);
            parts.Add(Vocabulary.HundredWord);
        }
    }

    private static void AppendTensAndOnes(List<string> parts, int remainder)
    {
        if (remainder >= Vocabulary.TensThreshold)
        {
            parts.Add(Vocabulary.Tens[remainder / Vocabulary.DigitBase]);
            if (remainder % Vocabulary.DigitBase > 0)
            {
                parts.Add(Vocabulary.Below20[remainder % Vocabulary.DigitBase]);
            }
        }
        else if (remainder > 0)
        {
            parts.Add(Vocabulary.Below20[remainder]);
        }
    }

    // LC 273's two concatenation ends, named for the roles they play here rather than
    // left as two adjacent `string` positions a caller could hand over the wrong way
    // round with the compiler none the wiser. `piece` is the words of the group being
    // prepended and `result` the answer accumulated so far - the concatenation is not
    // commutative, so a swap silently reverses the sentence.
    internal readonly record struct GroupPiece(string Text);

    internal readonly record struct AccumulatedWords(string Text);
}
