using Xero;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[ShortRunJob]
[MemoryDiagnoser]
public class UI : UI<ViewModel>
{
    static Context ctx = new();
    int c = 15;
    string s = "name";

    public UI() : base()
    {
    }

    [Benchmark]
    [IterationCount(10)]
    public View Test1()
    {
        return Compose(ctx);
    }

    protected override View MainLayout(ViewModel vm)
    {
        return $"""
            123{c}456{DateTime.Now}789{c}0123

            {GetComponent1(s)}
            
            {DateTime.Now:O} {c} {c:x} {c} {c} {c} {c}
        """;
    }

    UI<ViewModel>.View GetComponent1(string name)
    {
        return $"<p>I am {s} a component</p>";
    }

    [Benchmark]
    [IterationCount(10)]
    public Placebo Placebo(Context ctx)
    {
        return $"""
            123{c}456{DateTime.Now}789{c}0123

            {GetPlaceboComponent(s)}
            
            {DateTime.Now:O} {c} {c:x} {c} {c} {c} {c}
        """;
    }

    Placebo GetPlaceboComponent(string name)
    {
        return $"<p>I am {s} a component</p>";
    }
}