namespace DSAExperimentation.DataStructures.CircularBuffer;

// A fixed bound: the buffer holds at most Capacity elements and refuses a push beyond them.
/// <summary>The capacity of a buffer that holds at most a fixed number of elements.</summary>
public readonly struct BoundedCapacity : IBufferCapacity
{
    /// <summary>Creates a bound of <paramref name="capacity"/> elements.</summary>
    /// <param name="capacity">The most elements the buffer may hold, at least 1.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is below 1.</exception>
    public BoundedCapacity(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        Capacity = capacity;
    }

    /// <summary>The most elements the buffer may hold.</summary>
    public int Capacity { get; }

    public bool IsFullAt(int count) => count >= Capacity;
}
