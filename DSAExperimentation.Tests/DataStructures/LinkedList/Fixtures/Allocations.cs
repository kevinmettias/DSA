namespace DSAExperimentation.Tests.DataStructures.LinkedList.Fixtures;

// What one object takes on the heap, read off the thread's allocation count: the last of a few
// passes, so the first can JIT the factory, the way SegmentTreeTests reads its arenas.
internal static class Allocations
{
    private const int Passes = 3;

    public static long BytesOf(Func<object> make)
    {
        long allocated = 0;

        for (var pass = 0; pass < Passes; pass++)
        {
            var before = GC.GetAllocatedBytesForCurrentThread();
            var made = make();
            allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            GC.KeepAlive(made);
        }

        return allocated;
    }
}
