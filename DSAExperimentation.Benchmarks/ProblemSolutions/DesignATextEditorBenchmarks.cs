using BenchmarkDotNet.Attributes;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design a Text Editor (LC 2296): a textbook List<char> + cursor implementation
// (InsertRange/RemoveRange to edit at the cursor, shifting every character past it)
// vs. the classic two-stack cursor design built on this repo's own Stack<T>. Every
// iteration inserts a chunk right where the cursor already sits (position 0) and
// then walks the cursor back over it, so the List-backed version re-shifts the
// entire already-typed buffer on every single AddText - true O(n) per operation,
// O(n^2) total - while the two-stack version only ever touches the chunk itself,
// true O(chunk) per operation regardless of how large the buffer has grown, the
// same "positional array insert vs. LIFO Stack<T>" gap DesignBrowserHistoryBenchmarks
// already demonstrates for DynamicArray<T>.
[MemoryDiagnoser]
public class DesignATextEditorBenchmarks
{
    private const int ChunkLength = 5;
    private static readonly string Chunk = new('x', ChunkLength);

    [Params(200, 2_000)]
    public int OperationCount;

    [Benchmark(Baseline = true)]
    public int ListBacked()
    {
        var editor = new ListTextEditor();

        for (var i = 0; i < OperationCount; i++)
        {
            editor.AddText(Chunk);
            editor.CursorLeft(Chunk.Length);
        }

        return editor.Length;
    }

    [Benchmark]
    public int StackBacked()
    {
        var editor = new StackTextEditor();

        for (var i = 0; i < OperationCount; i++)
        {
            editor.AddText(Chunk);
            editor.CursorLeft(Chunk.Length);
        }

        return editor.Length;
    }

    private sealed class ListTextEditor
    {
        private readonly List<char> _buffer = [];
        private int _cursor;

        public int Length => _buffer.Count;

        public void AddText(string text)
        {
            _buffer.InsertRange(_cursor, text);
            _cursor += text.Length;
        }

        public void CursorLeft(int k) => _cursor = Math.Max(0, _cursor - k);
    }

    private sealed class StackTextEditor
    {
        private readonly RepoStack _left = new();
        private readonly RepoStack _right = new();

        public int Length => _left.Count + _right.Count;

        public void AddText(string text)
        {
            foreach (var c in text)
            {
                _left.Push(c);
            }
        }

        public void CursorLeft(int k)
        {
            var moved = 0;

            while (moved < k && _left.TryPop(out var c))
            {
                _right.Push(c);
                moved++;
            }
        }
    }
}
