namespace DSAExperimentation.DataStructures.Heap;

/// <summary>The min-heap order: a smaller element outranks a larger one.</summary>
public readonly struct MinHeapOrder<Element> : IHeapOrder<Element>
    where Element : IComparable<Element>
{
    public static bool HasPriority(Element candidate, Element incumbent) => candidate.CompareTo(incumbent) < 0;
}
