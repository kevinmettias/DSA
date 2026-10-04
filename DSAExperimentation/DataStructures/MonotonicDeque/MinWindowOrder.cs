namespace DSAExperimentation.DataStructures.MonotonicDeque;

// The window's minimum: a resident is dominated by an arriving key at most as large.
internal readonly struct MinWindowOrder<Key> : IWindowOrder<Key>
    where Key : IComparable<Key>
{
    public static bool IsDominatedBy(Key resident, Key arriving) => arriving.CompareTo(resident) <= 0;
}
