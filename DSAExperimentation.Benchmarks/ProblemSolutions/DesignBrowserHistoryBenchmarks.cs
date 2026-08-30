using BenchmarkDotNet.Attributes;
using RepoDynamicArray = DSAExperimentation.DataStructures.DynamicArray.DynamicArray<string>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Browser History (LC 1472): a textbook List<string> + cursor implementation
// (RemoveRange to truncate discarded forward history) vs. this repo's own
// DynamicArray<string> doing the same truncate-then-append, the same "array
// Representation primitive vs. the BCL equivalent" comparison
// DesignCircularQueueBenchmarks already makes for Deque<int>. Each [Benchmark]
// churns a Visit/Back/Visit cycle so every iteration exercises both the truncation
// path (discarding forward history) and plain append growth.
[MemoryDiagnoser]
public class DesignBrowserHistoryBenchmarks
{
    [Params(200, 5_000)]
    public int OperationCount;

    [Benchmark(Baseline = true)]
    public string ListBacked()
    {
        var history = new ListBrowserHistory("home.com");
        var last = "home.com";

        for (var i = 0; i < OperationCount; i++)
        {
            history.Visit($"url{i}.com");
            last = history.Back(2);
            history.Visit($"branch{i}.com");
        }

        return last;
    }

    [Benchmark]
    public string DynamicArrayBacked()
    {
        var history = new DynamicArrayBrowserHistory("home.com");
        var last = "home.com";

        for (var i = 0; i < OperationCount; i++)
        {
            history.Visit($"url{i}.com");
            last = history.Back(2);
            history.Visit($"branch{i}.com");
        }

        return last;
    }

    private sealed class ListBrowserHistory
    {
        private readonly List<string> _history;
        private int _current;

        public ListBrowserHistory(string homepage) => _history = [homepage];

        public void Visit(string url)
        {
            _history.RemoveRange(_current + 1, _history.Count - _current - 1);
            _history.Add(url);
            _current++;
        }

        public string Back(int steps)
        {
            _current = Math.Max(0, _current - steps);
            return _history[_current];
        }
    }

    private sealed class DynamicArrayBrowserHistory
    {
        private readonly RepoDynamicArray _history = new();
        private int _current;

        public DynamicArrayBrowserHistory(string homepage) => _history.Add(homepage);

        public void Visit(string url)
        {
            while (_history.Count > _current + 1)
            {
                _history.RemoveAt(_history.Count - 1);
            }

            _history.Add(url);
            _current++;
        }

        public string Back(int steps)
        {
            _current = Math.Max(0, _current - steps);
            return _history.Get(_current);
        }
    }
}
