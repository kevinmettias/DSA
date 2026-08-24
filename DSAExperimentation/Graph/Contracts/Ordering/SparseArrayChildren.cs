namespace DSAExperimentation.Graph.Contracts.Ordering;

// A view over a fixed-size slot array where most slots are typically empty (null) -
// e.g. a trie node's per-character children. Count and indexing scan the slots,
// which is fine for small, bounded slot counts (a 26-letter alphabet) and avoids
// the extra storage a Dictionary-backed alternative would need just to support
// indexed access at all.
internal readonly struct SparseArrayChildren<TNode>(TNode?[] slots) : IChildren<TNode>
    where TNode : class
{
    public int Count
    {
        get
        {
            var count = 0;

            for (var i = 0; i < slots.Length; i++)
            {
                if (slots[i] is not null)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public TNode Get(int index)
    {
        for (var i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];

            if (slot is not null)
            {
                if (index == 0)
                {
                    return slot;
                }

                index--;
            }
        }

        throw new IndexOutOfRangeException();
    }
}
