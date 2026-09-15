using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignATextEditor;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignATextEditorSolution's, the same classes
// DesignATextEditorTests proves correct - a textbook List<char> + cursor
// implementation (InsertRange/RemoveRange to edit at the cursor, shifting every
// character past it) against the classic two-stack cursor design built on this
// repo's own Stack<T>. Every iteration inserts a chunk right where the cursor
// already sits (position 0) and then walks the cursor back over it, so the
// List-backed version re-shifts the entire already-typed buffer on every single
// AddText - true O(n) per operation, O(n^2) total - while the two-stack version
// only ever touches the chunk itself, true O(chunk) per operation regardless of
// how large the buffer has grown, the same "positional array insert vs. LIFO
// Stack<T>" gap DesignBrowserHistoryBenchmarks already demonstrates for
// DynamicArray<T>.
[MemoryDiagnoser]
public class DesignATextEditorBenchmarks
{
    private const int ChunkLength = 5;
    private static readonly string Chunk = new('x', ChunkLength);

    [Params(200, 2_000)]
    public int OperationCount { get; set; }

    [Benchmark(Baseline = true)]
    public string ListBacked() => Replay(new DesignATextEditorSolution.TextEditorByListBacked());

    [Benchmark]
    public string StackBacked() => Replay(new DesignATextEditorSolution.TextEditorByStackBacked());

    private string Replay(DesignATextEditorSolution.ITextEditor editor)
    {
        var reported = string.Empty;

        for (var i = 0; i < OperationCount; i++)
        {
            editor.AddText(Chunk);
            reported = editor.CursorLeft(ChunkLength);
        }

        return reported;
    }
}
