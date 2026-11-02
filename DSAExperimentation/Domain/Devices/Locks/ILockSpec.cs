namespace DSAExperimentation.Domain.Devices.Locks;

// A rotary combination lock is fixed by three numbers: how many wheels it has, how many symbols each
// carries, and where those symbols start. A bike lock is four wheels of 0 to 9, a luggage lock three,
// a letter lock wheels of A onward. Everything else - how a wheel clicks, which combinations exist,
// which are one turn apart - follows from these, and is CombinationLock's to state.
/// <summary>The build of a rotary combination lock: its wheels and the symbols each one carries.</summary>
public interface ILockSpec
{
    /// <summary>How many wheels the lock has, one per position of a combination.</summary>
    static abstract int WheelCount { get; }

    /// <summary>
    /// How many symbols each wheel carries, at least 3, so that a click up and a click down reach
    /// different symbols.
    /// </summary>
    static abstract int SymbolsPerWheel { get; }

    /// <summary>The first symbol on each wheel; the rest follow it in character order.</summary>
    static abstract char FirstSymbol { get; }
}
