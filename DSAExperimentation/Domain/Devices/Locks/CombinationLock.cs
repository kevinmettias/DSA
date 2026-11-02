namespace DSAExperimentation.Domain.Devices.Locks;

// What every rotary combination lock does, for the build TSpec names. A wheel clicks one symbol up
// or down and wraps from its last symbol to its first, so its symbols are a cycle, and the lock's
// combinations are every choice of one symbol per wheel. Numbering them reads the combination as a
// number in base SymbolsPerWheel, the first wheel the most significant, so a four-digit lock's
// combination 42 is "0042".
/// <summary>The combinations of a rotary combination lock, and the turns between them.</summary>
/// <typeparam name="TSpec">The lock's build: its wheels and their symbols.</typeparam>
public static class CombinationLock<TSpec>
    where TSpec : struct, ILockSpec
{
    // A wheel clicks up or down.
    private const int DirectionsPerWheel = 2;

    private static char LastSymbol => (char)(TSpec.FirstSymbol + TSpec.SymbolsPerWheel - 1);

    /// <summary>How many combinations the lock has: its symbols per wheel, to the power of its wheels.</summary>
    public static int CombinationCount { get; } = CountCombinations();

    /// <summary>How many combinations lie one click of one wheel from any combination.</summary>
    public static int OneTurnAwayCount => DirectionsPerWheel * TSpec.WheelCount;

    /// <summary>The symbol one click up from another, the last symbol wrapping to the first.</summary>
    /// <param name="symbol">A symbol on the wheel.</param>
    /// <returns>The next symbol on the wheel.</returns>
    public static char ClickUp(char symbol)
    {
        if (symbol == LastSymbol)
        {
            return TSpec.FirstSymbol;
        }

        return (char)(symbol + 1);
    }

    /// <summary>The symbol one click down from another, the first symbol wrapping to the last.</summary>
    /// <param name="symbol">A symbol on the wheel.</param>
    /// <returns>The previous symbol on the wheel.</returns>
    public static char ClickDown(char symbol)
    {
        if (symbol == TSpec.FirstSymbol)
        {
            return LastSymbol;
        }

        return (char)(symbol - 1);
    }

    /// <summary>Spells the combination with a given number, one symbol per wheel, the first wheel first.</summary>
    /// <param name="index">The combination's number, in [0, <see cref="CombinationCount"/>).</param>
    /// <returns>The combination, such as <c>"0042"</c> for 42 on a four-digit lock.</returns>
    public static string Combination(int index)
        => string.Create(TSpec.WheelCount, index, static (wheels, number) =>
        {
            for (var wheel = wheels.Length - 1; wheel >= 0; wheel--)
            {
                wheels[wheel] = (char)(TSpec.FirstSymbol + (number % TSpec.SymbolsPerWheel));
                number /= TSpec.SymbolsPerWheel;
            }
        });

    /// <summary>Writes the combinations one click of one wheel away: each wheel up, then down, first wheel first.</summary>
    /// <param name="combination">The combination turned from.</param>
    /// <param name="into">Where to write them, at least <see cref="OneTurnAwayCount"/> long.</param>
    /// <returns>How many were written: <see cref="OneTurnAwayCount"/>.</returns>
    public static int WriteOneTurnAway(string combination, Span<string> into)
    {
        for (var wheel = 0; wheel < TSpec.WheelCount; wheel++)
        {
            var symbol = combination[wheel];
            into[DirectionsPerWheel * wheel] = WithWheelAt(combination, wheel, ClickUp(symbol));
            into[(DirectionsPerWheel * wheel) + 1] = WithWheelAt(combination, wheel, ClickDown(symbol));
        }

        return OneTurnAwayCount;
    }

    private static string WithWheelAt(string combination, int wheel, char symbol)
        => string.Create(combination.Length, (combination, wheel, symbol), static (wheels, turn) =>
        {
            turn.combination.AsSpan().CopyTo(wheels);
            wheels[turn.wheel] = turn.symbol;
        });

    private static int CountCombinations()
    {
        var count = 1;

        for (var wheel = 0; wheel < TSpec.WheelCount; wheel++)
        {
            count *= TSpec.SymbolsPerWheel;
        }

        return count;
    }
}
