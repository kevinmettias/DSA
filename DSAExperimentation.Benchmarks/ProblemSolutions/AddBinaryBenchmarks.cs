using BenchmarkDotNet.Attributes;
using BitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class AddBinaryBenchmarks
{
    private string _a = null!; private string _b = null!; [Params(200, 5_000)] public int Length; [GlobalSetup] public void Setup(){_a=new string('1',Length);_b=new string('1',Length);} 
    [Benchmark(Baseline = true)] public string CharArrayReverse(){var chars=new List<char>(); var i=_a.Length-1;var j=_b.Length-1;var carry=0; while(i>=0||j>=0||carry>0){var sum=carry;if(i>=0)sum+=_a[i--]-'0';if(j>=0)sum+=_b[j--]-'0';chars.Add((char)('0'+sum%2));carry=sum/2;} chars.Reverse(); return new string(chars.ToArray());}
    [Benchmark] public string StackBits(){var stack=new BitStack();var i=_a.Length-1;var j=_b.Length-1;var carry=0;while(i>=0||j>=0||carry>0){var sum=carry;if(i>=0)sum+=_a[i--]-'0';if(j>=0)sum+=_b[j--]-'0';stack.Push((char)('0'+sum%2));carry=sum/2;}var chars=new List<char>();while(stack.TryPop(out var bit))chars.Add(bit);return new string(chars.ToArray());}
}
