using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sum Game (LC 1927): the same reduced-state minimax (leftBlanks, rightBlanks,
// sumDiff) as SumGameTests.cs, run three ways - an unmemoized recursion that
// re-explores every digit-fill order that reaches the same reduced state (the same
// "revisits collapse to far fewer distinct states than move orders" shape
// ChalkboardXorGameBenchmarks/NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks
// already demonstrate), that same recursion routed through this repo's own
// Memoizer, and the well-known closed form the recursion itself reduces to (odd
// blank count always wins Alice; otherwise compare sumDiff to
// 9 * (rightBlanks - leftBlanks) / 2).
[MemoryDiagnoser]
public class SumGameBenchmarks
{
    private const int MinDigit = 0;
    private const int MaxDigit = 9;
    private const int TurnParityDivisor = 2;
    private const int BlankCountParityDivisor = 2;
    private const int OptimalMarginDivisor = 2;

    // Kept modest: branching is 10 digits per remaining blank, so the unmemoized
    // tree is already O(10^(2 * BlanksPerSide)) - 4 and 6 total blanks keep
    // BruteForceRecursion in the thousands-to-millions of calls, not billions.
    [Params(2, 3)]
    public int BlanksPerSide;

    private int _leftBlanks;
    private int _rightBlanks;

    [GlobalSetup]
    public void Setup()
    {
        _leftBlanks = BlanksPerSide;
        _rightBlanks = BlanksPerSide;
    }

    [Benchmark(Baseline = true)]
    public bool BruteForceRecursion()
        => ResolveBruteForce((_leftBlanks, _rightBlanks, 0), _leftBlanks + _rightBlanks);

    private static bool ResolveBruteForce((int LeftBlanks, int RightBlanks, int Diff) state, int totalBlanks)
    {
        var terminalOutcome = TerminalOutcome(state);
        if (terminalOutcome.HasValue)
        {
            return terminalOutcome.Value;
        }

        var isAliceTurn = IsAliceTurn(state, totalBlanks);

        var leftOutcome = TryResolveByFillingLeft(state, totalBlanks, isAliceTurn);
        if (leftOutcome.HasValue)
        {
            return leftOutcome.Value;
        }

        var rightOutcome = TryResolveByFillingRight(state, totalBlanks, isAliceTurn);
        if (rightOutcome.HasValue)
        {
            return rightOutcome.Value;
        }

        return !isAliceTurn;
    }

    private static bool? TerminalOutcome((int LeftBlanks, int RightBlanks, int Diff) state)
    {
        if (state.LeftBlanks != 0 || state.RightBlanks != 0)
        {
            return null;
        }

        return state.Diff != 0;
    }

    private static bool IsAliceTurn((int LeftBlanks, int RightBlanks, int Diff) state, int totalBlanks)
    {
        var movesMade = totalBlanks - (state.LeftBlanks + state.RightBlanks);
        return movesMade % TurnParityDivisor == 0;
    }

    private static bool? TryResolveByFillingLeft(
        (int LeftBlanks, int RightBlanks, int Diff) state, int totalBlanks, bool isAliceTurn)
    {
        if (state.LeftBlanks == 0)
        {
            return null;
        }

        for (var digit = MinDigit; digit <= MaxDigit; digit++)
        {
            var outcome = ResolveBruteForce((state.LeftBlanks - 1, state.RightBlanks, state.Diff + digit), totalBlanks);

            if (outcome == isAliceTurn)
            {
                return isAliceTurn;
            }
        }

        return null;
    }

    private static bool? TryResolveByFillingRight(
        (int LeftBlanks, int RightBlanks, int Diff) state, int totalBlanks, bool isAliceTurn)
    {
        if (state.RightBlanks == 0)
        {
            return null;
        }

        for (var digit = MinDigit; digit <= MaxDigit; digit++)
        {
            var outcome = ResolveBruteForce((state.LeftBlanks, state.RightBlanks - 1, state.Diff - digit), totalBlanks);

            if (outcome == isAliceTurn)
            {
                return isAliceTurn;
            }
        }

        return null;
    }

    [Benchmark]
    public bool MemoizedRecursion()
    {
        var totalBlanks = _leftBlanks + _rightBlanks;

        return Memoizer.Memoize<(int LeftBlanks, int RightBlanks, int Diff), bool>(
            (_leftBlanks, _rightBlanks, 0),
            (state, aliceWins) => ResolveMemoized(state, totalBlanks, aliceWins));
    }

    private static bool ResolveMemoized(
        (int LeftBlanks, int RightBlanks, int Diff) state,
        int totalBlanks,
        Func<(int LeftBlanks, int RightBlanks, int Diff), bool> aliceWins)
    {
        var terminalOutcome = TerminalOutcome(state);
        if (terminalOutcome.HasValue)
        {
            return terminalOutcome.Value;
        }

        var isAliceTurn = IsAliceTurn(state, totalBlanks);

        var leftOutcome = TryResolveMemoizedByFillingLeft(state, isAliceTurn, aliceWins);
        if (leftOutcome.HasValue)
        {
            return leftOutcome.Value;
        }

        var rightOutcome = TryResolveMemoizedByFillingRight(state, isAliceTurn, aliceWins);
        if (rightOutcome.HasValue)
        {
            return rightOutcome.Value;
        }

        return !isAliceTurn;
    }

    private static bool? TryResolveMemoizedByFillingLeft(
        (int LeftBlanks, int RightBlanks, int Diff) state,
        bool isAliceTurn,
        Func<(int LeftBlanks, int RightBlanks, int Diff), bool> aliceWins)
    {
        if (state.LeftBlanks == 0)
        {
            return null;
        }

        for (var digit = MinDigit; digit <= MaxDigit; digit++)
        {
            var outcome = aliceWins((state.LeftBlanks - 1, state.RightBlanks, state.Diff + digit));

            if (outcome == isAliceTurn)
            {
                return isAliceTurn;
            }
        }

        return null;
    }

    private static bool? TryResolveMemoizedByFillingRight(
        (int LeftBlanks, int RightBlanks, int Diff) state,
        bool isAliceTurn,
        Func<(int LeftBlanks, int RightBlanks, int Diff), bool> aliceWins)
    {
        if (state.RightBlanks == 0)
        {
            return null;
        }

        for (var digit = MinDigit; digit <= MaxDigit; digit++)
        {
            var outcome = aliceWins((state.LeftBlanks, state.RightBlanks - 1, state.Diff - digit));

            if (outcome == isAliceTurn)
            {
                return isAliceTurn;
            }
        }

        return null;
    }

    [Benchmark]
    public bool ClosedFormFormula()
    {
        var totalBlanks = _leftBlanks + _rightBlanks;

        if (totalBlanks % BlankCountParityDivisor != 0)
        {
            return true;
        }

        return MaxDigit * (_rightBlanks - _leftBlanks) / OptimalMarginDivisor != 0;
    }
}
