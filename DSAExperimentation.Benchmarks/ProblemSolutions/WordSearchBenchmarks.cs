using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class WordSearchBenchmarks
{
    private char[][] _board = null!; [GlobalSetup] public void Setup()=>_board=[['A','B','C','E'],['S','F','C','S'],['A','D','E','E']];
    [Benchmark(Baseline = true)] public bool SpecializedDfs()=>Exist("ABCCED");
    [Benchmark] public bool SameSearchShape()=>Exist("ABCCED");
    private bool Exist(string word){var used=new bool[_board.Length,_board[0].Length];bool Search(int r,int c,int i){if(i==word.Length)return true;if(r<0||r>=_board.Length||c<0||c>=_board[0].Length||used[r,c]||_board[r][c]!=word[i])return false;used[r,c]=true;var ok=Search(r+1,c,i+1)||Search(r-1,c,i+1)||Search(r,c+1,i+1)||Search(r,c-1,i+1);used[r,c]=false;return ok;}for(var r=0;r<_board.Length;r++)for(var c=0;c<_board[0].Length;c++)if(Search(r,c,0))return true;return false;}
}
