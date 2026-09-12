using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BasicCalculatorIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BasicCalculatorIVSolution's, the same methods
// BasicCalculatorIVTests proves correct, run over a sum of Length distinct
// variables (no evalvars, so every term survives to the final, sorted output).
// DictionaryPolynomial accumulates/sorts with a plain BCL Dictionary<string,long> +
// List.Sort, HashMapMergeSort accumulates with this repo's own
// HashMap<TKey,TValue> and sorts the final term list with MergeSort over
// ArrayIndexedSequence<Term>.
[MemoryDiagnoser]
public class BasicCalculatorIVBenchmarks
{
    // Base-26 "spreadsheet column" alphabet size (a-z).
    private const int AlphabetSize = 26;

    private static readonly string[] NoEvalVars = [];
    private static readonly int[] NoEvalInts = [];

    [Params(200, 2_000)]
    public int Length;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup() => _expression = BuildExpression(Length);

    [Benchmark(Baseline = true)]
    public List<string> DictionaryPolynomial() =>
        BasicCalculatorIVSolution.EvaluateByDictionaryPolynomial(_expression, NoEvalVars, NoEvalInts);

    [Benchmark]
    public List<string> HashMapMergeSort() =>
        BasicCalculatorIVSolution.EvaluateByHashMapMergeSort(_expression, NoEvalVars, NoEvalInts);

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
