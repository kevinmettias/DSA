using WordStack = DSAExperimentation.DataStructures.Stack.Stack<string>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IntegerToEnglishWords;

// LeetCode 273. Integer to English Words: each 3-digit group (ones, then
// "Thousand"/"Million"/"Billion") is only ever produced least-significant-
// group first via the mod/div cascade, so building the answer in reading
// order needs the group order reversed. This repo's own Stack<string> does
// that - push bottom-up, pop once into reading order - the same "build
// reversed, un-reverse through a stack" composition MultiplyStringsTests
// already uses for digit strings, just pushing whole word-groups instead of
// single digits.
public sealed partial class IntegerToEnglishWordsTests
{
    private static readonly string[] Below20 =
    [
        "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
        "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
        "Seventeen", "Eighteen", "Nineteen"
    ];

    private static readonly string[] Tens =
    [
        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    ];

    private static readonly string[] Scales = ["", "Thousand", "Million", "Billion"];

    [Theory]
    [InlineData(0, "Zero")]
    [InlineData(123, "One Hundred Twenty Three")]
    [InlineData(12345, "Twelve Thousand Three Hundred Forty Five")]
    [InlineData(1234567, "One Million Two Hundred Thirty Four Thousand Five Hundred Sixty Seven")]
    [InlineData(2147483647,
        "Two Billion One Hundred Forty Seven Million Four Hundred Eighty Three Thousand Six Hundred Forty Seven")]
    public void NumberToWords_LeetCodeExamples_ReturnsEnglishWords(int num, string expected) =>
        Assert.Equal(expected, NumberToWords(num));

    private static string NumberToWords(int num)
    {
        if (num == 0)
        {
            return "Zero";
        }

        var stack = new WordStack();
        var scale = 0;

        while (num > 0)
        {
            (num, scale) = PushGroup(stack, num, scale);
        }

        var words = new List<string>();
        while (stack.TryPop(out var word))
        {
            words.Add(word);
        }

        return string.Join(" ", words);
    }

    private static (int NextNum, int NextScale) PushGroup(WordStack stack, int num, int scale)
    {
        var group = num % 1000;
        if (group != 0)
        {
            if (Scales[scale].Length > 0)
            {
                stack.Push(Scales[scale]);
            }

            stack.Push(GroupToWords(group));
        }

        return (num / 1000, scale + 1);
    }

    private static string GroupToWords(int group)
    {
        var hundreds = group / 100;
        var remainder = group % 100;
        var parts = new List<string>();

        AppendHundreds(parts, hundreds);
        AppendRemainder(parts, remainder);

        return string.Join(" ", parts);
    }

    private static void AppendHundreds(List<string> parts, int hundreds)
    {
        if (hundreds > 0)
        {
            parts.Add(Below20[hundreds]);
            parts.Add("Hundred");
        }
    }

    private static void AppendRemainder(List<string> parts, int remainder)
    {
        if (remainder >= 20)
        {
            parts.Add(Tens[remainder / 10]);
            if (remainder % 10 > 0)
            {
                parts.Add(Below20[remainder % 10]);
            }
        }
        else if (remainder > 0)
        {
            parts.Add(Below20[remainder]);
        }
    }
}
