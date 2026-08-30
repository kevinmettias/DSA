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
            return "Zero";
        }

        var result = "";
        var scale = 0;

        while (num > 0)
        {
            var group = num % 1000;
            if (group != 0)
            {
                var groupWords = GroupToWords(group);
                var piece = Scales[scale].Length > 0 ? groupWords + " " + Scales[scale] : groupWords;
                result = result.Length > 0 ? piece + " " + result : piece;
            }

            num /= 1000;
            scale++;
        }

        return result;
    }

    private static string NumberToWordsStack(int num)
    {
        if (num == 0)
        {
            return "Zero";
        }

        var stack = new WordStack();
        var scale = 0;

        while (num > 0)
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

            num /= 1000;
            scale++;
        }

        var words = new List<string>();
        while (stack.TryPop(out var word))
        {
            words.Add(word);
        }

        return string.Join(" ", words);
    }

    private static string GroupToWords(int group)
    {
        var hundreds = group / 100;
        var remainder = group % 100;
        var parts = new List<string>();

        if (hundreds > 0)
        {
            parts.Add(Below20[hundreds]);
            parts.Add("Hundred");
        }

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

        return string.Join(" ", parts);
    }
}
