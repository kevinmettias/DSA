namespace DSAExperimentation.DataStructures.CircularBuffer;

// No bound: the buffer grows as far as its elements need, so it is never full.
/// <summary>The capacity of a buffer that grows without bound: it is never full.</summary>
public readonly struct UnboundedCapacity : IBufferCapacity
{
    public bool IsFullAt(int count) => false;
}
