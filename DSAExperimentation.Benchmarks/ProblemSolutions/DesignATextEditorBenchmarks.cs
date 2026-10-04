using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.LeetCode.DesignATextEditor;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignATextEditorSolution's, the same classes
// DesignATextEditorSolutionTests proves correct - a textbook List<char> + cursor
// implementation (InsertRange/RemoveRange to edit at the cursor, shifting every
// character past it) against the classic two-stack cursor design built on this
// repo's own Stack<T>. Every iteration adds a five-letter chunk at the cursor and
// walks the cursor back over all of it but its first letter, so one letter per
// chunk piles up left of the cursor and four per chunk to its right: the
// List-backed version shifts that ever-growing right side on every AddText -
// O(n) per operation, O(n^2) total - while the two-stack version only touches the
// chunk itself. Each chunk starts with the next letter of the alphabet, so every
// CursorLeft report - the last ten letters left of the cursor - is different.
// Two calls per iteration keeps the larger OperationCount inside LC 2296's 2 * 10^4
// calls; chunks of 5 and steps of 4 are inside its 40.
public class DesignATextEditorBenchmarks
{
    private const int ChunkLength = 5;
    private const int StepsBack = ChunkLength - 1;
    private const char Filler = 'x';

    private string[] _chunks = [];

    // Every CursorLeft report, in call order - what each arm returns.
    private string[] _reported = [];

    [Params(200, 2_000)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var alphabet = StandardAlphabets.LowercaseLatin.Characters;
        _chunks = [.. Enumerable.Range(0, OperationCount).Select(i => alphabet[i % alphabet.Length] + new string(Filler, StepsBack))];
        _reported = new string[OperationCount];
    }

    [Benchmark(Baseline = true)]
    public string[] ListBacked() => Replay(new DesignATextEditorSolution.TextEditorByListBacked());

    [Benchmark]
    public string[] StackBacked() => Replay(new DesignATextEditorSolution.TextEditorByStackBacked());

    private string[] Replay(DesignATextEditorSolution.ITextEditor editor)
    {
        for (var i = 0; i < OperationCount; i++)
        {
            editor.AddText(_chunks[i]);
            _reported[i] = editor.CursorLeft(StepsBack);
        }

        return _reported;
    }
}
