using BenchmarkDotNet.Attributes;
using WordStack = DSAExperimentation.DataStructures.Stack.Stack<string>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Integer to English Words (LC 273): groups are only ever produced
// least-significant-scale first (mod/div cascade), so reassembling them in
// reading order needs either repeated string prepending - the shortcut most
// people reach for first, reallocating and copying the whole result string
// on every group - or this repo's own Stack<string>, pushed bottom-up and
// popped once into reading order in a single string.Join. Same "build
// reversed, un-reverse through a stack" composition MultiplyStringsBenchmarks
// already uses for digit strings. Params span a single-group number (no
// prepend ever happens) up to Int32.MaxValue (all four groups), so
// StackComposed's saved reallocations actually have groups to save on.
[MemoryDiagnoser]
public class IntegerToEnglishWordsBenchmarks
{
    private const string ZeroWord = "Zero";
    private const string HundredWord = "Hundred";
    private const string WordSeparator = " ";
    private const string EmptyWords = "";
    private const int GroupSize = 1000;
    private const int HundredsDivisor = 100;
    private const int TensThreshold = 20;
    private const int DigitBase = 10;

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

    [Params(123, 2_147_483_647)]
    public int Number;

    [Benchmark(Baseline = true)]
    public string StringPrepend() => NumberToWordsPrepend(Number);

    [Benchmark]
    public string StackComposed() => NumberToWordsStack(Number);

    private static string NumberToWordsPrepend(int num)
    {
        if (num == 0)
        {
            return ZeroWord;
        }

        var result = EmptyWords;
        var scale = 0;

        while (num > 0)
        {
            result = PrependGroup(result, num % GroupSize, scale);
            num /= GroupSize;
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
        var piece = Scales[scale].Length > 0 ? groupWords + WordSeparator + Scales[scale] : groupWords;
        return result.Length > 0 ? piece + WordSeparator + result : piece;
    }

    private static string NumberToWordsStack(int num)
    {
        if (num == 0)
        {
            return ZeroWord;
        }

        var stack = new WordStack();
        var scale = 0;

        while (num > 0)
        {
            PushGroup(stack, num % GroupSize, scale);
            num /= GroupSize;
            scale++;
        }

        var words = new List<string>();
        while (stack.TryPop(out var word))
        {
            words.Add(word);
        }

        return string.Join(WordSeparator, words);
    }

    private static void PushGroup(WordStack stack, int group, int scale)
    {
        if (group == 0)
        {
            return;
        }

        if (Scales[scale].Length > 0)
        {
            stack.Push(Scales[scale]);
        }

        stack.Push(GroupToWords(group));
    }

    private static string GroupToWords(int group)
    {
        var hundreds = group / HundredsDivisor;
        var remainder = group % HundredsDivisor;
        var parts = new List<string>();

        AppendHundreds(parts, hundreds);
        AppendTensAndOnes(parts, remainder);

        return string.Join(WordSeparator, parts);
    }

    private static void AppendHundreds(List<string> parts, int hundreds)
    {
        if (hundreds > 0)
        {
            parts.Add(Below20[hundreds]);
            parts.Add(HundredWord);
        }
    }

    private static void AppendTensAndOnes(List<string> parts, int remainder)
    {
        if (remainder >= TensThreshold)
        {
            parts.Add(Tens[remainder / DigitBase]);
            if (remainder % DigitBase > 0)
            {
                parts.Add(Below20[remainder % DigitBase]);
            }
        }
        else if (remainder > 0)
        {
            parts.Add(Below20[remainder]);
        }
    }
}
