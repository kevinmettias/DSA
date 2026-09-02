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
    // Base-26 "spreadsheet column" alphabet size (a-z).
    private const int AlphabetSize = 26;

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
        var terms = CollectNonZeroTermsDict(polynomial);

        terms.Sort((a, b) => CompareByDegreeThenKey(a.Key, b.Key));

        return terms.Count;
    }

    [Benchmark]
    public int HashMapMergeSort()
    {
        var pos = 0;
        var polynomial = ParseExpressionHashMap(_expression, ref pos);
        var terms = CollectNonZeroTermsHashMap(polynomial);
        var termsArray = terms.ToArray();

        SortTerms(termsArray);

        return termsArray.Length;
    }

    private static List<(string Key, long Coefficient)> CollectNonZeroTermsDict(Dictionary<string, long> polynomial)
    {
        var terms = new List<(string Key, long Coefficient)>();

        foreach (var (key, coefficient) in polynomial)
        {
            if (coefficient != 0)
            {
                terms.Add((key, coefficient));
            }
        }

        return terms;
    }

    private static List<Term> CollectNonZeroTermsHashMap(HashMap<string, long> polynomial)
    {
        var terms = new List<Term>();

        foreach (var key in polynomial.Keys)
        {
            polynomial.TryGetValue(key, out var coefficient);

            if (coefficient != 0)
            {
                terms.Add(new Term(key, coefficient));
            }
        }

        return terms;
    }

    private static void SortTerms(Term[] termsArray)
    {
        var comparer = Comparer<Term>.Create((a, b) => CompareByDegreeThenKey(a.Key, b.Key));
        MergeSort.Sort<Term, ArrayIndexedSequence<Term>>(new ArrayIndexedSequence<Term>(termsArray), comparer);
    }

    private static int CompareByDegreeThenKey(string keyA, string keyB)
    {
        var degreeA = Degree(keyA);
        var degreeB = Degree(keyB);
        return degreeA != degreeB ? degreeB.CompareTo(degreeA) : string.CompareOrdinal(keyA, keyB);
    }

    private readonly record struct Term(string Key, long Coefficient);

    private static int Degree(string key) => key.Length == 0 ? 0 : key.Split('*').Length;

    // ---- Generic recursive-descent polynomial parser ----
    //
    // Both the Dictionary-backed and HashMap-backed evaluators walk the exact same
    // grammar (expression := term (('+'|'-') term)*, term := factor ('*' factor)*,
    // factor := '(' expression ')' | number | variable) and differ only in which
    // collection type carries the running polynomial and how that collection
    // implements add/negate/multiply/single-term construction. That single point of
    // variation is bundled into PolynomialOps<T> and threaded through one shared
    // parser core instead of keeping two structurally identical parsers in sync.

    private readonly record struct PolynomialOps<T>(
        Func<T, T, T> Add,
        Func<T, T> Negate,
        Func<T, T, T> Multiply,
        Func<string, long, T> FromKeyCoefficient);

    private static T ParseExpressionCore<T>(string expr, ref int pos, PolynomialOps<T> ops)
    {
        var result = ParseTermCore(expr, ref pos, ops);

        while (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-'))
        {
            var op = expr[pos];
            pos++;
            var rhs = ParseTermCore(expr, ref pos, ops);
            result = op == '+' ? ops.Add(result, rhs) : ops.Add(result, ops.Negate(rhs));
        }

        return result;
    }

    private static T ParseTermCore<T>(string expr, ref int pos, PolynomialOps<T> ops)
    {
        var result = ParseFactorCore(expr, ref pos, ops);

        while (pos < expr.Length && expr[pos] == '*')
        {
            pos++;
            var rhs = ParseFactorCore(expr, ref pos, ops);
            result = ops.Multiply(result, rhs);
        }

        return result;
    }

    private static T ParseFactorCore<T>(string expr, ref int pos, PolynomialOps<T> ops)
    {
        if (expr[pos] == '(')
        {
            return ParseParenthesized(expr, ref pos, ops);
        }

        if (char.IsDigit(expr[pos]))
        {
            return ParseNumber(expr, ref pos, ops);
        }

        return ParseVariable(expr, ref pos, ops);
    }

    private static T ParseParenthesized<T>(string expr, ref int pos, PolynomialOps<T> ops)
    {
        pos++;
        var inner = ParseExpressionCore(expr, ref pos, ops);
        pos++;
        return inner;
    }

    private static T ParseNumber<T>(string expr, ref int pos, PolynomialOps<T> ops)
    {
        var start = pos;

        while (pos < expr.Length && char.IsDigit(expr[pos]))
        {
            pos++;
        }

        return ops.FromKeyCoefficient(string.Empty, long.Parse(expr[start..pos]));
    }

    private static T ParseVariable<T>(string expr, ref int pos, PolynomialOps<T> ops)
    {
        var startVar = pos;

        while (pos < expr.Length && char.IsLower(expr[pos]))
        {
            pos++;
        }

        return ops.FromKeyCoefficient(expr[startVar..pos], 1L);
    }

    // ---- Dictionary-backed operations (baseline) ----

    private static readonly PolynomialOps<Dictionary<string, long>> DictOps = new(
        AddDict,
        NegateDict,
        MultiplyDict,
        (key, coefficient) => new Dictionary<string, long> { [key] = coefficient });

    private static Dictionary<string, long> ParseExpressionDict(string expr, ref int pos) =>
        ParseExpressionCore(expr, ref pos, DictOps);

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

    // ---- HashMap-backed operations (repo primitive) ----

    private static readonly PolynomialOps<HashMap<string, long>> HashMapOps = new(
        AddHashMap,
        NegateHashMap,
        MultiplyHashMap,
        (key, coefficient) =>
        {
            var single = new HashMap<string, long>();
            single.Set(key, coefficient);
            return single;
        });

    private static HashMap<string, long> ParseExpressionHashMap(string expr, ref int pos) =>
        ParseExpressionCore(expr, ref pos, HashMapOps);

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
            chars.Insert(0, (char)('a' + (n % AlphabetSize)));
            n = (n / AlphabetSize) - 1;
        } while (n >= 0);

        return new string(chars.ToArray());
    }
}
