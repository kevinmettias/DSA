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
// bundled into PolynomialOps<TPolynomial> and threaded through one shared parser core
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

    private static string MergeVariables(string firstKey, string secondKey)
    {
        if (firstKey.Length == 0)
        {
            return secondKey;
        }

        if (secondKey.Length == 0)
        {
            return firstKey;
        }

        var parts = new List<string>(firstKey.Split('*'));
        parts.AddRange(secondKey.Split('*'));
        parts.Sort(StringComparer.Ordinal);
        return string.Join('*', parts);
    }

    // ---- Generic recursive-descent polynomial parser ----
    //
    // Add/Negate/Multiply/Constant/Variable are the only places the two strategies'
    // polynomial representations differ; Variable already resolves known values
    // (baked into the closure at construction time) so the shared parser core never
    // needs to know about evalvars/evalints itself.
    private readonly record struct PolynomialOps<TPolynomial>(
        Func<TPolynomial, TPolynomial, TPolynomial> Add,
        Func<TPolynomial, TPolynomial> Negate,
        Func<TPolynomial, TPolynomial, TPolynomial> Multiply,
        Func<long, TPolynomial> Constant,
        Func<string, TPolynomial> Variable);

    private static TPolynomial ParseExpressionCore<TPolynomial>(
        string expr, ref int pos, PolynomialOps<TPolynomial> ops)
    {
        var result = ParseTermCore(expr, ref pos, ops);

        while (IsAtAdditiveOperator(expr, pos))
        {
            var op = expr[pos];
            pos++;
            var rhs = ParseTermCore(expr, ref pos, ops);
            result = op == '+' ? ops.Add(result, rhs) : ops.Add(result, ops.Negate(rhs));
        }

        return result;
    }

    // The next character exists and starts a `+`/`-` term in the sum.
    private static bool IsAtAdditiveOperator(string expr, int pos)
        => pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-');

    private static TPolynomial ParseTermCore<TPolynomial>(
        string expr, ref int pos, PolynomialOps<TPolynomial> ops)
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

    // The grammar's own three cases of `factor := '(' expression ')' | number |
    // variable`: a parenthesized sub-expression, a numeric literal, or a name.
    private static TPolynomial ParseFactorCore<TPolynomial>(
        string expr, ref int pos, PolynomialOps<TPolynomial> ops)
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

    // A parenthesized factor: step past the '(' , parse the whole expression inside
    // it (recursing back up to the sum rule), and step past the ')' that closed it.
    private static TPolynomial ParseParenthesized<TPolynomial>(
        string expr, ref int pos, PolynomialOps<TPolynomial> ops)
    {
        pos++;
        var inner = ParseExpressionCore(expr, ref pos, ops);
        pos++;
        return inner;
    }

    // A numeric factor: consume its digits and evaluate them as a constant term.
    private static TPolynomial ParseNumber<TPolynomial>(
        string expr, ref int pos, PolynomialOps<TPolynomial> ops)
    {
        var start = pos;

        while (pos < expr.Length && char.IsDigit(expr[pos]))
        {
            pos++;
        }

        return ops.Constant(long.Parse(expr[start..pos]));
    }

    // A variable factor: consume its letters and resolve the name through the ops -
    // a known variable becoming its value, an unknown one staying symbolic.
    private static TPolynomial ParseVariable<TPolynomial>(
        string expr, ref int pos, PolynomialOps<TPolynomial> ops)
    {
        var start = pos;

        while (pos < expr.Length && char.IsLower(expr[pos]))
        {
            pos++;
        }

        return ops.Variable(expr[start..pos]);
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

    private static Dictionary<string, long> AddDict(
        Dictionary<string, long> leftPolynomial, Dictionary<string, long> rightPolynomial)
    {
        var result = new Dictionary<string, long>(leftPolynomial);

        foreach (var (key, value) in rightPolynomial)
        {
            result[key] = result.GetValueOrDefault(key) + value;
        }

        return result;
    }

    private static Dictionary<string, long> NegateDict(Dictionary<string, long> polynomial)
    {
        var result = new Dictionary<string, long>();

        foreach (var (key, value) in polynomial)
        {
            result[key] = -value;
        }

        return result;
    }

    private static Dictionary<string, long> MultiplyDict(
        Dictionary<string, long> leftPolynomial, Dictionary<string, long> rightPolynomial)
    {
        var result = new Dictionary<string, long>();

        foreach (var (keyA, coeffA) in leftPolynomial)
        {
            foreach (var (keyB, coeffB) in rightPolynomial)
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

    private static HashMap<string, long> AddHashMap(
        HashMap<string, long> leftPolynomial, HashMap<string, long> rightPolynomial)
    {
        var result = new HashMap<string, long>();

        foreach (var key in leftPolynomial.Keys)
        {
            leftPolynomial.TryGetValue(key, out var value);
            result.Set(key, value);
        }

        foreach (var key in rightPolynomial.Keys)
        {
            rightPolynomial.TryGetValue(key, out var value);
            result.TryGetValue(key, out var existing);
            result.Set(key, existing + value);
        }

        return result;
    }

    private static HashMap<string, long> NegateHashMap(HashMap<string, long> polynomial)
    {
        var result = new HashMap<string, long>();

        foreach (var key in polynomial.Keys)
        {
            polynomial.TryGetValue(key, out var value);
            result.Set(key, -value);
        }

        return result;
    }

    private static HashMap<string, long> MultiplyHashMap(
        HashMap<string, long> leftPolynomial, HashMap<string, long> rightPolynomial)
    {
        var result = new HashMap<string, long>();

        foreach (var keyA in leftPolynomial.Keys)
        {
            leftPolynomial.TryGetValue(keyA, out var coeffA);

            foreach (var keyB in rightPolynomial.Keys)
            {
                rightPolynomial.TryGetValue(keyB, out var coeffB);
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
