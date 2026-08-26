namespace DSAExperimentation.DataStructures.Heap;

internal readonly struct MaxHeapOrder<Element> : IHeapOrder<Element>
    where Element : IComparable<Element>
{
    public static bool HasPriority(Element candidate, Element incumbent) => candidate.CompareTo(incumbent) > 0;
}
