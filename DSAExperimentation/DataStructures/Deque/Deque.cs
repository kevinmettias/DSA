
namespace DSAExperimentation.DataStructures.Deque;

// No throwing PeekFront/PeekBack/PopFront/PopBack convenience: whether the deque is
// empty is state an external caller can't always know in advance, so the TryX forms
// are the only public surface for it, forcing callers onto the return-value form
// instead of a try/catch.
internal sealed class Deque<Element>
{
    private readonly CircularBuffer<Element> _items = new();

    public int Count => _items.Count;

    public void PushFront(Element item) => _items.AddFront(item);

    public void PushBack(Element item) => _items.AddBack(item);

    public bool TryPeekFront(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = _items.GetFront();
        return true;
    }

    public bool TryPeekBack(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = _items.GetBack();
        return true;
    }

    public bool TryPopFront(out Element item)
    {
        if (!TryPeekFront(out item))
        {
            return false;
        }

        _items.RemoveFront();
        return true;
    }

    public bool TryPopBack(out Element item)
    {
        if (!TryPeekBack(out item))
        {
            return false;
        }

        _items.RemoveBack();
        return true;
    }
}
