using System.Text;
using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Solve the Equation (LC 640): solving the pair of (coefficient, constant) sums
// itself is already O(1) closed-form arithmetic once each side has been parsed -
// the real cost lives entirely in scanning the equation string, exactly the same
// shape ComplexNumberMultiplicationBenchmarks already found for LC 537. No repo
// container or algorithm primitive applies to either parsing strategy below, so the
// comparison is between two parsing strategies for the same term scan:
// SubstringParse is the straightforward allocating baseline - it slices each term
// out as its own heap-allocated System.String via string.Substring before parsing
// it; SpanParse instead walks the same term boundaries over a ReadOnlySpan<char>
// and calls int.Parse directly on the slice, allocating nothing per term.
[MemoryDiagnoser]
public class SolveTheEquationBenchmarks
{
    [Params(200, 5_000)]
    public int TermsPerSide;

    private string _equation = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(11);
        _equation = $"{BuildSide(random, TermsPerSide)}={BuildSide(random, TermsPerSide)}";
    }

    private static string BuildSide(Random random, int termCount)
    {
        var side = new StringBuilder();

        for (var t = 0; t < termCount; t++)
        {
            if (t > 0)
            {
                side.Append(random.Next(2) == 0 ? '+' : '-');
            }

            side.Append(random.Next(1, 100));

            if (random.Next(2) == 0)
            {
                side.Append('x');
            }
        }

        return side.ToString();
    }

    [Benchmark(Baseline = true)]
    public string SubstringParse()
    {
        var separator = _equation.IndexOf('=');
        var (leftCoefficient, leftConstant) = ParseSideSubstring(_equation[..separator]);
        var (rightCoefficient, rightConstant) = ParseSideSubstring(_equation[(separator + 1)..]);
        return Combine(leftCoefficient, leftConstant, rightCoefficient, rightConstant);
    }

    [Benchmark]
    public string SpanParse()
    {
        var separator = _equation.IndexOf('=');
        var (leftCoefficient, leftConstant) = ParseSideSpan(_equation.AsSpan(0, separator));
        var (rightCoefficient, rightConstant) = ParseSideSpan(_equation.AsSpan(separator + 1));
        return Combine(leftCoefficient, leftConstant, rightCoefficient, rightConstant);
    }

    private static string Combine(int leftCoefficient, int leftConstant, int rightCoefficient, int rightConstant)
    {
        var coefficientX = leftCoefficient - rightCoefficient;
        var constant = rightConstant - leftConstant;

        if (coefficientX == 0)
        {
            return constant == 0 ? "Infinite solutions" : "No solution";
        }

        return $"x={constant / coefficientX}";
    }

    private static (int CoefficientX, int Constant) ParseSideSubstring(string side)
    {
        var coefficientX = 0;
        var constant = 0;
        var sign = 1;
        var i = 0;

        while (i < side.Length)
        {
            if (side[i] == '+')
            {
                sign = 1;
                i++;
                continue;
            }

            if (side[i] == '-')
            {
                sign = -1;
                i++;
                continue;
            }

            var start = i;

            while (i < side.Length && side[i] != '+' && side[i] != '-')
            {
                i++;
            }

            var term = side.Substring(start, i - start);

            if (term[^1] == 'x')
            {
                var digits = term[..^1];
                coefficientX += sign * (digits.Length == 0 ? 1 : int.Parse(digits));
            }
            else
            {
                constant += sign * int.Parse(term);
            }
        }

        return (coefficientX, constant);
    }

    private static (int CoefficientX, int Constant) ParseSideSpan(ReadOnlySpan<char> side)
    {
        var coefficientX = 0;
        var constant = 0;
        var sign = 1;
        var i = 0;

        while (i < side.Length)
        {
            if (side[i] == '+')
            {
                sign = 1;
                i++;
                continue;
            }

            if (side[i] == '-')
            {
                sign = -1;
                i++;
                continue;
            }

            var start = i;

            while (i < side.Length && side[i] != '+' && side[i] != '-')
            {
                i++;
            }

            var term = side[start..i];

            if (term[^1] == 'x')
            {
                var digits = term[..^1];
                coefficientX += sign * (digits.IsEmpty ? 1 : int.Parse(digits));
            }
            else
            {
                constant += sign * int.Parse(term);
            }
        }

        return (coefficientX, constant);
    }
}
