namespace DSAExperimentation.DataStructures.Heap;

/// <summary>The max-heap order: a larger element outranks a smaller one.</summary>
public readonly struct MaxHeapOrder<Element> : IHeapOrder<Element>
    where Element : IComparable<Element>
{
    public static bool HasPriority(Element candidate, Element incumbent) => candidate.CompareTo(incumbent) > 0;
}
