using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BasicCalculatorIV;

// LeetCode 770. Basic Calculator IV: a recursive-descent parse (+/- lowest
// precedence, * higher, parens override) that evaluates directly into a symbolic
// polynomial instead of a number - a HashMap<string,long> from "sorted, '*'-joined
// variable list" (empty string for the constant term) to coefficient, the same
// accumulator-map role TwoSumTests' HashMap<int,int> plays for seen values. Known
// variables substitute to a constant term via a second HashMap<string,int>
// (evalvars -> evalints) before parsing ever starts. The final term list is sorted
// by (degree desc, variable-key asc) with this repo's own MergeSort over
// ArrayIndexedSequence<Term> - the same "sort with this repo's MergeSort" convention
// AccountsMergeTests already uses, just with a custom IComparer<Term> instead of
// StringComparer.Ordinal.
public sealed partial class BasicCalculatorIVTests
{
    [Fact]
    public void Evaluate_KnownVariableLinearExpression_ReturnsConstantFoldedTerms()
    {
        var terms = Evaluate("e + 8 - a + 5", ["e"], [1]);

        Assert.Equal(["-1*a", "14"], terms);
    }

    [Fact]
    public void Evaluate_AllVariablesUnknown_KeepsBothSymbolicLinearTerms()
    {
        var terms = Evaluate("e - 8 + temperature - 1", [], []);

        Assert.Equal(["1*e", "1*temperature", "-9"], terms);
    }

    [Fact]
    public void Evaluate_ProductOfSums_ExpandsToDegreeTwoAndConstantTerms()
    {
        var terms = Evaluate("(e + 8) * (e - 8)", [], []);

        Assert.Equal(["1*e*e", "-64"], terms);
    }

    [Fact]
    public void Evaluate_ProductOfSumsWithKnownVariable_FoldsToASingleConstant()
    {
        var terms = Evaluate("(e + 8) * (e - 8)", ["e"], [3]);

        Assert.Equal(["-55"], terms);
    }

    private readonly record struct Term(string Key, long Coefficient);

    private static List<string> Evaluate(string expression, string[] evalvars, int[] evalints)
    {
        var values = new HashMap<string, int>();

        for (var i = 0; i < evalvars.Length; i++)
        {
            values.Set(evalvars[i], evalints[i]);
        }

        var pos = 0;
        var polynomial = ParseExpression(expression.Replace(" ", string.Empty), ref pos, values);

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

        var formatted = new List<string>();

        foreach (var term in termsArray)
        {
            formatted.Add(term.Key.Length == 0 ? term.Coefficient.ToString() : $"{term.Coefficient}*{term.Key}");
        }

        return formatted;
    }

    private static int Degree(string key) => key.Length == 0 ? 0 : key.Split('*').Length;

    private static HashMap<string, long> ParseExpression(string expr, ref int pos, HashMap<string, int> values)
    {
        var result = ParseTerm(expr, ref pos, values);

        while (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-'))
        {
            var op = expr[pos];
            pos++;
            var rhs = ParseTerm(expr, ref pos, values);
            result = op == '+' ? Add(result, rhs) : Add(result, Negate(rhs));
        }

        return result;
    }

    private static HashMap<string, long> ParseTerm(string expr, ref int pos, HashMap<string, int> values)
    {
        var result = ParseFactor(expr, ref pos, values);

        while (pos < expr.Length && expr[pos] == '*')
        {
            pos++;
            var rhs = ParseFactor(expr, ref pos, values);
            result = Multiply(result, rhs);
        }

        return result;
    }

    private static HashMap<string, long> ParseFactor(string expr, ref int pos, HashMap<string, int> values)
    {
        if (expr[pos] == '(')
        {
            pos++;
            var inner = ParseExpression(expr, ref pos, values);
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

            return Constant(long.Parse(expr[start..pos]));
        }

        var startVar = pos;

        while (pos < expr.Length && char.IsLower(expr[pos]))
        {
            pos++;
        }

        return Variable(expr[startVar..pos], values);
    }

    private static HashMap<string, long> Constant(long value)
    {
        var result = new HashMap<string, long>();
        result.Set(string.Empty, value);
        return result;
    }

    private static HashMap<string, long> Variable(string name, HashMap<string, int> values)
    {
        if (values.TryGetValue(name, out var known))
        {
            return Constant(known);
        }

        var result = new HashMap<string, long>();
        result.Set(name, 1L);
        return result;
    }

    private static HashMap<string, long> Add(HashMap<string, long> a, HashMap<string, long> b)
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

    private static HashMap<string, long> Negate(HashMap<string, long> a)
    {
        var result = new HashMap<string, long>();

        foreach (var key in a.Keys)
        {
            a.TryGetValue(key, out var value);
            result.Set(key, -value);
        }

        return result;
    }

    private static HashMap<string, long> Multiply(HashMap<string, long> a, HashMap<string, long> b)
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
}
