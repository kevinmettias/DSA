using RepoDynamicArray = DSAExperimentation.DataStructures.DynamicArray.DynamicArray<string?>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAnOrderedStream;

// LeetCode 1656. Design an Ordered Stream: a fixed-size slot array plus a read cursor
// that only advances while consecutive slots are filled - exactly this repo's own
// DynamicArray<string?> (Set/Get), pre-filled with n empty slots, the same
// "sequence + access constraint" composition DesignBrowserHistoryTests already makes
// over DynamicArray<T>, just advancing on contiguous fill instead of truncating on
// write.
public sealed partial class DesignAnOrderedStreamTests
{
    [Fact]
    public void Insert_LeetCodeExample_ReturnsChunksAsGapsClose()
    {
        var stream = new OrderedStream(5);

        var afterInsert3 = stream.Insert(3, "ccccc");
        Assert.Equal([], afterInsert3);

        var afterInsert1 = stream.Insert(1, "aaaaa");
        Assert.Equal(["aaaaa"], afterInsert1);

        var afterInsert2 = stream.Insert(2, "bbbbb");
        Assert.Equal(["bbbbb", "ccccc"], afterInsert2);

        var afterInsert5 = stream.Insert(5, "eeeee");
        Assert.Equal([], afterInsert5);

        var afterInsert4 = stream.Insert(4, "ddddd");
        Assert.Equal(["ddddd", "eeeee"], afterInsert4);
    }

    [Fact]
    public void Insert_ValuesArriveInOrder_ReturnsOneValuePerCall()
    {
        var stream = new OrderedStream(3);

        var afterInsert1 = stream.Insert(1, "a");
        Assert.Equal(["a"], afterInsert1);

        var afterInsert2 = stream.Insert(2, "b");
        Assert.Equal(["b"], afterInsert2);

        var afterInsert3 = stream.Insert(3, "c");
        Assert.Equal(["c"], afterInsert3);
    }

    private sealed class OrderedStream
    {
        private readonly RepoDynamicArray _values = new();
        private int _ptr;

        public OrderedStream(int n)
        {
            for (var i = 0; i < n; i++)
            {
                _values.Add(null);
            }
        }

        public List<string> Insert(int idKey, string value)
        {
            _values.Set(idKey - 1, value);

            var chunk = new List<string>();
            while (_ptr < _values.Count && _values.Get(_ptr) is { } filled)
            {
                chunk.Add(filled);
                _ptr++;
            }

            return chunk;
        }
    }
}
