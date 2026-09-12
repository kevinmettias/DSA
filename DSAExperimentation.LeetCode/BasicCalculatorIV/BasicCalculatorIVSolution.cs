using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.BasicCalculatorIV;

// LeetCode 770. Basic Calculator IV: a recursive-descent parse (+/- lowest
// precedence, * higher, parens override) that evaluates directly into a symbolic
// polynomial instead of a number, then formats the non-zero terms sorted by
// (degree desc, variable-key asc) as "coefficient*v1*v2*.." strings (bare
// coefficient for the constant term). Known variables (evalvars/evalints)
// substitute to a constant term the moment a variable is parsed.
//
// Both strategies walk the exact same grammar - expression := term (('+'|'-')
// term)*, term := factor ('*' factor)*, factor := '(' expression ')' | number |
// variable - and differ only in which collection type carries the running
// polynomial (a term key -> coefficient map) and how that collection implements
// add/negate/multiply/single-term construction. That single point of variation is
// bundled into PolynomialOps<T> and threaded through one shared parser core
// instead of keeping two structurally identical parsers in sync - the same
// "same algorithm, repo primitive vs. BCL equivalent" comparison
// BasicCalculatorSolution's Stack<(int,int)> vs. BCL Stack arms use for LC 224.
internal static class BasicCalculatorIVSolution
{
    // The textbook baseline: a plain BCL Dictionary<string,long> for the running
    // polynomial and List<T>.Sort for the final ordering - deliberately written
    // without this repo's primitives.
    public static List<string> EvaluateByDictionaryPolynomial(string expression, string[] evalvars, int[] evalints)
    {
        var known = BuildKnownValuesDict(evalvars, evalints);
        var ops = BuildDictOps(known);
        var pos = 0;
        var polynomial = ParseExpressionCore(Normalize(expression), ref pos, ops);
        var terms = CollectNonZeroTerms(polynomial);

        terms.Sort((a, b) => CompareByDegreeThenKey(a.Key, b.Key));

        return FormatTerms(terms);
    }

    // This repo's own HashMap<TKey,TValue> for the running polynomial, sorted with
    // MergeSort over ArrayIndexedSequence<Term> - the same "sort with this repo's
    // MergeSort" convention AccountsMergeSolution uses, with a custom
    // IComparer<Term> in place of StringComparer.Ordinal.
    public static List<string> EvaluateByHashMapMergeSort(string expression, string[] evalvars, int[] evalints)
    {
        var known = BuildKnownValuesHashMap(evalvars, evalints);
        var ops = BuildHashMapOps(known);
        var pos = 0;
        var polynomial = ParseExpressionCore(Normalize(expression), ref pos, ops);
        var termsArray = CollectNonZeroTerms(polynomial).ToArray();

        SortTerms(termsArray);

        return FormatTerms(termsArray);
    }

    private static string Normalize(string expression) => expression.Replace(" ", string.Empty);

    private readonly record struct Term(string Key, long Coefficient);

    private static List<string> FormatTerms(IEnumerable<Term> terms)
    {
        var formatted = new List<string>();

        foreach (var term in terms)
        {
            formatted.Add(term.Key.Length == 0 ? term.Coefficient.ToString() : $"{term.Coefficient}*{term.Key}");
        }

        return formatted;
    }

    private static int Degree(string key) => key.Length == 0 ? 0 : key.Split('*').Length;

    private static int CompareByDegreeThenKey(string keyA, string keyB)
    {
        var degreeA = Degree(keyA);
        var degreeB = Degree(keyB);
        return degreeA != degreeB ? degreeB.CompareTo(degreeA) : string.CompareOrdinal(keyA, keyB);
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

    // ---- Generic recursive-descent polynomial parser ----
    //
    // Add/Negate/Multiply/Constant/Variable are the only places the two strategies'
    // polynomial representations differ; Variable already resolves known values
    // (baked into the closure at construction time) so the shared parser core never
    // needs to know about evalvars/evalints itself.
    private readonly record struct PolynomialOps<T>(
        Func<T, T, T> Add,
        Func<T, T> Negate,
        Func<T, T, T> Multiply,
        Func<long, T> Constant,
        Func<string, T> Variable);

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
            pos++;
            var inner = ParseExpressionCore(expr, ref pos, ops);
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

            return ops.Constant(long.Parse(expr[start..pos]));
        }

        var startVar = pos;

        while (pos < expr.Length && char.IsLower(expr[pos]))
        {
            pos++;
        }

        return ops.Variable(expr[startVar..pos]);
    }

    // ---- Dictionary-backed operations (baseline) ----

    private static Dictionary<string, int> BuildKnownValuesDict(string[] evalvars, int[] evalints)
    {
        var values = new Dictionary<string, int>();

        for (var i = 0; i < evalvars.Length; i++)
        {
            values[evalvars[i]] = evalints[i];
        }

        return values;
    }

    private static PolynomialOps<Dictionary<string, long>> BuildDictOps(Dictionary<string, int> known) => new(
        AddDict,
        NegateDict,
        MultiplyDict,
        value => new Dictionary<string, long> { [string.Empty] = value },
        name => known.TryGetValue(name, out var value)
            ? new Dictionary<string, long> { [string.Empty] = value }
            : new Dictionary<string, long> { [name] = 1L });

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

    private static List<Term> CollectNonZeroTerms(Dictionary<string, long> polynomial)
    {
        var terms = new List<Term>();

        foreach (var (key, coefficient) in polynomial)
        {
            if (coefficient != 0)
            {
                terms.Add(new Term(key, coefficient));
            }
        }

        return terms;
    }

    // ---- HashMap-backed operations (repo primitive) ----

    private static HashMap<string, int> BuildKnownValuesHashMap(string[] evalvars, int[] evalints)
    {
        var values = new HashMap<string, int>();

        for (var i = 0; i < evalvars.Length; i++)
        {
            values.Set(evalvars[i], evalints[i]);
        }

        return values;
    }

    private static PolynomialOps<HashMap<string, long>> BuildHashMapOps(HashMap<string, int> known) => new(
        AddHashMap,
        NegateHashMap,
        MultiplyHashMap,
        value => SingleTerm(string.Empty, value),
        name => known.TryGetValue(name, out var value) ? SingleTerm(string.Empty, value) : SingleTerm(name, 1L));

    private static HashMap<string, long> SingleTerm(string key, long coefficient)
    {
        var single = new HashMap<string, long>();
        single.Set(key, coefficient);
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

    private static List<Term> CollectNonZeroTerms(HashMap<string, long> polynomial)
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
}
