namespace DSAExperimentation.Collections.Heap;

internal readonly struct MaxHeapOrder<T> : IHeapOrder<T>
    where T : IComparable<T>
{
    public static bool HasPriority(T candidate, T incumbent) => candidate.CompareTo(incumbent) > 0;
}
