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
    public HtmlString Test1()
    {
        return Compose(ctx);
    }

    protected override HtmlString MainLayout(ViewModel vm)
    {
        return $"""
            123{c}456{DateTime.Now}789{c}0123

            {new Component2(s)}
            
            {DateTime.Now:O} {c} {c:x} {c} {c} {c} {c}
        """;
    }

    UI<ViewModel>.HtmlString GetComponent1(string name)
    {
        return $"<p>I am {name} a component</p>";
    }

    [Benchmark]
    [IterationCount(10)]
    public Placebo Placebo()
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

record struct Component2(string Name) : UI.IView
{
    public UI.HtmlString Render() => $"<p>I am {Name} a component</p>";
}

class Wat
{
    void DoIt()
    {
        Component2 c = new();
        DoIt2(ref c);
    }

    void DoIt2(ref Component2 component2)
    {

    }
}