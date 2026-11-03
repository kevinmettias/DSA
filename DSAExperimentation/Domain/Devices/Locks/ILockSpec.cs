using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Devices.Locks;

// A rotary combination lock is fixed by how many wheels it has and the alphabet each wheel carries:
// a bike lock is four wheels of the digits 0 to 9, a luggage lock three, a word lock wheels of the
// letters A to Z. The wheel is an alphabet rather than a first symbol and a count, so a wheel whose
// symbols are not one run of character codes is a lock too. Everything else - how a wheel clicks,
// which combinations exist, which are one turn apart - follows, and is CombinationLock's to state.
/// <summary>The build of a rotary combination lock: how many wheels it has, each carrying <typeparamref name="TWheel"/>.</summary>
/// <typeparam name="TWheel">
/// The alphabet each wheel carries, of at least 3 symbols, so that a click up and a click down reach
/// different symbols.
/// </typeparam>
public interface ILockSpec<TWheel>
    where TWheel : struct, IAlphabet
{
    /// <summary>How many wheels the lock has, one per position of a combination.</summary>
    static abstract int WheelCount { get; }
}
