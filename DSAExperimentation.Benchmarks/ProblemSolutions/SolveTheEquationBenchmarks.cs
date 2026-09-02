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
    // Arbitrary seed for reproducible benchmark input.
    private const int RandomSeed = 11;

    // Random.Next(BinaryChoiceBound) yields 0 or 1, used for coin-flip decisions
    // (term sign, whether a term carries the 'x' variable).
    private const int BinaryChoiceBound = 2;

    // Exclusive upper bound on a generated term's numeric coefficient magnitude.
    private const int CoefficientUpperBound = 100;

    private const string InfiniteSolutionsMessage = "Infinite solutions";
    private const string NoSolutionMessage = "No solution";

    [Params(200, 5_000)]
    public int TermsPerSide;

    private string _equation = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _equation = $"{BuildSide(random, TermsPerSide)}={BuildSide(random, TermsPerSide)}";
    }

    private static string BuildSide(Random random, int termCount)
    {
        var side = new StringBuilder();

        for (var t = 0; t < termCount; t++)
        {
            if (t > 0)
            {
                side.Append(random.Next(BinaryChoiceBound) == 0 ? '+' : '-');
            }

            var coefficient = random.Next(1, CoefficientUpperBound);
            side.Append(coefficient);

            if (random.Next(BinaryChoiceBound) == 0)
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
        var leftSpan = _equation.AsSpan(0, separator);
        var (leftCoefficient, leftConstant) = ParseSideSpan(leftSpan);
        var rightSpan = _equation.AsSpan(separator + 1);
        var (rightCoefficient, rightConstant) = ParseSideSpan(rightSpan);
        return Combine(leftCoefficient, leftConstant, rightCoefficient, rightConstant);
    }

    private static string Combine(int leftCoefficient, int leftConstant, int rightCoefficient, int rightConstant)
    {
        var coefficientX = leftCoefficient - rightCoefficient;
        var constant = rightConstant - leftConstant;

        if (coefficientX == 0)
        {
            return constant == 0 ? InfiniteSolutionsMessage : NoSolutionMessage;
        }

        return $"x={constant / coefficientX}";
    }

    private readonly record struct EquationParseState(int CoefficientX, int Constant, int Sign, int Index);

    private static (int CoefficientX, int Constant) ParseSideSubstring(string side)
    {
        var state = new EquationParseState(0, 0, 1, 0);

        while (state.Index < side.Length)
        {
            state = ConsumeTermSubstring(side, state);
        }

        return (state.CoefficientX, state.Constant);
    }

    private static bool TryConsumeSign(char c, out int sign)
    {
        if (c == '+')
        {
            sign = 1;
            return true;
        }

        if (c == '-')
        {
            sign = -1;
            return true;
        }

        sign = 0;
        return false;
    }

    private static int FindTermEnd(ReadOnlySpan<char> side, int start)
    {
        var i = start;

        while (i < side.Length && side[i] != '+' && side[i] != '-')
        {
            i++;
        }

        return i;
    }

    private static (bool IsCoefficient, int Value) ParseTermValue(ReadOnlySpan<char> term)
    {
        if (term[^1] == 'x')
        {
            var digits = term[..^1];
            return (true, digits.IsEmpty ? 1 : int.Parse(digits));
        }

        return (false, int.Parse(term));
    }

    private static EquationParseState ApplyTerm(EquationParseState state, bool isCoefficient, int value, int newIndex)
    {
        var coefficientX = state.CoefficientX;
        var constant = state.Constant;

        if (isCoefficient)
        {
            coefficientX += state.Sign * value;
        }
        else
        {
            constant += state.Sign * value;
        }

        return state with { CoefficientX = coefficientX, Constant = constant, Index = newIndex };
    }

    private static EquationParseState ConsumeTermSubstring(string side, EquationParseState state)
    {
        var i = state.Index;

        if (TryConsumeSign(side[i], out var sign))
        {
            return state with { Sign = sign, Index = i + 1 };
        }

        var end = FindTermEnd(side, i);
        var term = side.Substring(i, end - i);
        var (isCoefficient, value) = ParseTermValue(term);

        return ApplyTerm(state, isCoefficient, value, end);
    }

    private static (int CoefficientX, int Constant) ParseSideSpan(ReadOnlySpan<char> side)
    {
        var state = new EquationParseState(0, 0, 1, 0);

        while (state.Index < side.Length)
        {
            state = ConsumeTermSpan(side, state);
        }

        return (state.CoefficientX, state.Constant);
    }

    private static EquationParseState ConsumeTermSpan(ReadOnlySpan<char> side, EquationParseState state)
    {
        var i = state.Index;

        if (TryConsumeSign(side[i], out var sign))
        {
            return state with { Sign = sign, Index = i + 1 };
        }

        var end = FindTermEnd(side, i);
        var term = side[i..end];
        var (isCoefficient, value) = ParseTermValue(term);

        return ApplyTerm(state, isCoefficient, value, end);
    }
}
