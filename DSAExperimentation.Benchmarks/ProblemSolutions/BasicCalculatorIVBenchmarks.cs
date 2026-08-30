using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Basic Calculator IV (LC 770): the identical recursive-descent symbolic-polynomial
// evaluator run over a sum of Length distinct variables (no evalvars supplied, so
// every term survives to the final, sorted output) - DictionaryPolynomial
// accumulates/sorts with a plain BCL Dictionary<string,long> + List.Sort,
// HashMapMergeSort accumulates with this repo's own HashMap<TKey,TValue> and sorts
// the final term list with MergeSort over ArrayIndexedSequence<Term> - the same
// "same algorithm, repo primitive vs. BCL equivalent" comparison
// BasicCalculatorBenchmarks already uses for LC 224's Stack<(int,int)>.
[MemoryDiagnoser]
public class BasicCalculatorIVBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup() => _expression = BuildExpression(Length);

    [Benchmark(Baseline = true)]
    public int DictionaryPolynomial()
    {
        var pos = 0;
        var polynomial = ParseExpressionDict(_expression, ref pos);
        var terms = new List<(string Key, long Coefficient)>();

        foreach (var (key, coefficient) in polynomial)
        {
            if (coefficient != 0)
            {
                terms.Add((key, coefficient));
            }
        }

        terms.Sort((a, b) =>
        {
            var degreeA = Degree(a.Key);
            var degreeB = Degree(b.Key);
            return degreeA != degreeB ? degreeB.CompareTo(degreeA) : string.CompareOrdinal(a.Key, b.Key);
        });

        return terms.Count;
    }

    [Benchmark]
    public int HashMapMergeSort()
    {
        var pos = 0;
        var polynomial = ParseExpressionHashMap(_expression, ref pos);
        var terms = new List<Term>();

        foreach (var key in polynomial.Keys)
        {
            polynomial.TryGetValue(key, out var coefficient);

            if (coefficient != 0)
            {
                terms.Add(new Term(key, coefficient));
            }
        }

        var termsArray = terms.ToArray();
        var comparer = Comparer<Term>.Create((a, b) =>
        {
            var degreeA = Degree(a.Key);
            var degreeB = Degree(b.Key);
            return degreeA != degreeB ? degreeB.CompareTo(degreeA) : string.CompareOrdinal(a.Key, b.Key);
        });

        MergeSort.Sort<Term, ArrayIndexedSequence<Term>>(new ArrayIndexedSequence<Term>(termsArray), comparer);

        return termsArray.Length;
    }

    private readonly record struct Term(string Key, long Coefficient);

    private static int Degree(string key) => key.Length == 0 ? 0 : key.Split('*').Length;

    // ---- Dictionary-backed parse/eval (baseline) ----

    private static Dictionary<string, long> ParseExpressionDict(string expr, ref int pos)
    {
        var result = ParseTermDict(expr, ref pos);

        while (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-'))
        {
            var op = expr[pos];
            pos++;
            var rhs = ParseTermDict(expr, ref pos);
            result = op == '+' ? AddDict(result, rhs) : AddDict(result, NegateDict(rhs));
        }

        return result;
    }

    private static Dictionary<string, long> ParseTermDict(string expr, ref int pos)
    {
        var result = ParseFactorDict(expr, ref pos);

        while (pos < expr.Length && expr[pos] == '*')
        {
            pos++;
            var rhs = ParseFactorDict(expr, ref pos);
            result = MultiplyDict(result, rhs);
        }

        return result;
    }

    private static Dictionary<string, long> ParseFactorDict(string expr, ref int pos)
    {
        if (expr[pos] == '(')
        {
            pos++;
            var inner = ParseExpressionDict(expr, ref pos);
            pos++;
            return inner;
        }

        if (char.IsDigit(expr[pos]))
        {
            var start = pos;

            while (pos < expr.Length && char.IsDigit(expr[pos]))
            {
                pos++;
            }

            return new Dictionary<string, long> { [string.Empty] = long.Parse(expr[start..pos]) };
        }

        var startVar = pos;

        while (pos < expr.Length && char.IsLower(expr[pos]))
        {
            pos++;
        }

        return new Dictionary<string, long> { [expr[startVar..pos]] = 1L };
    }

    private static Dictionary<string, long> AddDict(Dictionary<string, long> a, Dictionary<string, long> b)
    {
        var result = new Dictionary<string, long>(a);

        foreach (var (key, value) in b)
        {
            result[key] = result.GetValueOrDefault(key) + value;
        }

        return result;
    }

    private static Dictionary<string, long> NegateDict(Dictionary<string, long> a)
    {
        var result = new Dictionary<string, long>();

        foreach (var (key, value) in a)
        {
            result[key] = -value;
        }

        return result;
    }

    private static Dictionary<string, long> MultiplyDict(Dictionary<string, long> a, Dictionary<string, long> b)
    {
        var result = new Dictionary<string, long>();

        foreach (var (keyA, coeffA) in a)
        {
            foreach (var (keyB, coeffB) in b)
            {
                var mergedKey = MergeVariables(keyA, keyB);
                result[mergedKey] = result.GetValueOrDefault(mergedKey) + (coeffA * coeffB);
            }
        }

        return result;
    }

    // ---- HashMap-backed parse/eval (repo primitive) ----

    private static HashMap<string, long> ParseExpressionHashMap(string expr, ref int pos)
    {
        var result = ParseTermHashMap(expr, ref pos);

        while (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-'))
        {
            var op = expr[pos];
            pos++;
            var rhs = ParseTermHashMap(expr, ref pos);
            result = op == '+' ? AddHashMap(result, rhs) : AddHashMap(result, NegateHashMap(rhs));
        }

        return result;
    }

    private static HashMap<string, long> ParseTermHashMap(string expr, ref int pos)
    {
        var result = ParseFactorHashMap(expr, ref pos);

        while (pos < expr.Length && expr[pos] == '*')
        {
            pos++;
            var rhs = ParseFactorHashMap(expr, ref pos);
            result = MultiplyHashMap(result, rhs);
        }

        return result;
    }

    private static HashMap<string, long> ParseFactorHashMap(string expr, ref int pos)
    {
        if (expr[pos] == '(')
        {
            pos++;
            var inner = ParseExpressionHashMap(expr, ref pos);
            pos++;
            return inner;
        }

        var single = new HashMap<string, long>();

        if (char.IsDigit(expr[pos]))
        {
            var start = pos;

            while (pos < expr.Length && char.IsDigit(expr[pos]))
            {
                pos++;
            }

            single.Set(string.Empty, long.Parse(expr[start..pos]));
            return single;
        }

        var startVar = pos;

        while (pos < expr.Length && char.IsLower(expr[pos]))
        {
            pos++;
        }

        single.Set(expr[startVar..pos], 1L);
        return single;
    }

    private static HashMap<string, long> AddHashMap(HashMap<string, long> a, HashMap<string, long> b)
    {
        var result = new HashMap<string, long>();

        foreach (var key in a.Keys)
        {
            a.TryGetValue(key, out var value);
            result.Set(key, value);
        }

        foreach (var key in b.Keys)
        {
            b.TryGetValue(key, out var value);
            result.TryGetValue(key, out var existing);
            result.Set(key, existing + value);
        }

        return result;
    }

    private static HashMap<string, long> NegateHashMap(HashMap<string, long> a)
    {
        var result = new HashMap<string, long>();

        foreach (var key in a.Keys)
        {
            a.TryGetValue(key, out var value);
            result.Set(key, -value);
        }

        return result;
    }

    private static HashMap<string, long> MultiplyHashMap(HashMap<string, long> a, HashMap<string, long> b)
    {
        var result = new HashMap<string, long>();

        foreach (var keyA in a.Keys)
        {
            a.TryGetValue(keyA, out var coeffA);

            foreach (var keyB in b.Keys)
            {
                b.TryGetValue(keyB, out var coeffB);
                var mergedKey = MergeVariables(keyA, keyB);
                result.TryGetValue(mergedKey, out var existing);
                result.Set(mergedKey, existing + (coeffA * coeffB));
            }
        }

        return result;
    }

    private static string MergeVariables(string a, string b)
    {
        if (a.Length == 0)
        {
            return b;
        }

        if (b.Length == 0)
        {
            return a;
        }

        var parts = new List<string>(a.Split('*'));
        parts.AddRange(b.Split('*'));
        parts.Sort(StringComparer.Ordinal);
        return string.Join('*', parts);
    }

    private static string BuildExpression(int length)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            if (i > 0)
            {
                builder.Append('+');
            }

            builder.Append(VariableName(i));
        }

        return builder.ToString();
    }

    // Base-26 "spreadsheet column" naming (a, b, ..., z, aa, ab, ...) so every
    // generated name is valid per LC 770's lowercase-letters-only variable grammar,
    // however large Length grows.
    private static string VariableName(int index)
    {
        var chars = new List<char>();
        var n = index;

        do
        {
            chars.Insert(0, (char)('a' + (n % 26)));
            n = (n / 26) - 1;
        } while (n >= 0);

        return new string(chars.ToArray());
    }
}
