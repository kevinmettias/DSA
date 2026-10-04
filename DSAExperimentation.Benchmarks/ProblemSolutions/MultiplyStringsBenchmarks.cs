using DSAExperimentation.LeetCode.MultiplyStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MultiplyStringsSolution's, the same methods
// MultiplyStringsSolutionTests proves correct - the textbook grade-school
// multiplication that adds every digit pair's product into an int[] of column
// sums and carries once at the end, against this repo's Stack<char>-based
// digit-by-digit multiply/add. Both stay correct at any length, so the operands
// run to LeetCode's 200 digits; 20 digits is already a product no machine
// integer holds.
public class MultiplyStringsBenchmarks
{
    private const int DecimalBase = 10;

    // The leading digit is drawn from 1..9, as random.Next(9) + 1, since LeetCode's
    // operands carry no leading zero.
    private const int LeadingDigitRange = 9;

    private string _num1 = "";

    private string _num2 = "";

    [Params(20, 200)]
    public int Digits { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _num1 = GenerateDigits(random, Digits);
        _num2 = GenerateDigits(random, Digits);
    }

    private static string GenerateDigits(Random random, int digits)
    {
        var chars = new char[digits];
        chars[0] = (char)('1' + random.Next(LeadingDigitRange));

        for (var i = 1; i < digits; i++)
        {
            chars[i] = (char)('0' + random.Next(DecimalBase));
        }

        return new string(chars);
    }

    [Benchmark(Baseline = true)]
    public string ColumnSums() => MultiplyStringsSolution.MultiplyByColumnSums(_num1, _num2);

    [Benchmark]
    public string StackDigitByDigit() => MultiplyStringsSolution.MultiplyByDigitStack(_num1, _num2);
}
